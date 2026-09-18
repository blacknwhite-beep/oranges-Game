using UnityEngine;

public class playerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rb2D;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private Animator _animator;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        Debug.DrawRay(transform.position, Vector2.down * 1.5f, Color.red);
        if (Input.GetKeyDown(KeyCode.Space) && GetIsGrounded())
        {
            Jump();
        }
    }
    private void Jump()
    {
        if (isGrounded)
        {
            Rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canDoubleJump = true;
            _animator.SetBool("isJumping", true);
        }
        else if (canDoubleJump)
        {
            Rb2D.linearVelocity = new Vector2(Rb2D.linearVelocity.x, 0f); // Reset vertical velocity before double jump
            Rb2D.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            canDoubleJump = false;
            _animator.SetBool("isJumping", true);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks if the physical object the player just bumped into is tagged as "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Tells the Animator the player has landed
            _animator.SetBool("isJumping", false);
            isGrounded = true;
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
