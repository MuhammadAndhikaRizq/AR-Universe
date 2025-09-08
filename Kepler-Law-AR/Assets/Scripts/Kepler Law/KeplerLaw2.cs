using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeplerLaw2 : MonoBehaviour
{
    public Transform sun; // drag Sun ke sini
    public float a = 5f; // semi-major axis
    public float b = 3f; // semi-minor axis
    public float orbitalPeriod = 10f; // durasi satu orbit (detik)
    public float sectorTimeSpan = 1f; // interval waktu tiap juring (misal 1 detik)
    public int maxSectors = 4; // maksimum juring yang ditampilkan (A,B,C,D)

    private List<Vector3> sectorPositions = new List<Vector3>();
    private List<GameObject> sectorMeshes = new List<GameObject>();
    private LineRenderer orbitRenderer;

    void Start()
    {
        DrawOrbit();
        StartCoroutine(Animate());
    }

    void DrawOrbit()
    {
        GameObject orbitObj = new GameObject("Orbit");
        orbitRenderer = orbitObj.AddComponent<LineRenderer>();
        orbitRenderer.positionCount = 360;
        orbitRenderer.startWidth = 0.05f;
        orbitRenderer.endWidth = 0.05f;
        orbitRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.gray };

        for (int i = 0; i < 360; i++)
        {
            float angle = i * Mathf.Deg2Rad;
            float x = a * Mathf.Cos(angle);
            float z = b * Mathf.Sin(angle);
            orbitRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    System.Collections.IEnumerator Animate()
    {
        float t = 0f;
        float lastRecordTime = Time.time;
        Vector3 lastPos = GetPosition(t);

        while (true)
        {
            t += Time.deltaTime / orbitalPeriod;
            if (t > 1f) t -= 1f;

            Vector3 newPos = GetPosition(t);
            transform.position = newPos;

            // Simpan titik tiap interval waktu
            if (Time.time - lastRecordTime >= sectorTimeSpan)
            {
                if (sectorPositions.Count == 0)
                {
                    // Titik awal (A)
                    sectorPositions.Add(newPos);
                }
                else
                {
                    // Titik berikutnya (B, C, D...)
                    sectorPositions.Add(newPos);
                    DrawSector(sectorPositions[sectorPositions.Count - 2], newPos);
                }

                // Batasi jumlah juring
                if (sectorPositions.Count > maxSectors)
                {
                    sectorPositions.RemoveAt(0); // hapus titik paling awal
                    if (sectorMeshes.Count > 0)
                    {
                        Destroy(sectorMeshes[0]);
                        sectorMeshes.RemoveAt(0);
                    }
                }

                UpdateLabels();
                lastRecordTime = Time.time;
            }

            yield return null;
        }
    }

    Vector3 GetPosition(float t)
    {
        float meanAnomaly = t * 2 * Mathf.PI;
        float e = Mathf.Sqrt(1 - (b * b) / (a * a)); // eksentrisitas
        float E = SolveKeplerEquation(meanAnomaly, e); // eccentric anomaly
        float trueAnomaly = 2 * Mathf.Atan2(
            Mathf.Sqrt(1 + e) * Mathf.Sin(E / 2),
            Mathf.Sqrt(1 - e) * Mathf.Cos(E / 2)
        );

        float r = a * (1 - e * e) / (1 + e * Mathf.Cos(trueAnomaly));
        float x = r * Mathf.Cos(trueAnomaly);
        float z = r * Mathf.Sin(trueAnomaly);
        return new Vector3(x, 0, z);
    }

    float SolveKeplerEquation(float M, float e, int maxIter = 10)
    {
        float E = M;
        for (int i = 0; i < maxIter; i++)
        {
            E = M + e * Mathf.Sin(E);
        }
        return E;
    }

    void DrawSector(Vector3 start, Vector3 end)
    {
        GameObject sectorObj = new GameObject("Sector");
        sectorMeshes.Add(sectorObj);

        MeshFilter mf = sectorObj.AddComponent<MeshFilter>();
        MeshRenderer mr = sectorObj.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"))
        {
            color = new Color(0.5f, 0.8f, 1f, 0.4f) // biru transparan
        };

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Titik: Matahari, Titik Awal, Titik Akhir
        vertices.Add(sun.position);
        vertices.Add(start);
        vertices.Add(end);

        triangles.Add(0); triangles.Add(1); triangles.Add(2);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        mf.mesh = mesh;
    }

    void UpdateLabels()
    {
        // Hapus semua label lama
        GameObject[] oldLabels = GameObject.FindGameObjectsWithTag("KeplerLabel");
        foreach (GameObject label in oldLabels)
        {
            Destroy(label);
        }

        // Tambahkan label A, B, C, D di atas titik
        for (int i = 0; i < sectorPositions.Count; i++)
        {
            Vector3 pos = sectorPositions[i];
            GameObject label = new GameObject($"Label_{(char)(65 + i)}");
            label.tag = "KeplerLabel"; // Untuk mudah dihapus nanti
            label.transform.position = pos + Vector3.up * 0.8f; // di atas planet

            // Gunakan TextMeshPro
            TextMeshPro text = label.AddComponent<TextMeshPro>();
            text.text = ((char)(65 + i)).ToString(); // A, B, C, D
            text.fontSize = 2.0f;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.Center;
            text.fontStyle = FontStyles.Bold;
        }
    }
}
