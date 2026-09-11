using UnityEngine;

public class playerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rb2D;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private Animator _animator;

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
        Rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        _animator.SetBool("isJumping", true);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks if the physical object the player just bumped into is tagged as "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Tells the Animator the player has landed
            _animator.SetBool("isJumping", false);
        }
    }
}
