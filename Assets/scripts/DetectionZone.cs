using System;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform Target { get; private set; }
    public bool PlayerDetected { get; private set; }

    [SerializeField]
    private string Detection = "Player";

    private int collidersInside;


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag(Detection)) return;

        collidersInside++;
        PlayerDetected = true;
        Target = other.transform;

        Debug.Log($"Trigger entered by {other.name}, tag={other.tag}, layer={LayerMask.LayerToName(other.gameObject.layer)}");


    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (!other.CompareTag(Detection)) return;

        collidersInside--;
        if (collidersInside <= 0)
        {
            collidersInside = 0;
            PlayerDetected = false;
            Target = null;
        }

        

    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
