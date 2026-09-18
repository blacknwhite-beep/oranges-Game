using UnityEngine;

public class Platform_Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f; // speed of the platform
    private float timer = 0;

    [SerializeField] public float maxTimeMovingLeft = 15; // How long the platform moves left before switching direction
    [SerializeField] public float maxTimeMovingRight = 60; //Twice as long as left, so platform will return to 0 and move right for the same amount of time as left


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < maxTimeMovingLeft)
        {
            timer = timer + Time.deltaTime;
            transform.position = transform.position + (Vector3.left * moveSpeed) * Time.deltaTime;
        }
        else if (timer < ( maxTimeMovingLeft * 3))
        {
            timer = timer + Time.deltaTime;
            transform.position = transform.position + (Vector3.right * moveSpeed) * Time.deltaTime;

        }
        else
        {
            timer = -(maxTimeMovingLeft);
        }



    }
        
    
}
