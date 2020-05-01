

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using UnityEngine.VR;
using System;
using UnityEngine.UI;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public Text Error;

   
    private ARPlaneManager arPlane;
    private ARRaycastManager arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = true;

    void Start()
    {
        arOrigin = FindObjectOfType<ARRaycastManager>();
        arPlane = FindObjectOfType<ARPlaneManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        //UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        //var placementIndicatorRender = placementIndicator.GetComponent<Renderer>();
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        
        arOrigin.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
           
           
            ARPlane plane = arPlane.GetPlane(hits[0].trackableId);
            
            if (plane.alignment == PlaneAlignment.HorizontalUp)

            {
                placementPose = hits[0].pose;
                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                //placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
                UpdatePlacementIndicator();
                Error.text = "This is the floor";

                GameObject Cube = placementIndicator.transform.GetChild(0).gameObject;
                GameObject leftPlane = Cube.transform.GetChild(0).gameObject;
                GameObject topPlane = Cube.transform.GetChild(1).gameObject;
                GameObject rightPlane = Cube.transform.GetChild(2).gameObject;
                GameObject frontPlane = Cube.transform.GetChild(3).gameObject;

                Vector3 leftPlanePosition = leftPlane.transform.position;
                Vector3 topPlanePosition = topPlane.transform.position;
                Vector3 rightPlanePosition = rightPlane.transform.position;
                Vector3 frontPlanePosition = frontPlane.transform.position;



                //Pose leftPlane = placementIndicator.transform.Find("Left Plane").gameObject.GetComponent<Pose>();

                Vector3 top = placementPose.up;
                Vector3 right = placementPose.right; //new Vector3(1, 0, 0);
                //Vector3 left = Quaternion.AngleAxis(right,     ;//new Vector3(-1, 0, 0);
                Vector3 front = placementPose.forward; //new Vector3(0, 0, -1);


                /*Renderer rend = placementIndicator.GetComponent<Renderer>();
                float x1 = rend.bounds.max.x;
                float x2 = rend.bounds.min.x;
                float y1 = rend.bounds.max.y;
                float y2 = rend.bounds.min.y;
                float z1 = rend.bounds.max.z;
                float z2 = rend.bounds.min.z;
                Vector3 topRightCorner = new Vector3(x1, y1, z1);
                Vector3 bottomLeftCorner = new Vector3(x2, y2, z2);

                float distance = Mathf.Sqrt(Mathf.Pow((x2 - x1), 2) + Mathf.Pow((y2 - y1), 2) + Mathf.Pow((z2 - z1), 2)); // distance formula
                float sideLength = Mathf.Sqrt(3) / distance; // Math stuff to get length of sides
                float halfSide = sideLength / 2;
                float diagonal = Mathf.Sqrt(2) * halfSide;

                float xf = x1 - 
                */


                //placementIndicatorRender.material.SetColor("_Color", Color.blue);

                //Ray leftRay = new Ray(leftPlanePosition, left);
                Ray topRay = new Ray(topPlanePosition, top);
                Ray rightRay = new Ray(rightPlanePosition, right);
                Ray frontRay = new Ray(frontPlanePosition, front);


                var leftHit = new List<ARRaycastHit>();
                var topHit = new List<ARRaycastHit>();
                var rightHit = new List<ARRaycastHit>();
                var frontHit = new List<ARRaycastHit>();
                //arOrigin.Raycast(leftRay, leftHit, TrackableType.All);
                arOrigin.Raycast(topRay, topHit, TrackableType.All);
                arOrigin.Raycast(rightRay, rightHit, TrackableType.All);
                arOrigin.Raycast(frontRay, frontHit, TrackableType.All);
                


               /* if (leftHit[0].distance < 0.05)
                {
                    Error.text = "Left Side Collision";
                }*/

                if (topHit[0].distance < 0.10)
                {
                    Error.text = "Up Side Collision";
                }

                if (rightHit[0].distance < 0.10)
                {
                    Error.text = "Right Side Collision";
                }

                if (frontHit[0].distance < 0.10)
                {
                    Error.text = "Front Side Collision";
                }














            }
            else
            {
                Error.text = "Collision with Wall!";
                
                //placementIndicatorRender.material.SetColor("_Color", Color.red);

            }
        }
        // Next is to detect collisions with planes

        // Method 1
        // Using cube (each side has 4 corners)
        // Send a ray from each corner perpendiculer from cube side
        // If ray hits a plane determine distance from corner is less than 0.01m then display error
        // Only need to to do 4 sides (Top, left, right, foward)
        // Cast 16 rays every frame

        // Method 2
        // If cube is on horizontal then no change.
        // If cube is on vertical plane then change color of cube
        // Maybe: change raycast to feature points, check if plane then place object
        // That way if it detects a small object (no planes) then it would still work
    }
}
























