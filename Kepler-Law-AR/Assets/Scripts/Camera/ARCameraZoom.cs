using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraZoom : MonoBehaviour
{
     public Camera mainCam;
    public float zoomStep = 2f;    // besar perubahan tiap klik
    public float minZoom = 5f;
    public float maxZoom = 50f;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    public void ZoomIn()
    {
        Debug.Log("On clicked");
        if (mainCam.orthographic)
        {
            mainCam.orthographicSize = Mathf.Max(minZoom, mainCam.orthographicSize - zoomStep);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Max(minZoom, mainCam.fieldOfView - zoomStep);
        }
    }

    public void ZoomOut()
    {
        if (mainCam.orthographic)
        {
            mainCam.orthographicSize = Mathf.Min(maxZoom, mainCam.orthographicSize + zoomStep);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Min(maxZoom, mainCam.fieldOfView + zoomStep);
        }
    }
}
