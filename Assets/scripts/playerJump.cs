using UnityEngine;

public class playerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rb2D;
    [SerializeField] private float jumpForce = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    private void Jump()
    {
        print("jump");
    }
}
