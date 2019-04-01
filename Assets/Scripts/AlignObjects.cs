using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AlignObjects : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public Text statusText;

    uint pointsCaptured = 1;
    Vector3 translationVectorAvg;
    bool acquire = false;
    bool dragging = false;
    Transform toDrag;
    float dist;
    Vector3 offset;
    GameObject[] cadObjs;
    GameObject mainCalibraitonObj;

    public void CapturePoint()
    {
        if (pointsCaptured == 2)
            return;
        acquire = true;
    }

    private void Start()
    {
        //cadObjs = GameObject.FindGameObjectsWithTag("cadobject");
        mainCalibraitonObj = GameObject.FindGameObjectWithTag("calibration");

    }

    private void Update()
    {
        if (pointsCaptured == 0 && acquire)
        {
            statusText.text = "Tap 1st calibration point";
            if (Input.touchCount > 0)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    var trans = hit.point - point1.transform.position;
                    translationVectorAvg = trans;
                    acquire = false;
                    pointsCaptured++;
                    statusText.text = "";
                }
            }
        }
        else if (pointsCaptured == 1 && acquire)
        {
            statusText.text = "Tap 2nd calibration point";
            if (Input.touchCount > 0)
            {
                //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
                float yAngleVal = Camera.main.transform.rotation.eulerAngles.y - 180;
                //var hit = new RaycastHit();
                //if (Physics.Raycast(ray, out hit))
                //{
                //var trans = hit.point - point2.transform.position;
                var trans = point - point2.transform.position;
                translationVectorAvg = trans;
                //translationVectorAvg = (translationVectorAvg + trans) / 2;
                /*foreach (GameObject go in cadObjs)
                {
                    go.transform.position += translationVectorAvg;
                }*/
                mainCalibraitonObj.transform.position += translationVectorAvg;
                mainCalibraitonObj.transform.eulerAngles = new Vector3(0f, yAngleVal, 0f);
                acquire = false;
                pointsCaptured++;
                statusText.text = "";
                //}
            }
        }
        else
        {
            Vector3 v3;

            if (Input.touchCount != 1)
            {
                dragging = false;
                return;
            }

            Touch touch = Input.touches[0];
            Vector3 pos = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(pos);
                if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "cadobject"))
                {
                    toDrag = hit.transform;
                    dist = hit.transform.position.z - Camera.main.transform.position.z;
                    v3 = new Vector3(pos.x, pos.y, dist);
                    v3 = Camera.main.ScreenToWorldPoint(v3);
                    offset = toDrag.position - v3;
                    dragging = true;
                }
            }
            if (dragging && touch.phase == TouchPhase.Moved)
            {
                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
                v3 = Camera.main.ScreenToWorldPoint(v3);
                var lastPosition = toDrag.position;
                toDrag.position = v3 + offset;
                var newPosition = toDrag.position;
                var diff = newPosition - lastPosition;
                foreach (GameObject go in cadObjs)
                {
                    if (go != toDrag)
                    {
                        go.transform.position += diff;
                    }
                }

            }
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                dragging = false;
            }
        }
    }
}