using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlacement : MonoBehaviour
{
    public bool debug = false;
    public Bounds bounds;

    private void Awake()
    {
        if (!debug)
            return;
        // Initial bounds. 
        // Inititialized here because bound directions after rotation will not be true to visible object size
        bounds = gameObject.GetComponent<Renderer>().bounds;
    }
}