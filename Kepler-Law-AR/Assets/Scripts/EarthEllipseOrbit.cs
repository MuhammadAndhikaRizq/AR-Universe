using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthEllipseOrbit : MonoBehaviour
{
    public Transform sun;
    public float orbitSpeed = 1f;
    public float a = 15f; // sumbu mayor (jarak x)
    public float b = 10f; // sumbu minor (jarak z)

    private float angle = 0f;

    void Update()
    {
        if (sun == null) return;

        angle += orbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(angle) * a;
        float z = Mathf.Sin(angle) * b;
        
        transform.position = sun.position + new Vector3(x, 0, z);
    }
}
