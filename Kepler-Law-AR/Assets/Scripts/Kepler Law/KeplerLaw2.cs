using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeplerLaw2 : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform sun;
    public float orbitSpeed = 1f;
    public float a = 0.8f;    // Sumbu semi-mayor
    public float b = 0.6f;    // Sumbu semi-minor

    [Header("Orbit Line")]
    public LineRenderer orbitLine;
    public int segments = 200;

    [Header("Planet Rotation")]
    public bool rotate = true;
    public float rotationSpeed = 10f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Juring Settings")]
    public LineRenderer juringLine;         // untuk gambar sektor
    public float deltaTimeArea = 1.0f;      // interval waktu (detik)

    private float angle = 0f;
    private float timer = 0f;
    private Vector3 lastPlanetPos;

    void Start()
    {
        // Orbit path
        if (orbitLine != null)
        {
            orbitLine.positionCount = segments + 1;
            orbitLine.useWorldSpace = true;
            DrawEllipse();
        }

        // Juring line setup
        if (juringLine != null)
        {
            juringLine.positionCount = 3; // segitiga: sun, start, end
            juringLine.loop = true;
            juringLine.material = new Material(Shader.Find("Unlit/Color"));
            juringLine.startColor = Color.cyan;
            juringLine.endColor = Color.cyan;
        }

        lastPlanetPos = CalculateOrbitPosition(angle);
    }

    void Update()
    {
        // Posisi sekarang
        Vector3 currentPos = CalculateOrbitPosition(angle);

        // Jarak ke Matahari
        float r = Vector3.Distance(currentPos, sun.position);

        // Hukum Kepler II: makin dekat makin cepat
        float dynamicSpeed = orbitSpeed / r;
        angle += dynamicSpeed * Time.deltaTime;

        // Update posisi planet
        Vector3 planetPos = CalculateOrbitPosition(angle);
        transform.position = planetPos;

        // Rotasi planet
        if (rotate)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        // Update juring setiap deltaTimeArea
        timer += Time.deltaTime;
        if (timer >= deltaTimeArea)
        {
            if (juringLine != null)
            {
                juringLine.SetPosition(0, sun.position);
                juringLine.SetPosition(1, lastPlanetPos);
                juringLine.SetPosition(2, planetPos);
            }
            lastPlanetPos = planetPos;
            timer = 0f;
        }
    }

    Vector3 CalculateOrbitPosition(float currentAngle)
    {
        float c = Mathf.Sqrt(Mathf.Abs(a * a - b * b));
        bool isMajorHorizontal = a >= b;

        Vector3 focusOffset;
        if (isMajorHorizontal)
        {
            focusOffset = new Vector3(c, 0, 0);
        }
        else
        {
            focusOffset = new Vector3(0, 0, c);
        }

        Vector3 ellipseCenter = sun.position - focusOffset;

        float x = a * Mathf.Cos(currentAngle);
        float z = b * Mathf.Sin(currentAngle);

        if (!isMajorHorizontal)
        {
            x = b * Mathf.Cos(currentAngle);
            z = a * Mathf.Sin(currentAngle);
        }

        return ellipseCenter + new Vector3(x, 0, z);
    }

    void DrawEllipse()
    {
        if (orbitLine == null) return;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * 2 * Mathf.PI;
            Vector3 pos = CalculateOrbitPosition(theta);
            orbitLine.SetPosition(i, pos);
        }
    }
    
    void LateUpdate()
    {
        DrawEllipse(); // Update jalur tiap frame jika Matahari bergerak
    }
}
