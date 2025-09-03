using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Transform solarSystemRoot;        // Root objek tata surya
    public float minScale = 0.5f;            // Zoom out (kecil)
    public float maxScale = 5f;              // Zoom in (besar)
    public float scrollSensitivity = 0.5f;   // Sensitivitas

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f && solarSystemRoot != null)
        {
            float newScale = solarSystemRoot.localScale.x + scroll * scrollSensitivity;
            newScale = Mathf.Clamp(newScale, minScale, maxScale);

            solarSystemRoot.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}
