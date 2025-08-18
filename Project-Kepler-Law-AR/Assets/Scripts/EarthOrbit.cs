using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthOrbit : MonoBehaviour
{
    public Transform sun;       // Matahari
    public float orbitSpeed = 10f; // Kecepatan orbit

    void Update()
    {
        if (sun != null)
        {
            // Bumi mengorbit matahari
            transform.RotateAround(sun.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
    }
}
