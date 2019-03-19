using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RepositionObjects : MonoBehaviour
{

    // Meters per each deck
    public int deckMeters = 3;
    public int relativeHeight = 1;
    // Meteres per each fram
    public int frameMeters = 1;

    private GameObject[] CADObjects;
    private Vector3 origin;
    private bool active = true;

    void Start()
    {
        CADObjects = GameObject.FindGameObjectsWithTag("cadobject");
        origin = Camera.main.transform.position;
    }

    void Update()
    {
        if (active && Math.Abs(origin.y - Camera.main.transform.position.y) > (deckMeters - relativeHeight))
        {
            active = false;
            foreach (GameObject go in CADObjects)
                go.SetActive(false);
        }
        else if (!active && Math.Abs(origin.y - Camera.main.transform.position.y) <= (deckMeters - relativeHeight))
        {
            active = true;
            foreach (GameObject go in CADObjects)
                go.SetActive(true);
        }
    }

    public void Reposition(InputField input)
    {
        if (input.text.Length != 3) return;
        string[] inputData = input.text.Split('-');
        int deck = int.Parse(inputData[0]);
        int frame = int.Parse(inputData[1]);
        origin = Camera.main.transform.position + new Vector3(
                0,
                -deck * deckMeters,
                frame * frameMeters
                );

        foreach (GameObject go in CADObjects)
        {
            var posData = go.GetComponent<PositionData>();
            go.transform.position = origin +
                new Vector3(0, posData.position.y, 0) +
                Camera.main.transform.forward * posData.position.z +
                Camera.main.transform.right * posData.position.x;
            go.transform.rotation = Camera.main.gameObject.transform.rotation;
        }
    }
}