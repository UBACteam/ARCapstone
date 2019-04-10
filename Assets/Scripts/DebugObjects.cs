using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObjects : MonoBehaviour
{
    public Material debugMaterial;

    GameObject[] cadObjs;
    List<GameObject> debugObjs = new List<GameObject>();
    GameObject parentObj;
    bool instantiated = false;

    // Use this for initialization
    void Start()
    {
        cadObjs = GameObject.FindGameObjectsWithTag("cadobject");
        parentObj = GameObject.FindGameObjectWithTag("calibration");
    }

    void InitDebugObjects()
    {
        foreach (var go in cadObjs)
        {
            DebugPlacement dp = go.GetComponent<DebugPlacement>();
            if (dp && dp.debug)
            {
                // Place new debug object beside current object.
                // Add same parent so will move with the rest of the scene
                GameObject newObj = Instantiate(
                    go,
                    go.transform.position + (-go.transform.right * dp.bounds.size.x),
                    go.transform.rotation,
                    parentObj.transform
                    );
                newObj.GetComponent<Renderer>().material = debugMaterial;
                debugObjs.Add(newObj);
            }
        }
        instantiated = true;
    }

    void ToggleDebugObjects()
    {
        foreach (var go in debugObjs)
        {
            go.GetComponent<Renderer>().enabled = !go.GetComponent<Renderer>().enabled;
        }
    }

    void ShowDebug()
    {
        if (!instantiated)
            InitDebugObjects();
        else
            ToggleDebugObjects();
    }

}