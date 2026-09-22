using UnityEngine;

public class PlayerMovment1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D playerRb;
    public float speed;
    public float input;
 

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        Input = Input.GetAxisRaw("Horizontal");
=======
        HandleMovement();
        ////ClampMovement();
        FlipCharacterX();
>>>>>>> origin/Fixxed-stage-v0.01
    }
    void FixedUpdate()
    {
        playerRb.linearVelocity = new Vector2(input * speed, playerRb.linearVelocity.y);
    }

    //private void ClampMovement()
    //{
    //    // Restricts the X position between the left screen edge (plus half the sprite) and the right screen edge (minus half the sprite)
    //    float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x + playerHalfWidth, screenBounds.x - playerHalfWidth);
    //    // Temporarily stores the current position, overwrites the X value with the restricted value, and applies it back
    //    Vector2 pos = transform.position;
    //    pos.x = clampedX;
    //    transform.position = pos;
    //}

    private void HandleMovement()
    {
        // Locks rotation directly in code so the physics engine cannot tilt the character
        playerRb.freezeRotation = true;

        float input = Input.GetAxisRaw("Horizontal");

        // Explicitly sets the Y value to 0 to prevent erratic vertical flying
        movement = new Vector2(input * speed * Time.deltaTime, 0);
        transform.Translate(movement);

        if (input != 0)
        {
            _animator.SetBool("isRunning", true);
            // Replaces your buggy FlipCharacterX logic to stop the rapid visual flickering
            spriteRenderer.flipX = input < 0;
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
    }
}
