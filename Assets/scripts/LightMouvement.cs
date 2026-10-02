using UnityEngine;

public class LightMouvement : MonoBehaviour
{
    public float sweepAngle = 30f;  
    public float speed = 1.5f;
    public float baseZ = 180f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float z = baseZ + Mathf.Sin(Time.time * speed) * sweepAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, z);

    }
}
