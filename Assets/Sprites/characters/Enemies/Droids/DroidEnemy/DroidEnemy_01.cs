using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class DroidEnemy_01 : MonoBehaviour
{
    public float walkSpeed = 5f;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public enum WalkableDirection{Left, Right}

    private Vector2 walkDirectionVector;
    private WalkableDirection _walkDirection;
    public int attackDamage = 1;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if(_walkDirection != value)
            {
                // Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if(value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                } else if(value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }

            }

            _walkDirection = value;
            

        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>(); 
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(walkDirectionVector.x * walkSpeed, rb.linearVelocity.y);
        animator.SetBool("isMoving", Mathf.Abs(rb.linearVelocity.x) > 0.01f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        Debug.Log("Enemy bumped into: " + collision.gameObject.name);
        if (player != null)
        {
            player.TakeDamage(attackDamage);
        }
    }

    public void Shoot() => animator.SetTrigger("shoot"); 
    public void TakeHit() => animator.SetTrigger("hurt");
    public void Die() => animator.SetTrigger("dead");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
