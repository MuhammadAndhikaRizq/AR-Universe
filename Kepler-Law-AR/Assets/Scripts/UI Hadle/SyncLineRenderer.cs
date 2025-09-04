using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncLineRenderer : MonoBehaviour
{
    [Header("Line References")]
    public LineRenderer orbitLine;           // Drag OrbitPath's LineRenderer
    public LineRenderer juringLine;          // Drag JuringRenderer's LineRenderer

    [Header("Width Settings")]
    public float baseOrbitWidth = 0.015f;    // Lebar normal saat scale = 1
    public float baseJuringWidth = 0.025f;
    public float minWidth = 0.005f;
    public float maxWidth = 0.15f;

    private Vector3 originalLocalScale;

    void Start()
    {
        // Simpan skala awal
        originalLocalScale = transform.localScale;

        // Validasi
        if (orbitLine == null) Debug.LogWarning("OrbitLine tidak diassign!", this);
        if (juringLine == null) Debug.LogWarning("JuringLine tidak diassign!", this);

        // Set lebar awal
        UpdateLineWidths();
    }

    void Update()
    {
        // Update tiap frame agar selalu sinkron dengan perubahan skala
        UpdateLineWidths();
    }

    void UpdateLineWidths()
    {
        // Hitung faktor skala saat ini (relatif terhadap awal)
        float currentScaleFactor = GetCurrentScaleFactor();

        // Update Orbit Line
        if (orbitLine != null)
        {
            float targetWidth = baseOrbitWidth * currentScaleFactor;
            orbitLine.startWidth = Mathf.Clamp(targetWidth, minWidth, maxWidth);
            orbitLine.endWidth = orbitLine.startWidth;
        }

        // Update Juring Line
        if (juringLine != null)
        {
            float targetWidth = baseJuringWidth * currentScaleFactor;
            juringLine.startWidth = Mathf.Clamp(targetWidth, minWidth, maxWidth);
            juringLine.endWidth = juringLine.startWidth;
        }
    }

    float GetCurrentScaleFactor()
    {
        // Asumsi skala uniform (x=y=z)
        return transform.localScale.x / originalLocalScale.x;
    }

    // Opsional: panggil manual (jika pakai event)
    public void ForceUpdate() => UpdateLineWidths();
}
