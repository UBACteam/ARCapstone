using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureObject : MonoBehaviour
{
    public bool measure = false;
    public Bounds bounds;

    private void Awake()
    {
        if (!measure)
            return;
        // Initial bounds. 
        // Inititialized here because bound directions after rotation will not be true to visible object size
        bounds = gameObject.GetComponent<Renderer>().bounds;
    }
}