using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EarthEclipse : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform sun;        // Matahari
    public float orbitSpeed = 1f;
    public float a = 15f;        // Semi-major axis (panjang orbit)
    public float b = 10f;        // Semi-minor axis (lebar orbit)
    public float minDistanceFromSun = 3f; // jarak minimal orbit ke Matahari

    private float angle = 0f;
    private Vector3 orbitCenter;

    private LineRenderer lineRenderer;
    public int segments = 200;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;

        // Hitung fokus elips
        float c = Mathf.Sqrt(a * a - b * b);

        // Geser orbit supaya Matahari tetap dekat fokus tapi tidak menempel
        orbitCenter = new Vector3(-c + minDistanceFromSun, 0, 0);

        DrawEllipse();
    }

    void Update()
    {
        angle += orbitSpeed * Time.deltaTime;

        float x = Mathf.Cos(angle) * a;
        float z = Mathf.Sin(angle) * b;

        // Posisi bumi mengikuti elips yang digeser
        transform.position = sun.position + orbitCenter + new Vector3(x, 0, z);
    }

    void DrawEllipse()
    {
        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * 2 * Mathf.PI;
            float x = Mathf.Cos(theta) * a;
            float z = Mathf.Sin(theta) * b;

            lineRenderer.SetPosition(i, sun.position + orbitCenter + new Vector3(x, 0, z));
        }
    }
}
