using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AlignObjects : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public Text statusText;

    uint pointsCaptured = 0;
    Vector3 translationVectorSum = new Vector3(0, 0, 0);
    float translationAngleSum = 0f;
    bool acquire = false;
    bool dragging = false;
    Transform toDrag;
    float dist;
    Vector3 offset;
    GameObject mainCalibrationObj;
    GameObject[] cadObjs;
    readonly int numCalPoints = 2;

    public void CapturePoint()
    {
        if (pointsCaptured == 2)
            return;
        StartCoroutine("SetAcquire");
    }

    private IEnumerator SetAcquire()
    {
        yield return new WaitForSeconds(0.5f);
        acquire = true;

    }

    private void Start()
    {
        mainCalibrationObj = GameObject.FindGameObjectWithTag("calibration");
        cadObjs = GameObject.FindGameObjectsWithTag("cadobject");

    }

    private void AcquirePoint()
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        var calPoint = pointsCaptured == 0 ? point1 : point2;
        var trans = point - calPoint.transform.position;
        float yAngleVal = Camera.main.transform.rotation.eulerAngles.y - 180;
        translationVectorSum += trans;
        translationAngleSum += yAngleVal;
        acquire = false;
        pointsCaptured++;
        statusText.text = "";
    }


    private void Update()
    {
        if (pointsCaptured == 0 && acquire)
        {
            statusText.text = "Tap 1st calibration point";
            if (Input.touchCount > 0)
                AcquirePoint();
        }
        else if (pointsCaptured == 1 && acquire)
        {
            statusText.text = "Tap 2nd calibration point";
            if (Input.touchCount > 0)
            {
                AcquirePoint();
                mainCalibrationObj.transform.position += translationVectorSum / numCalPoints;
                mainCalibrationObj.transform.eulerAngles = new Vector3(0f, translationAngleSum / numCalPoints, 0f);
            }
        }
        else
            HandleDrag();
    }


    private void HandleDrag()
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