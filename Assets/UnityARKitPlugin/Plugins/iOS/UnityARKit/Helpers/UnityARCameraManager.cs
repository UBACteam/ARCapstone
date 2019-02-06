using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class UnityARCameraManager : MonoBehaviour {

    public Text speedText;
    public Text warningText;
    public Text posText;
    public Text accText;
    public int speedFrames = 10;

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

    private Vector3 lastPos;
    private float speed;
    private int frames = 0;
    private float passedTime = 0;

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

    void Start () {

        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        Application.targetFrameRate = 60;
        
        var config = sessionConfiguration;
        if (config.IsSupported) {
            m_session.RunWithConfig (config);
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
        }

        if (m_camera == null) {
            m_camera = Camera.main;
        }
    }

    void OnDestroy()
    {
        m_session.Pause();
    }

    void FirstFrameUpdate(UnityARCamera cam)
    {
        sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
        lastPos = m_camera.transform.localPosition;
    }

    void Update () {
        
        if (m_camera != null && sessionStarted)
        {
            Matrix4x4 matrix = m_session.GetCameraPose();
            Vector3 newPos = UnityARMatrixOps.GetPosition(matrix);
            passedTime += Time.deltaTime;
            if (frames % speedFrames == 0) {
                float dist = Vector3.Distance(newPos, lastPos);
                speed = dist / passedTime;
                speedText.text = "Speed " + speed.ToString();
                warningText.text = speed > 1 ? "Slow down!" : "";
                Vector3 curPos = m_camera.transform.localPosition;
                posText.text = "Pos: " + curPos;
                frames = 0;
                passedTime = 0;
                lastPos = curPos;

                float curAcc = Input.acceleration.z;
                accText.text = "Acc: " + curAcc;
            }
            frames++;
            // Transfer AR session pose to Unity Camera
            m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
            m_camera.transform.localRotation = UnityARMatrixOps.GetRotation (matrix);
            m_camera.projectionMatrix = m_session.GetCameraProjection ();
        }

    }

}


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