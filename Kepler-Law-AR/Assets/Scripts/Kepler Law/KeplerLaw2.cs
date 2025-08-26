using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeplerLaw2 : MonoBehaviour
{
    [Header("Target & Focus")]
    public Transform sun;                    // Fokus elips (Matahari)

    [Header("Orbital Elements")]
    public float semiMajorAxis = 100f;       // a (meter)
    public float eccentricity = 0.3f;        // e (0 = lingkaran, 0.9 = sangat lonjong)

    [Header("Motion Settings")]
    public float orbitPeriod = 20f;          // Waktu untuk 1 putaran penuh (detik)
    public bool isClockwise = false;         // Arah orbit

    [Header("Debug")]
    public bool drawDebugLines = true;       // Tampilkan garis ke Matahari

    private float meanAnomaly = 0f;          // M (sudut rata-rata)
    private float currentTrueAnomaly = 0f;   // θ (sudut aktual dari Matahari)
    private Vector3 initialPosition;

    void Start()
    {
        if (sun == null)
        {
            Debug.LogError("Sun tidak diassign!", this);
            enabled = false;
            return;
        }

        // Hitung kecepatan rata-rata (dalam radian/detik)
        orbitPeriod = Mathf.Max(orbitPeriod, 0.1f); // Hindari nol
        meanAnomaly = 0f;

        // Set posisi awal
        initialPosition = CalculatePosition(currentTrueAnomaly);
        transform.position = initialPosition;
    }

    void Update()
    {
        // 1. Mean anomaly bertambah linear terhadap waktu
        float meanMotion = 2 * Mathf.PI / orbitPeriod; // n = 2π/T
        meanAnomaly += meanMotion * Time.deltaTime;
        meanAnomaly = WrapAngle(meanAnomaly, 2 * Mathf.PI);

        // 2. Hitung True Anomaly dari Mean Anomaly
        currentTrueAnomaly = SolveTrueAnomaly(meanAnomaly, eccentricity);

        // 3. Hitung posisi nyata
        Vector3 newPosition = CalculatePosition(currentTrueAnomaly);
        transform.position = newPosition;
    }

    void OnDrawGizmos()
    {
        if (drawDebugLines && sun != null && enabled)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(sun.position, transform.position);
        }
    }

    // Hitung True Anomaly dari Mean Anomaly (via Eccentric Anomaly)
    float SolveTrueAnomaly(float M, float e)
    {
        // Langkah 1: Cari Eccentric Anomaly (E) dari M = E - e*sin(E)
        float E = M;
        for (int i = 0; i < 10; i++)
        {
            float delta = (E - e * Mathf.Sin(E) - M) / (1 - e * Mathf.Cos(E));
            E -= delta;
            if (Mathf.Abs(delta) < 1e-6f) break;
        }

        // Langkah 2: Hitung True Anomaly dari E
        float sinE = Mathf.Sin(E);
        float cosE = Mathf.Cos(E);

        float numerator = Mathf.Sqrt(1 - e * e) * sinE;
        float denominator = 1 - e * cosE;

        return Mathf.Atan2(numerator, denominator);
    }

    // Hitung posisi dari True Anomaly (θ)
    Vector3 CalculatePosition(float theta)
    {
        // Tanda tergantung arah
        if (isClockwise) theta = -theta;

        // Jarak dari Matahari ke planet
        float r = (semiMajorAxis * (1 - eccentricity * eccentricity)) / (1 + eccentricity * Mathf.Cos(theta));

        // Offset relatif terhadap Matahari
        Vector3 offset = new Vector3(
            r * Mathf.Cos(theta),
            0,
            r * Mathf.Sin(theta)
        );

        // Fokus elips: Matahari di salah satu fokus
        float c = semiMajorAxis * eccentricity;
        Vector3 ellipseCenter = sun.position - new Vector3(c, 0, 0);

        return ellipseCenter + offset;
    }

    // Wrap angle ke [0, max)
    float WrapAngle(float angle, float max)
    {
        while (angle < 0) angle += max;
        while (angle >= max) angle -= max;
        return angle;
    }
}
