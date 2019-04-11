using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugObjects : MonoBehaviour
{
    public Material debugMaterial;
    public Dropdown directionDropdown;

    bool isDebugging = false;
    GameObject debugObj;
    GameObject parentObj;
    bool getDebugPoint = false;

    void Start()
    {
        parentObj = GameObject.FindGameObjectWithTag("calibration");
    }



    public void DebugObject()
    {
        if (isDebugging)
        {
            Destroy(debugObj);
            isDebugging = false;
        }
        else
        {
            getDebugPoint = true;
        }

    }


    private void Update()
    {
        if (!getDebugPoint || Input.touchCount <= 0)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            var go = hit.transform.gameObject;
            DebugPlacement dp = go.GetComponent<DebugPlacement>();
            if (!dp || !dp.debug || go.tag != "cadobject")
                return;

            // Vector facing out from plane that was hit
            Vector3 normal = hit.normal;
            float xMag = dp.bounds.size.x;
            float yMag = dp.bounds.size.y;
            Vector3 right = go.transform.right;

            // Reassign x-bounds depending on side chosen
            if (normal == go.transform.right || normal == -go.transform.right)
            {
                xMag = dp.bounds.size.z;
            }
            // Account top/bottom placmenet when choosing sides of object
            if (normal == go.transform.right)
                right = go.transform.forward;
            else if (normal == -go.transform.right)
                right = -go.transform.forward;

            // Account for top/bottom placement when choosing back of object
            int rightMult = (normal == go.transform.forward) ? -1 : 1;

            // Directions to place debug object, depending on dropdown values
            Vector3[] directions = new Vector3[] {
                (Quaternion.AngleAxis(90, Vector3.up) * normal) * xMag,
                (Quaternion.AngleAxis(-90, Vector3.up) * normal) * xMag,
                (Quaternion.AngleAxis(90, right * rightMult) * normal) * yMag,
                (Quaternion.AngleAxis(-90, right * rightMult) * normal) * yMag,
            };
            Vector3 direction = directions[directionDropdown.value];
            // Place new debug object beside current object.
            // Add same parent so will move with the rest of the scene
            GameObject newObj = Instantiate(
                go,
                go.transform.position + direction,
                go.transform.rotation,
                parentObj.transform
                );
            newObj.GetComponent<Renderer>().material = debugMaterial;
            debugObj = newObj;
            isDebugging = true;
            getDebugPoint = false;
        }
    }
}