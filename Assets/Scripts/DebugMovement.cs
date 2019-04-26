using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class DebugMovement : MonoBehaviour {

    public Text speedText;
    public Text warningText;
    public Text posText;
    public Text accText;
    public int speedFrames = 10;

    Camera m_camera;
    bool sessionStarted = false;
    Vector3 lastPos;
    float speed; 
    int frames = 0;
    float passedTime = 0;

    private void Start()
    {
        m_camera = Camera.main;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
    }

    void FirstFrameUpdate(UnityARCamera cam)
    {
        sessionStarted = true;
        lastPos = m_camera.transform.localPosition;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
    }

    void Update()
    {

        if (m_camera == null || !sessionStarted)
            return;

        passedTime += Time.deltaTime;
        frames++;
        if (frames % speedFrames != 0)
            return;
        Vector3 newPos = m_camera.transform.localPosition;
        float dist = Vector3.Distance(newPos, lastPos);
        speed = dist / passedTime;
        speedText.text = "Speed " + speed.ToString();
        warningText.text = speed > 1 ? "Slow down!" : "";
        posText.text = "Pos: " + newPos;
        frames = 0;
        passedTime = 0;
        lastPos = newPos;
        float curAcc = Input.acceleration.z;
        accText.text = "Acc: " + curAcc;
    }

}
