using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;



public class SnapBox : MonoBehaviour
{
    public Transform p_transform;
    private bool selected = false;

    private void ChangeColor(Color color)
    {
        //Change color
        Renderer rend = GetComponent<Renderer>();

        ////Set the main Color of the Material to green
        //rend.material.shader = Shader.Find("_Color");
        //rend.material.SetColor("_Color", Color.green);

        //Find the Specular shader and change its Color to red
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", color);
    }

    void Update()
    {
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (!selected && raycastHit.collider.name == name)
                {
                    ChangeColor(Color.red);
                    selected = true;
                } else if (raycastHit.collider.name == "Plane")
                {
                    GameObject plane = raycastHit.transform.gameObject;
                    Renderer planeRend = plane.GetComponent<Renderer>();
                    float planeBound = planeRend.bounds.size.x;
                    //Transform boxTrans = GetComponent<Transform>();
                    //boxTrans.parent = plane.transform;
                    //Renderer boxRend = GetComponent<Renderer>();
                    //float boxBounds = boxRend.bounds.size.y;
                    //boxTrans.position = new Vector3(plane.transform.position.x, plane.transform.position.y + boxBounds / 2, plane.transform.position.z);
                    Vector3 rcpos = raycastHit.transform.position;
                    //p_transform.position = new Vector3(rcpos.x + planeBound / 2, rcpos.y, rcpos.z);
                    p_transform.position = new Vector3(rcpos.x, rcpos.y, rcpos.z);
                    selected = false;
                    ChangeColor(Color.white);
                }
            }
        }
    }
}