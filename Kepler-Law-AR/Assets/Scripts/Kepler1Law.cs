using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kepler1Law : MonoBehaviour
{
    [Header("References")]
    public Transform sun;        // Matahari

    [Header("Orbit Settings")]
    public float orbitSpeed = 50f;  // derajat per detik
    public float a = 0.15f;         // semi-major axis (15 cm)
    public float b = 0.10f;         // semi-minor axis (10 cm)
    public float minDistanceFromSun = 0.03f; // jarak aman dari Matahari

    private float angle = 0f;
    private Vector3 orbitCenter;

    private LineRenderer lineRenderer;
    public int segments = 100;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;
        lineRenderer.widthMultiplier = 0.005f; // ketebalan garis orbit (5 mm)

        // Hitung fokus elips
        float c = Mathf.Sqrt(a * a - b * b);
        orbitCenter = new Vector3(-c + minDistanceFromSun, 0, 0);

        DrawEllipse();
    }

    void Update()
    {
        // Gerak revolusi (mengelilingi Matahari)
        angle += orbitSpeed * Time.deltaTime * Mathf.Deg2Rad;

        float x = Mathf.Cos(angle) * a;
        float z = Mathf.Sin(angle) * b;

        transform.position = sun.position + orbitCenter + new Vector3(x, 0, z);

        // Gerak rotasi (berputar pada sumbu)
        transform.Rotate(Vector3.up, 360f * Time.deltaTime, Space.Self);
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