/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;
    //public Text Error;
    //public string ErrorText;
    
    private ARRaycastManager arRaycastManager;   // Ref to session origin
    private ARPlaneManager arPlane;
    private Pose placementPose;         // Position in space of cube 
    private bool placementPoseIsValid = true; // Checks if flat plane is detected
    



    // Start is called before the first frame update
    void Start()
    {
        
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

    }

    // Update is called once per frame
    void Update()
    {
        // Check world around us
        // Find where camera is pointing
        // Able to place virtual object
        UpdatePlacementPose();
        //UpdatePlacementIndicator();

    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)  // Checks if valid
        {
            placementIndicator.SetActive(true); // Makes indicator visible
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        // Center of screen described in pixels
        // Vector3
        var placementIndicatorRender = placementIndicator.GetComponent<Renderer>();
        Vector2 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        // Represent a list of any points in physical space where the ray hits a physical surface
        var hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
        {
            //arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

            // Sends a ray out of some point on the screen and detects any real-word surfaces.
            // Returns hit of planes
            // Trackable types (17:01)

            // Returns either empty hit list (no planes in front of camera)
            // Or hit list with one or more items




            //placementPoseIsValid = hits.Count > 0;
            int n = 0;
            //if (placementPoseIsValid)

            //ARRaycastHit hit = hits[0];
            foreach (ARRaycastHit hit in hits)
            {

                placementPose = hits[0].pose;
                //bool x = UnityEngine.XR.ARSubsystems.PlaneAlignmentExtensions.IsHorizontal(arPlane.GetComponent<ARPlaneManager>().GetPlane(hit.trackableId).alignment);
                //if (arPlane.GetComponent<ARPlaneManager>().GetPlane(hit.TrackableId).alignment != UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                //if (x)
                ARPlane plane = arPlane.GetPlane(hit.trackableId);
                if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                {
                    placementIndicatorRender.material.SetColor("_Color", Color.blue);
                }
                else
                {
                    placementPose = hits[n].pose;   // placement pose constantly updated (rot. & pos.)
                    placementIndicatorRender.material.SetColor("_Color", Color.red);
                    placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
                    
                  
                }
                n++;
            }
        }
    }
}
            //arOrigin.GetComponent<ARRaycastManager>().Raycast(placementPose.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
            //if (arPlane.GetComponent<ARPlaneManager>().GetPlane(hits[0].trackableId).alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
            //{
            //    DisplayError();
            //}

        //if (arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
        

            //foreach (ARRaycastHit hit in hits)
            //{

                /*if (arPlane.GetComponent<ARPlaneManager>().GetPlane(hits[0].trackableId).alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
                {

                    placementPose = hits[0].pose;   // placement pose constantly updated (rot. & pos.)
                    var cameraFoward = Camera.current.transform.forward; // Vector that acts a arrow describes direction camera facing
                    var cameraBearing = new Vector3(cameraFoward.x, 0, cameraFoward.z).normalized; // Give direction except for y
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                    
                }
            //}

        }

    }
}

        /*else
        {
            placementPose = hits[0].pose;   // placement pose constantly updated (rot. & pos.)
            var cameraFoward = Camera.current.transform.forward; // Vector that acts a arrow describes direction camera facing
            var cameraBearing = new Vector3(cameraFoward.x, 0, cameraFoward.z).normalized; // Give direction except for y
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }*/








/*private void DisplayError()
{
    ErrorText = "ERROR";
    Error.text = ErrorText;
}*/
