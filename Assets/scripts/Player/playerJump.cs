using UnityEngine;

public class playerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rb2D;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private Animator _animator;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(5f, 5f); // Force applied during wall jump

    private bool canDoubleJump;
    private bool isGrounded;
    private float playerHalfWidth;

    void Start()
    {
        playerHalfWidth = spriteRenderer.bounds.extents.x; // Get half the width of the player's spritex`
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector2.down * 1.5f, Color.red);

        // Check for input once, then pass it to the main Jump logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            // Standard Ground Jump
            Rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canDoubleJump = true;
            _animator.SetBool("isJumping", true);
        }
        else
        {
            int direction = GetWallJumpDirection();

            // Wall Jump Check (prioritized over double jump if next to a wall)
            if (direction != 0)
            {
                wallJump(direction);
            }
            // Double Jump Check
            else if (canDoubleJump)
            {
                DoubleJump();
            }
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

    private int GetWallJumpDirection()
    {
        float raycastDistance = playerHalfWidth + 0.1f;

        // Draw rays in the Scene view to visualize them (Blue for right, Green for left)
        Debug.DrawRay(transform.position, Vector2.right * raycastDistance, Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * raycastDistance, Color.green);

        if (Physics2D.Raycast(transform.position, Vector2.right, raycastDistance, LayerMask.GetMask("Wall")))
        {
            return -1; // Wall is to the right, jump left
        }
        if (Physics2D.Raycast(transform.position, Vector2.left, raycastDistance, LayerMask.GetMask("Wall")))
        {
            return 1; // Wall is to the left, jump right
        }
        return 0; // No wall detected
    }

    private void wallJump(int direction)
    {
        Vector2 force = wallJumpForce;
        force.x *= direction; // Apply the direction to the x component of the force

        // Reset velocity before applying force to ensure consistent jump heights
        Rb2D.linearVelocity = new Vector2(0f, 0f);
        Rb2D.AddForce(force, ForceMode2D.Impulse);
    }

    private void DoubleJump()
    {
        Rb2D.linearVelocity = new Vector2(Rb2D.linearVelocity.x, 0f); // Reset vertical velocity before double jump
        Rb2D.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
        canDoubleJump = false;
        _animator.SetBool("isJumping", true);
    }
}