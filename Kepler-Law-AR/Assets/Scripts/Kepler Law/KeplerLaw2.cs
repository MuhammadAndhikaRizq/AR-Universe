using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeplerLaw2 : MonoBehaviour
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

    [Header("Area Wedge (Juring)")]
    public LineRenderer juringLine;         // Untuk gambar segitiga juring
    public float deltaTimeArea = 1.0f;      // Interval waktu gambar juring (detik)

    [Header("Line Appearance")]
    public float baseOrbitWidth = 0.5f;     // Lebar normal saat scale = (1,1,1)
    public float baseJuringWidth = 0.5f;
    public float minWidth = 0.1f;
    public float maxWidth = 2f;

    private float meanAnomaly = 0f;
    private float timer = 5f;
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

        // Setup juring
        if (juringLine != null)
        {
            juringLine.positionCount = 3;
            juringLine.useWorldSpace = true;
            juringLine.loop = false;

            if (juringLine.material == null)
            {
                juringLine.material = new Material(Shader.Find("Unlit/Color"));
                juringLine.startColor = new Color(0, 1, 1, 0.5f); // Cyan transparan
                juringLine.endColor = new Color(0, 1, 1, 0.5f);
            }

            // Set width awal
            juringLine.startWidth = baseJuringWidth;
            juringLine.endWidth = baseJuringWidth;
            juringLine.enabled = false;
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

        // 5. Update juring
        timer += Time.deltaTime;
        if (timer >= deltaTimeArea)
        {
            if (juringLine != null)
            {
                juringLine.SetPosition(0, sun.position);
                juringLine.SetPosition(1, lastPlanetPos);
                juringLine.SetPosition(2, planetPos);
                juringLine.enabled = true;
            }
            lastPlanetPos = planetPos;
            timer = 0f;
        }

        UpdateLineWidths();
    }

    void LateUpdate()
    {
        DrawOrbitPath(); 
    }

   
    void UpdateLineWidths()
    {
        float currentScaleFactor = GetCurrentScaleFactor();

        if (orbitLine != null)
        {
            float targetWidth = baseOrbitWidth * currentScaleFactor;
            orbitLine.startWidth = Mathf.Clamp(targetWidth, minWidth, maxWidth);
            orbitLine.endWidth = orbitLine.startWidth;
        }

        if (juringLine != null)
        {
            float targetWidth = baseJuringWidth * currentScaleFactor;
            juringLine.startWidth = Mathf.Clamp(targetWidth, minWidth, maxWidth);
            juringLine.endWidth = juringLine.startWidth;
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
