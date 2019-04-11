using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlacement : MonoBehaviour
{
    public bool debug = false;
    public Material debugMaterial;

    public Dictionary<string, GameObject> debugObjs =
        new Dictionary<string, GameObject>();

    Quaternion rotation;
    Transform parent;

    private void AddDebugObj(string direction, Vector3 position)
    {
        var debugObj = Instantiate(gameObject, position, rotation, parent);
        var rend = debugObj.GetComponent<Renderer>();
        rend.enabled = false;
        debugObj.SetActive(false);
        rend.material = debugMaterial;
        debugObjs.Add(direction, debugObj);
    }

    public void InstantiateDebugObjs()
    {
        if (!debug)
            return;
        rotation = gameObject.transform.rotation;
        parent = gameObject.transform.parent;
        Vector3 bounds = gameObject.GetComponent<Renderer>().bounds.size;
        Vector3 position = gameObject.transform.position;

        AddDebugObj("left", position + (-Vector3.right * bounds.x));
        AddDebugObj("right", position + (Vector3.right * bounds.x));
        AddDebugObj("up", position + (Vector3.up * bounds.y));
        AddDebugObj("down", position + (-Vector3.up * bounds.y));
        AddDebugObj("forward", position + (Vector3.forward * bounds.z));
        AddDebugObj("backward", position + (-Vector3.forward * bounds.z));
    }
}