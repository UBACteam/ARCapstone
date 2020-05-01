
// -------------------------IMPORTS---------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using UnityEngine.VR;
using System;
using UnityEngine.UI;
//------------------------------------------------------


public class ARTapToPlaceObject : MonoBehaviour
{
    // Public variables 
    public GameObject objectToPlace;        // Object to be placed
    public GameObject placementIndicator;   // Object to display in center of screen
    public Text Error;                      // Error text to dispay

    // Private variables
    private ARPlaneManager arPlane;
    private ARRaycastManager arOrigin;
    private Pose placementPose;

    void Start()
    {
        arOrigin = FindObjectOfType<ARRaycastManager>();  // Raycast Manager
        arPlane = FindObjectOfType<ARPlaneManager>();     // Plane Manager
    }

    // Called every frame
    void Update()
    {
        UpdatePlacementPose();
       
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) // Checks for a screen tap
        {
            PlaceObject();
        }
    }

    // Places objectToPlace
    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }


    // Used to update placement of PlacementIndicator
    private void UpdatePlacementPose()
    {
        // Gets screen center pointing away from device
        // Right hand corner of screen is (1,1) and bottom left is (0,0)
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        // Creates list of ARRaycastHits
        var hits = new List<ARRaycastHit>();
        // Cast ray using screencenter and stores in hits list
        // Only tracks if collides with finite ploygon
        arOrigin.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        // If there was a hit
        if (hits.Count > 0)
        {
           
            // Initializes an ARPlane to hit plane 
            ARPlane plane = arPlane.GetPlane(hits[0].trackableId);
             
            // Checks if plane is horizontal up (floor)
            if (plane.alignment == PlaneAlignment.HorizontalUp)

            {
                // Updates PlacementPose to raycast hit pose
                placementPose = hits[0].pose;

                // Get rotational pose of camera in x & y
                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);

                // Set placementIndicator position and rotation
                placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

                // Displays error text
                Error.text = "This is the floor";

                // -------------------------------------------------------------------------------------
                // THE REST OF THIS IF STATEMENT IS FOR FUTURE DEVELOPMENT (STILL IN TESTING)
                // Feel free to comment this block out to use a different method. 

                // Gets child of placementIndicator (Cube)
                GameObject Cube = placementIndicator.transform.GetChild(0).gameObject;

                // Gets children of cube
                // Each side of the cube (except bottom & back sides) has another game object attached to
                // determine location of the side of the cubes 
                GameObject leftPlane = Cube.transform.GetChild(0).gameObject;
                GameObject topPlane = Cube.transform.GetChild(1).gameObject;
                GameObject rightPlane = Cube.transform.GetChild(2).gameObject;
                GameObject frontPlane = Cube.transform.GetChild(3).gameObject;

                // Gets postion of each side
                Vector3 leftPlanePosition = leftPlane.transform.position; // current not used
                Vector3 topPlanePosition = topPlane.transform.position;
                Vector3 rightPlanePosition = rightPlane.transform.position;
                Vector3 frontPlanePosition = frontPlane.transform.position;

                // Receives direction vector of up, front, right
                // No left direction implemented
                // Possible solution is to get right direction of left plane then
                // multiply by 180 degrees in x-dimension
                Vector3 top = placementPose.up;
                Vector3 right = placementPose.right; //new Vector3(1, 0, 0)
                Vector3 front = placementPose.forward; //new Vector3(0, 0, -1);


                // This is a second method to cast rays out of the cube (NOT TESTED)
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
               
                // Rays are casted out the planes
                Ray topRay = new Ray(topPlanePosition, top);
                Ray rightRay = new Ray(rightPlanePosition, right);
                Ray frontRay = new Ray(frontPlanePosition, front);
                //Ray leftRay = new Ray(leftPlanePosition, left); // After left direction is calcuated

                // Creates a AR Raycast Hit for each plane
                var leftHit = new List<ARRaycastHit>();
                var topHit = new List<ARRaycastHit>();
                var rightHit = new List<ARRaycastHit>();
                var frontHit = new List<ARRaycastHit>();

                // Cast ray for each plane to collide with feature points
                arOrigin.Raycast(topRay, topHit, TrackableType.All);
                arOrigin.Raycast(rightRay, rightHit, TrackableType.All);
                arOrigin.Raycast(frontRay, frontHit, TrackableType.All);
                //arOrigin.Raycast(leftRay, leftHit, TrackableType.All); // To be implemented

                // If distance from plane side (origin of ray) to the collision of feature point
                // is less than 10 cm then display an error
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

                /* Implement later
                if (leftHit[0].distance < 0.05)
                 {
                     Error.text = "Left Side Collision";
                 }*/


                // ---------------------------------------------------------------------------


            }

            // Else plane must be vertical
            else
            {
                Error.text = "Collision with Wall!";
                
                //placementIndicatorRender.material.SetColor("_Color", Color.red);

            }
        }
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
