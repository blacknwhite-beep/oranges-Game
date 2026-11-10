using UnityEngine;

public class Rotating_Platform_Script : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // Speed of rotation in degrees per second
    private float timer = 0f; // Timer to track the rotation duration

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotatePlatform();
    }

    public void rotatePlatform()
    {
        timer = timer + Time.deltaTime;
        this.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
