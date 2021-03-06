﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeasureObjects : MonoBehaviour
{
    // The material to apply to mirrored objects
    public Material graduatedMaterial;  
    // The dropdown where the user chooses to what side of the face they want the mirrored object
    public Dropdown directionDropdown;
    // The number of graduations on the mirrored object's skin - to be used in calculations
    public int graduations;
    // To output the size of each graduation
    public Text measurementText;

    bool isMeasuring = false;
    GameObject mirrorObj;
    GameObject parentObj;
    bool getMeasurement = false;

    void Start()
    {
        parentObj = GameObject.FindGameObjectWithTag("calibration");
    }



    public void MeasureObject()
    {
        if (isMeasuring)
        {
            Destroy(mirrorObj);
            isMeasuring = false;
            measurementText.text = "";
        }
        else
        {
            measurementText.text = "Tap face to measure from";
            getMeasurement = true;
        }

    }


    private void Update()
    {
        if (!getMeasurement || Input.touchCount <= 0)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            var go = hit.transform.gameObject;
            MeasureObject dp = go.GetComponent<MeasureObject>();
            if (!dp || !dp.measure || go.tag != "cadobject")
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

            // Directions to place mirrored object, depending on dropdown values
            Vector3[] directions = new Vector3[] {
                (Quaternion.AngleAxis(90, Vector3.up) * normal) * xMag,
                (Quaternion.AngleAxis(-90, Vector3.up) * normal) * xMag,
                (Quaternion.AngleAxis(90, right * rightMult) * normal) * yMag,
                (Quaternion.AngleAxis(-90, right * rightMult) * normal) * yMag,
            };
            Vector3 direction = directions[directionDropdown.value];
            // Place new mirrored object beside current object.
            // Add same parent so will move with the rest of the scene
            GameObject newObj = Instantiate(
                go,
                go.transform.position + direction,
                go.transform.rotation,
                parentObj.transform
                );
            measurementText.text = ((xMag * 100) / graduations).ToString("F2") + "cm/grad horizontal\n" + 
                ((yMag * 100) / graduations).ToString("F2") + "cm/grad vertical";
            newObj.GetComponent<Renderer>().material = graduatedMaterial;
            mirrorObj = newObj;
            isMeasuring = true;
            getMeasurement = false;
        }
    }
}