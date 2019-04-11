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
    bool getDebugPoint = false;

    void Start()
    {
        GameObject[] debugObjs = GameObject.FindGameObjectsWithTag("cadobject");
        foreach (var go in debugObjs)
        {
            var dp = go.GetComponent<DebugPlacement>();
            if (dp)
                dp.InstantiateDebugObjs();
        }
    }



    public void DebugObject()
    {
        if (isDebugging)
        {
            debugObj.GetComponent<Renderer>().enabled = false;
            //debugObj.SetActive(false);
            isDebugging = false;
        }
        else
            getDebugPoint = true;
    }


    private void Update()
    {
        if (!getDebugPoint || !Input.GetMouseButtonDown(0))
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            var go = hit.transform.gameObject;
            DebugPlacement dp = go.GetComponent<DebugPlacement>();
            if (!dp || !dp.debug || go.tag != "cadobject")
                return;

            // Vector facing into plane that was hit
            Vector3 facing = -hit.normal;

            GameObject[] directions = new GameObject[4];
            directions[2] = dp.debugObjs["up"];
            directions[3] = dp.debugObjs["down"];
            if (facing == go.transform.forward)
            {
                directions[0] = dp.debugObjs["left"];
                directions[1] = dp.debugObjs["right"];
            }
            else if (facing == -go.transform.forward)
            {
                directions[0] = dp.debugObjs["right"];
                directions[1] = dp.debugObjs["left"];
            }
            else if (facing == -go.transform.right)
            {
                directions[0] = dp.debugObjs["backward"];
                directions[1] = dp.debugObjs["forward"];
            }
            else if (facing == go.transform.right)
            {
                directions[0] = dp.debugObjs["forward"];
                directions[1] = dp.debugObjs["backward"];
            }
            debugObj = directions[directionDropdown.value];
            debugObj.SetActive(true);
            debugObj.GetComponent<Renderer>().enabled = true;
            isDebugging = true;
            getDebugPoint = false;
        }
    }
}