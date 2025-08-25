using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kepler1Law : MonoBehaviour
{
    [Header("Referensi")]
    public Transform sun;      // Matahari di fokus (letakkan di (0,0,0) relatif anchor)
    public Transform earth;    // Planet yang bergerak

    [Header("Parameter Elips (Unity units)")]
    [Min(0.01f)] public float a = 0.6f;  // semi-major axis
    [Min(0.01f)] public float b = 0.4f;  // semi-minor axis

    [Header("Animasi")]
    [Tooltip("Kecepatan sudut orbit (rad/detik)")]
    public float angularSpeed = 0.6f;
    [Tooltip("Resolusi garis elips penuh")]
    [Range(32, 2048)] public int segments = 360;

    [Header("Garis")]
    public float lineWidth = 0.005f;
    public bool useXZPlane = true; // true: elips di bidang XZ (dilihat dari atas)

    LineRenderer lr;
    float c;                 // jarak fokus
    float theta;             // sudut berjalan
    Vector3[] fullEllipse;   // cache elips penuh
    int revealedPoints = 0;  // titik yang sudah dimunculkan

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;                // penting di AR
        lr.loop = false;                        // kita gambar bertahap
        lr.widthCurve = AnimationCurve.Constant(0, 1, lineWidth);
        lr.positionCount = 0;
    }

    void Start()
    {
        // Hitung fokus
        float a2 = a * a;
        float b2 = b * b;
        c = Mathf.Sqrt(Mathf.Max(0f, a2 - b2));  // c = sqrt(a^2 - b^2)

        // Opsional: jaga jarak aman dari Matahari (supaya tidak menembus)
        // Misal radius matahari ~ 0.05 unit → perihelion harus > 0.05
        float sunRadius = 0.05f;
        float perihelion = a - c;
        if (perihelion <= sunRadius)
        {
            float delta = (sunRadius + 0.02f) - perihelion; // padding 0.02
            a += delta; // geser sedikit supaya aman
            a2 = a * a;
            c = Mathf.Sqrt(Mathf.Max(0f, a2 - b2));
        }

        // Precompute elips penuh buat nanti kalau mau ditutup
        fullEllipse = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = (i / (float)segments) * Mathf.PI * 2f;
            fullEllipse[i] = ParametricPoint(t);
        }

        // Set posisi awal Bumi dan siapkan LineRenderer
        theta = 0f;
        earth.position = ParametricPoint(theta);
        revealedPoints = 1;
        lr.positionCount = 1;
        lr.SetPosition(0, earth.position);
    }

    void Update()
    {
        // Gerakkan Bumi
        theta += angularSpeed * Time.deltaTime;
        if (theta > Mathf.PI * 2f) theta -= Mathf.PI * 2f;

        Vector3 p = ParametricPoint(theta);
        earth.position = p;

        // Tambahkan titik ke LineRenderer agar "jejak" muncul di belakang Bumi
        // Hitung index segment saat ini
        int idx = Mathf.FloorToInt((theta / (Mathf.PI * 2f)) * segments);
        idx = Mathf.Clamp(idx, 0, segments);

        // Pastikan jumlah titik yang diungkap minimal idx+1
        if (idx + 1 > revealedPoints)
        {
            // Tambah titik baru dari fullEllipse
            int add = (idx + 1) - revealedPoints;
            int newCount = revealedPoints + add;
            Vector3[] current = new Vector3[newCount];
            // copy lama
            for (int i = 0; i < revealedPoints; i++)
                current[i] = lr.GetPosition(i);
            // isi tambahan (sinkron dengan fullEllipse)
            for (int i = revealedPoints; i < newCount; i++)
                current[i] = fullEllipse[i - 1 < 0 ? 0 : i - 1]; // selaras indeks

            lr.positionCount = newCount;
            for (int i = 0; i < newCount; i++)
                lr.SetPosition(i, current[i]);

            revealedPoints = newCount;
        }
        else
        {
            // Update titik terakhir agar selalu tepat di posisi Bumi (opsional)
            if (revealedPoints > 0)
                lr.SetPosition(revealedPoints - 1, p);
        }
    }

    Vector3 ParametricPoint(float t)
    {
        // Elips dengan pusat (c,0,0) agar fokus di (0,0,0) untuk Matahari
        float x = a * Mathf.Cos(t);
        float z = b * Mathf.Sin(t);
        if (useXZPlane)
            return transform.TransformPoint(new Vector3(x, 0f, z));
        else
            return transform.TransformPoint(new Vector3(x, z, 0f));
    }

    // (Opsional) panggil dari UI button untuk langsung menutup elips.
    public void RevealFullEllipse()
    {
        lr.positionCount = fullEllipse.Length;
        for (int i = 0; i < fullEllipse.Length; i++)
            lr.SetPosition(i, fullEllipse[i]);
        revealedPoints = fullEllipse.Length;
    }
}
