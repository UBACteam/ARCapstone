
/*
    Author: Jacob Sword
    Last Edited: February 2020
        Editing Engineer: Joel Rattray 
    These comments are to test Git Commits 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class UnityARCameraManager : MonoBehaviour {

    // UI debug text elements
    public Text speedText;
    public Text warningText;
    public Text posText;
    public Text accText;
    public Text debugAccText;   //Added 6 Apr for Acceleration Debug
    public Text directionText;
    // Number of frames passed before next speed calculation
    public int speedFrames = 10;

    //Unity ARKit plugin provided variables
    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    private Material savedClearMaterial;

    [Header("AR Config Options")]
    public UnityARAlignment startAlignment = UnityARAlignment.UnityARAlignmentGravity;
    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal;
    public bool getPointCloud = true;
    public bool enableLightEstimation = true;
    public bool enableAutoFocus = true;
	public UnityAREnvironmentTexturing environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;

    [Header("Image Tracking")]
    public ARReferenceImagesSet detectionImages = null;
    public int maximumNumberOfTrackedImages = 0;

    [Header("Object Tracking")]
    public ARReferenceObjectsSetAsset detectionObjects = null;
    private bool sessionStarted = false;
    //End Unity ARKit plugin provided variables

    // Last recorded position
    private Vector3 lastPos;
    // Last recorded accelerometer reading
    private Vector3 lastAcc;
    // Current speed
    private float speed;
    // Frames passed since last calculation
    private int frames = 0;
    // Time passed since last calculation
    private float passedTime = 0;
    // Direction device is moving
    private enum Direction { forward, backward };
    private Direction dir = Direction.forward;

    // Session configuration provided by Unity ARKIT plugin
    public ARKitWorldTrackingSessionConfiguration sessionConfiguration
    {
        get
        {
            ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration ();
            config.planeDetection = planeDetection;
            config.alignment = startAlignment;
            config.getPointCloudData = getPointCloud;
            config.enableLightEstimation = enableLightEstimation;
            config.enableAutoFocus = enableAutoFocus;
            config.maximumNumberOfTrackedImages = maximumNumberOfTrackedImages;
            config.environmentTexturing = environmentTexturing;
            if (detectionImages != null)
                config.referenceImagesGroupName = detectionImages.resourceGroupName;

			if (detectionObjects != null) 
			{
				config.referenceObjectsGroupName = "";  //lets not read from XCode asset catalog right now
				config.dynamicReferenceObjectsPtr = m_session.CreateNativeReferenceObjectsSet(detectionObjects.LoadReferenceObjectsInSet());
			}

            return config;
        }
    }

    // Origin of world is set as where device is at this point
    void Start () {

        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        Application.targetFrameRate = 60;
        
        var config = sessionConfiguration;
        if (config.IsSupported) {
            m_session.RunWithConfig (config);
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
        }

        // AR camera will be the camera in the scene designated as Main Camera
        if (m_camera == null) {
            m_camera = Camera.main;
        }
    }

    void OnDestroy()
    {
        m_session.Pause();
    }

    // Start session and get initial position and accelerometer readings
    void FirstFrameUpdate(UnityARCamera cam)
    {
        sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
        lastPos = m_camera.transform.localPosition;
        lastAcc = Input.acceleration;
        lastVel= 0;             //Added 27 Apr for Acceleration-Based Test //Assume initial velocity at start is 0
    }

    void Update () {
        
        if (m_camera != null && sessionStarted)
        {
            // Positional information of AR camera
            Matrix4x4 matrix = m_session.GetCameraPose();
            Vector3 newPos = UnityARMatrixOps.GetPosition(matrix);

            passedTime += Time.deltaTime;

            // Get accelerometer difference (experimental)
            double  acc = Input.acceleration.z - lastAcc.z;     //Changed from Float to double on 6 Apr to try and get Acc data to show
            if (acc > 0)
                dir = Direction.backward;
            else if (acc < 0)
                dir = Direction.forward;

            // Do speed calculation and update UI
            if (frames % speedFrames == 0)
            {
                //double dist = Vector3.Distance(newPos, lastPos);        //Changed from Float to double on 6 Apr to try and get Acc data to show | Added 6 Apr
                //speed = dist / passedTime;        //Commented out this section on 27 Apr to include acceleration test.
                acc = Input.acceleration.z;         //Get current Z acceleration
                currentSpeed = ((acc*passedTime) + lastVel);        //Added 27 Apr for Acceleration Speed Test 

                speedText.text = "Speed " + currentSpeed.ToString();
                if (speed > 1)
                {
                    warningText.text = "Slow down!";
                    if (dir == Direction.forward)
                        directionText.text = "Forwards";
                    else
                        directionText.text = "Backwards";
                }
                else
                {
                    warningText.text = "";
                    directionText.text = "";
                }
                // Reset counters and update last recorded info
                frames = 0;
                passedTime = 0;
                lastPos = m_camera.transform.localPosition;
                lastAcc = Input.acceleration;
                lastVel = currentSpeed;         //Added 27 Apr: Set LastVel to 
                                                //currentSpeed to be able to track most recent lastVel 
                // Debug text
                posText.text = "Pos: " + m_camera.transform.localPosition;
                accText.text = "Acc: " + (string.Format("{0:0.000000000000}", (Input.acceleration - lastAcc)));  //Ensure it's formatting value to 12 places after the decimal point so that it's clear whether there's any value in there. 
                debugAccText.text = "Acc Debug: " + (Input.acceleration); //Display Current Acceleration to debug the Acceleration bug: Added 6 Apr 
                debugAccTypeText.text = "Acc Type: " + (Input.acceleration.GetType());      //Added 6 Apr: Should Display type of the Input.accleration val to determine if it's an int
            }
            frames++;
            // Move game camera to new position/rotation sensed
            m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
            m_camera.transform.localRotation = UnityARMatrixOps.GetRotation (matrix);
            m_camera.projectionMatrix = m_session.GetCameraProjection ();
        }

    }

}


// Below is code included from the ARkit plugin example for creating new camera, not needed for this example

//public void SetCamera(Camera newCamera)
//{
//    if (m_camera != null) {
//        UnityARVideo oldARVideo = m_camera.gameObject.GetComponent<UnityARVideo> ();
//        if (oldARVideo != null) {
//            savedClearMaterial = oldARVideo.m_ClearMaterial;
//            Destroy (oldARVideo);
//        }
//    }
//    SetupNewCamera (newCamera);
//}

//private void SetupNewCamera(Camera newCamera)
//{
//    m_camera = newCamera;

//    if (m_camera != null) {
//        UnityARVideo unityARVideo = m_camera.gameObject.GetComponent<UnityARVideo> ();
//        if (unityARVideo != null) {
//            savedClearMaterial = unityARVideo.m_ClearMaterial;
//            Destroy (unityARVideo);
//        }
//        unityARVideo = m_camera.gameObject.AddComponent<UnityARVideo> ();
//        unityARVideo.m_ClearMaterial = savedClearMaterial;
//    }
//}