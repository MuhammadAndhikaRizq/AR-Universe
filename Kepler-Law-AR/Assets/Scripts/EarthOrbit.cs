using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform sun;                
    public float orbitSpeed = 1f;
    public float a = 0.8f;    // Sumbu semi-mayor (meter)
    public float b = 0.6f;    // Sumbu semi-minor

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    private float angle = 0f;
    public int segments = 200;

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer tidak diassign!");
            return;
        }

        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true; // Penting: posisi dunia

        // Material
        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
        }

        DrawEllipse();
    }

    void Update()
    {
        // Update orbit Earth
        angle += orbitSpeed * Time.deltaTime;

        Vector3 earthPosition = CalculateOrbitPosition(angle);
        transform.position = earthPosition;
    }

    // Fungsi terpusat untuk menghitung posisi orbit
    Vector3 CalculateOrbitPosition(float currentAngle)
    {
        float c = Mathf.Sqrt(Mathf.Abs(a * a - b * b));
        bool isMajorHorizontal = a >= b;

        // Fokus elips: kita ingin Matahari di salah satu fokus
        // Misal: fokus kiri (untuk elips horizontal) atau bawah (vertikal)
        // Kita asumsikan elips horizontal jika a >= b
        Vector3 focusOffset;

        if (isMajorHorizontal)
        {
            focusOffset = new Vector3(c, 0, 0); // Fokus di kanan dan kiri
        }
        else
        {
            focusOffset = new Vector3(0, 0, c); // Jika vertikal, fokus atas-bawah
        }

        // Pusat elips: agar Matahari (sun.position) berada di fokus kiri (atau bawah)
        Vector3 ellipseCenter = sun.position - focusOffset;

        // Hitung posisi di elips
        float x = a * Mathf.Cos(currentAngle);
        float z = b * Mathf.Sin(currentAngle);

        if (!isMajorHorizontal)
        {
            // Kalau elips vertikal, tukar peran
            x = b * Mathf.Cos(currentAngle);
            z = a * Mathf.Sin(currentAngle);
        }

        return ellipseCenter + new Vector3(x, 0, z);
    }

    void DrawEllipse()
    {
        if (lineRenderer == null) return;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * 2 * Mathf.PI;
            Vector3 pos = CalculateOrbitPosition(theta);
            lineRenderer.SetPosition(i, pos);
        }
    }

    // Optional: Update jalur jika sun bergerak
    void LateUpdate()
    {
        DrawEllipse(); // Update jalur tiap frame jika Matahari bergerak
    }
}
