using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthTransit : MonoBehaviour
{
    public Transform sun; 
    public float moveSpeed = 5f; 
    public float distance = 20f; 

    private float startX;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        float newX = Mathf.PingPong(Time.time * moveSpeed, distance * 2) - distance + startX;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
