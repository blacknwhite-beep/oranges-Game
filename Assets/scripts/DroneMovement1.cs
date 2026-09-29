using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneMovement1 : MonoBehaviour
{
    [SerializeField] private float amplitude = 1f;  // how far up/down
    [SerializeField] private float frequency = 1f;  // how fast

    private Vector3 startPos;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector2(transform.position.x, newY);
    }

}

