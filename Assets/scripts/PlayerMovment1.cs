using UnityEngine;

public class PlayerMovment1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D playerRb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator _animator;
    // Stores the X and Y distances the player will move in a single frame
    private Vector2 movement;
    // Stores the X and Y world space coordinates of the camera's edges
    private Vector2 screenBounds;
    // Stores exactly half the width of the player's sprite image
    private float playerHalfWidth;
    private float xPosLastFrame;
    private void Start()
    {
        // Converts the literal pixel dimensions of the screen into Unity's internal world coordinate system
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        // Grabs the SpriteRenderer attached to this object and gets the distance from its center point to its outer edge
        playerHalfWidth = spriteRenderer.bounds.extents.x;
    }
    
 

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        ClampMovement();
        FlipCharacterX();
    }
    private void FlipCharacterX()
    {
        float input = Input.GetAxisRaw("Horizontal");
        if (input > 0 && (transform.position.x > xPosLastFrame))
        {
            // we are moving right
            spriteRenderer.flipX = false;
        }
        else if (input < 0 && (transform.position.x < xPosLastFrame))
        {
            spriteRenderer.flipX = true;
        }
        xPosLastFrame = transform.position.x;

    }

    private void ClampMovement()
    {
        // Restricts the X position between the left screen edge (plus half the sprite) and the right screen edge (minus half the sprite)
        float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x + playerHalfWidth, screenBounds.x - playerHalfWidth);
        // Temporarily stores the current position, overwrites the X value with the restricted value, and applies it back
        Vector2 pos = transform.position;
        pos.x = clampedX;
        transform.position = pos;
    }

    private void HandleMovement()
    {
        // Listens for A/D or Left/Right Arrow keys. Returns -1 (left), 1 (right), or 0 (no input)
        float input = Input.GetAxisRaw("Horizontal");
        // Calculates distance to move: direction * speed * time since last frame
        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);
        if (input != 0) 
        {
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning",false);
        }
    }
}
