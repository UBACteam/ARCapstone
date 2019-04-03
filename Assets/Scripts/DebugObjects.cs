using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObjects : MonoBehaviour
{
    public Material debugMaterial;

    GameObject[] cadObjs;
    GameObject parentObj;
    bool instantiated = false;

    // Use this for initialization
    void Start()
    {
        cadObjs = GameObject.FindGameObjectsWithTag("cadobject");
        parentObj = GameObject.FindGameObjectWithTag("calibration");
        foreach (var go in cadObjs)
        {
            DebugPlacement dp = go.GetComponent<DebugPlacement>();
            if (dp && dp.debug)
            {
                Debug.Log(go.GetComponent<Renderer>().bounds.size.x);
                Debug.Log(go.transform.right);
                GameObject newObj = Instantiate(
                    go,
                    go.transform.position + (-go.transform.right * go.GetComponent<Renderer>().bounds.size.x),
                    go.transform.rotation,
                    parentObj.transform
                    );
                newObj.tag = "debug";
                newObj.GetComponent<Renderer>().material = debugMaterial;
            }
        }
        ShowDebug();
        instantiated = true;
    }

    void ShowDebug()
    {
        GameObject[] debugObjs = GameObject.FindGameObjectsWithTag("debug");
        foreach (var go in debugObjs)
        {
            go.GetComponent<Renderer>().enabled = !go.GetComponent<Renderer>().enabled;
        }
    }

}