using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AlignObjects : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject pointMarker;
    public Text statusText;

    uint pointsCaptured = 0;
    Vector3 translationVector;
    bool acquire = false;
    bool dragging = false;
    Transform toDrag;
    float dist;
    Vector3 offset;
    GameObject mainCalibrationObj;
    List<GameObject> cadObjs = new List<GameObject>();
    int numCalPoints;
    private GameObject[] points;
    private readonly Vector3[] capturedPoints = new Vector3[3];


    // Set acquire for all points to capture
    public void CapturePoint()
    {
        if (pointsCaptured == 3)
            return;
        StartCoroutine("SetAcquire");
    }

    // Set acquire after delay to account for touch for button press
    private IEnumerator SetAcquire()
    {
        yield return new WaitForSeconds(0.5f);
        acquire = true;

    }

    private void Start()
    {
        mainCalibrationObj = GameObject.FindGameObjectWithTag("calibration");
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("cadobject"))
        {
            cadObjs.Add(go);
        }
        cadObjs.Add(mainCalibrationObj);
        points = new GameObject[3] { point1, point2, point3 };
        numCalPoints = points.Length;
    }

    private void AcquirePoint()
    {
        // Cal point is just beyond finger on screen in real-word coords (min draw dist from screen)
        Vector3 capPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Instantiate(pointMarker, capPoint, Quaternion.identity);
        if (pointsCaptured < 3)
        {
            capturedPoints[pointsCaptured] = capPoint;
        }
        if (pointsCaptured == 2)
        {
            translationVector = capPoint - points[2].transform.position;
        }
        acquire = false;
        pointsCaptured++;
        statusText.text = "";
    }


    private void Update()
    {
        if (pointsCaptured < points.Length - 1 && acquire)
        {
            statusText.text = "Tap calibration point " + (pointsCaptured + 1);
            if (Input.touchCount > 0)
            {
                AcquirePoint();

            }
        }
        else if (pointsCaptured < points.Length && acquire)
        {
            statusText.text = "Tap calibration point " + (pointsCaptured + 1);
            if (Input.touchCount > 0)
            {
                AcquirePoint();
                mainCalibrationObj.transform.position += translationVector;
                // Rotation is angle between forward in world coordinates and normal vector of calibration plane
                Vector3 side1 = capturedPoints[1] - capturedPoints[0];
                Vector3 side2 = capturedPoints[2] - capturedPoints[0];
                Vector3 normal = Vector3.Cross(side2, side1);
                float transAngle = Vector3.Angle(mainCalibrationObj.transform.forward, normal);
                mainCalibrationObj.transform.eulerAngles = new Vector3(0f, transAngle, 0f);
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
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "calibration"))
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