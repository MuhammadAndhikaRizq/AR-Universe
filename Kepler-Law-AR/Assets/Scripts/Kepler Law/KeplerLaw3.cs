using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeplerLaw3 : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform sun;                    // Matahari (fokus elips)
    public float semiMajorAxis = 100f;      // a (sumbu semi-mayor)
    public float eccentricity = 0.5f;       // e (0 = lingkaran, 0.3 = elips sedang)
    public float orbitPeriod = 20f;         // Waktu untuk 1 putaran (detik)

    [Header("Visualization")]
    public LineRenderer orbitLine;          // Garis orbit (path)
    public int segments = 200;              // Jumlah titik di orbit

    [Header("Planet Rotation")]
    public bool rotate = true;              // Apakah planet berotasi?
    public float rotationSpeed = 10f;       // Kecepatan rotasi (derajat/detik)
    public Vector3 rotationAxis = Vector3.up;


    [Header("Line Appearance")]
    public float baseOrbitWidth = 0.5f;     // Lebar normal saat scale = (1,1,1)
    public float minWidth = 0.1f;
    public float maxWidth = 2f;

    private float meanAnomaly = 0f;
    private Vector3 lastPlanetPos;
    private Vector3 originalLocalScale;     // Skala awal

    void Start()
    {
        if (sun == null)
        {
            Debug.LogError("Sun tidak diassign!", this);
            enabled = false;
            return;
        }

        // Simpan skala awal
        originalLocalScale = transform.localScale;

        // Setup orbit path
        if (orbitLine != null)
        {
            orbitLine.positionCount = segments + 1;
            orbitLine.useWorldSpace = true;
            orbitLine.loop = true;

            if (orbitLine.material == null)
            {
                orbitLine.material = new Material(Shader.Find("Unlit/Color"));
                orbitLine.startColor = Color.yellow;
                orbitLine.endColor = Color.yellow;
            }

            // Set width awal
            orbitLine.startWidth = baseOrbitWidth;
            orbitLine.endWidth = baseOrbitWidth;
            
            DrawOrbitPath();
        }

        // Posisi awal
        lastPlanetPos = CalculatePosition(0f);
        transform.position = lastPlanetPos;

        // Update width sekali
        UpdateLineWidths();
    }

    void Update()
    {
        // 1. Mean anomaly bertambah linear
        float meanMotion = (2 * Mathf.PI) / orbitPeriod;
        meanAnomaly += meanMotion * Time.deltaTime;
        meanAnomaly = Wrap(meanAnomaly, 2 * Mathf.PI);

        // 2. Hitung True Anomaly
        float trueAnomaly = SolveTrueAnomaly(meanAnomaly, eccentricity);

        // 3. Hitung posisi planet
        Vector3 planetPos = CalculatePosition(trueAnomaly);
        transform.position = planetPos;

        // 4. Rotasi planet
        if (rotate)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        UpdateLineWidths();
    }

    void LateUpdate()
    {
        DrawOrbitPath(); 
    }


    void UpdateLineWidths()
    {
        if (orbitLine == null) return;

        float scaleFactor = GetCameraScaleFactor();

        float targetWidth = baseOrbitWidth * scaleFactor;
        orbitLine.startWidth = Mathf.Clamp(targetWidth, minWidth, maxWidth);
        orbitLine.endWidth = orbitLine.startWidth;
    }

    float GetCameraScaleFactor()
    {
         Camera cam = Camera.main;

    if (cam.orthographic) // kalau pakai kamera ortho
    {
        // kecilkan garis kalau camera di-zoom out (orthographicSize besar)
        return 1f / cam.orthographicSize;
        }
        else // kamera perspektif
        {
            // bisa juga pakai jarak ke planet
            float distance = Vector3.Distance(cam.transform.position, transform.position);
            return 1f / distance; 
            // atau return cam.fieldOfView * 0.01f;  (opsi lain)
        }
    }

    float GetCurrentScaleFactor()
    {
        // Asumsi skala uniform (X=Y=Z)
        return transform.localScale.x / originalLocalScale.x;
    }

    // ... (fungsi SolveTrueAnomaly, CalculatePosition, DrawOrbitPath, Wrap tetap sama)
    float SolveTrueAnomaly(float M, float e)
    {
        float E = M;
        for (int i = 0; i < 10; i++)
        {
            float delta = (E - e * Mathf.Sin(E) - M) / (1 - e * Mathf.Cos(E));
            E -= delta;
            if (Mathf.Abs(delta) < 1e-6f) break;
        }

        float sinE = Mathf.Sin(E);
        float cosE = Mathf.Cos(E);
        float denominator = 1 - e * cosE;

        float sinTheta = (Mathf.Sqrt(1 - e * e) * sinE) / denominator;
        float cosTheta = (cosE - e) / denominator;

        return Mathf.Atan2(sinTheta, cosTheta);
    }

    Vector3 CalculatePosition(float theta)
    {
        float r = (semiMajorAxis * (1 - eccentricity * eccentricity)) /
                  (1 + eccentricity * Mathf.Cos(theta));

        Vector3 offset = new Vector3(
            r * Mathf.Cos(theta),
            0,
            r * Mathf.Sin(theta)
        );

        float c = semiMajorAxis * eccentricity;
        Vector3 ellipseCenter = sun.position - new Vector3(c, 0, 0);

        return ellipseCenter + offset;
    }

    void DrawOrbitPath()
    {
        if (orbitLine == null) return;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * 2 * Mathf.PI;
            Vector3 pos = CalculatePosition(theta);
            orbitLine.SetPosition(i, pos);
        }
    }

    float Wrap(float angle, float max)
    {
        while (angle < 0) angle += max;
        while (angle >= max) angle -= max;
        return angle;
    }
}


