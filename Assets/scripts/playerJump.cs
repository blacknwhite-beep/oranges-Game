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
       
        Debug.DrawRay(transform.position, Vector2.down * 1.5f, Color.red);
        if (Input.GetButtonDown("Jump") && GetIsGrounded())
        {
            Jump();
        }
    }

    private bool GetIsGrounded()
    {
        // Lowers the starting point by 1 unit (adjust the 1f based on your sprite's size)
        Vector2 startPos = new Vector2(transform.position.x, transform.position.y - 1f);

        // Shoots a shorter raycast since it is already starting closer to the ground
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, 0.5f);

        // Draws the new offset raycast in the Scene view for debugging
        Debug.DrawRay(startPos, Vector2.down * 0.5f, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            return true;
        }
        return false;
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
