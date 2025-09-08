using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeplerLaw2 : MonoBehaviour
{
    public Camera mainCam;
    [Header("Zoom Settings")]
    public float zoomStep = 5f;       // besar perubahan tiap klik
    public float minOrthoSize = 5f;   // batas zoom untuk ortho
    public float maxOrthoSize = 50f;
    public float minFOV = 15f;        // batas zoom untuk perspektif
    public float maxFOV = 90f;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    public void ZoomIn()
    {
        if (mainCam.orthographic)
        {
            mainCam.orthographicSize = Mathf.Max(minOrthoSize, mainCam.orthographicSize - zoomStep);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Max(minFOV, mainCam.fieldOfView - zoomStep);
        }
    }

    public void ZoomOut()
    {
        if (mainCam.orthographic)
        {
            mainCam.orthographicSize = Mathf.Min(maxOrthoSize, mainCam.orthographicSize + zoomStep);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Min(maxFOV, mainCam.fieldOfView + zoomStep);
        }
    }
}
