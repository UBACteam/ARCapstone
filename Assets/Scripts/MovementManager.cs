using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class MovementManager : MonoBehaviour
{

    public Text speedText;
    public Text warningText;
    public Text posText;
    public Text accText;
    public int speedFrames = 10;

    public Camera m_camera;

    private bool sessionStarted = false;

    private Vector3 lastPos;
    private float speed;
    private int frames = 0;
    private float passedTime = 0;


    void FirstFrameUpdate(UnityARCamera cam)
    {
        sessionStarted = true;
        lastPos = m_camera.transform.localPosition;
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