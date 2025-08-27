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

    private float meanAnomaly = 0f;         // M: sudut rata-rata (bertambah konstan)
    private float timer = 0f;
    private Vector3 lastPlanetPos;

    void Start()
    {
        if (sun == null)
        {
            Debug.LogError("Sun tidak diassign!", this);
            enabled = false;
            return;
        }

        // Setup orbit path
        if (orbitLine != null)
        {
            orbitLine.positionCount = segments + 1;
            orbitLine.useWorldSpace = true;
            orbitLine.loop = true;
            ConfigureOrbitLine();
            DrawOrbitPath();
        }

        // Setup juring
        if (juringLine != null)
        {
            juringLine.positionCount = 3;
            juringLine.useWorldSpace = true;
            juringLine.loop = false;
            juringLine.material = new Material(Shader.Find("Unlit/Color"));
            juringLine.startColor = new Color(0, 1, 1, 0.5f); // Cyan transparan
            juringLine.endColor = new Color(0, 1, 1, 0.5f);
            juringLine.startWidth = 0.2f;
            juringLine.endWidth = 0.2f;
            juringLine.enabled = false;
        }

        // Posisi awal
        lastPlanetPos = CalculatePosition(0f);
        transform.position = lastPlanetPos;
    }

    void ConfigureOrbitLine()
    {
        orbitLine.material = new Material(Shader.Find("Unlit/Color"));
        orbitLine.startColor = Color.yellow;
        orbitLine.endColor = Color.yellow;
        orbitLine.startWidth = 0.1f;
        orbitLine.endWidth = 0.1f;
    }

    void Update()
    {
        // 1. Mean anomaly bertambah linear (waktu nyata)
        float meanMotion = (2 * Mathf.PI) / orbitPeriod; // n = 2π/T
        meanAnomaly += meanMotion * Time.deltaTime;
        meanAnomaly = Wrap(meanAnomaly, 2 * Mathf.PI);

        // 2. Hitung True Anomaly dari Mean Anomaly
        float trueAnomaly = SolveTrueAnomaly(meanAnomaly, eccentricity);

        // 3. Hitung posisi planet
        Vector3 planetPos = CalculatePosition(trueAnomaly);
        transform.position = planetPos;

        // 4. Rotasi planet pada porosnya
        if (rotate)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        // 5. Update juring (luas yang disapu)
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
    }

    void LateUpdate()
    {
        DrawOrbitPath(); // Update jalur jika Matahari bergerak
    }

    // Hitung True Anomaly dari Mean Anomaly (via Eccentric Anomaly)
    float SolveTrueAnomaly(float M, float e)
    {
        // Newton-Raphson: selesaikan M = E - e*sin(E)
        float E = M;
        for (int i = 0; i < 10; i++)
        {
            float delta = (E - e * Mathf.Sin(E) - M) / (1 - e * Mathf.Cos(E));
            E -= delta;
            if (Mathf.Abs(delta) < 1e-6f) break;
        }

        // Hitung True Anomaly dari E
        float sinE = Mathf.Sin(E);
        float cosE = Mathf.Cos(E);
        float denominator = 1 - e * cosE;

        float sinTheta = (Mathf.Sqrt(1 - e * e) * sinE) / denominator;
        float cosTheta = (cosE - e) / denominator;

        return Mathf.Atan2(sinTheta, cosTheta);
    }

    // Hitung posisi dari True Anomaly
    Vector3 CalculatePosition(float theta)
    {
        // Jarak dari Matahari: r = a(1-e²)/(1+e*cos(θ))
        float r = (semiMajorAxis * (1 - eccentricity * eccentricity)) /
                  (1 + eccentricity * Mathf.Cos(theta));

        // Koordinat relatif terhadap Matahari
        Vector3 offset = new Vector3(
            r * Mathf.Cos(theta),
            0,
            r * Mathf.Sin(theta)
        );

        // Pusat elips: fokus di Matahari (kiri)
        float c = semiMajorAxis * eccentricity;
        Vector3 ellipseCenter = sun.position - new Vector3(c, 0, 0);

        return ellipseCenter + offset;
    }

    // Gambar jalur elips
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

    // Wrap angle ke [0, max)
    float Wrap(float angle, float max)
    {
        while (angle < 0) angle += max;
        while (angle >= max) angle -= max;
        return angle;
    }
}
