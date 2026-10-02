using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class DroidEnemy_01 : MonoBehaviour
{
    public float walkSpeed = 2f;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public float currentHealth = 50f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;
    public float detectRange = 6f;
    public float shootInterval = 2f;

    float shootTimer;

    public LayerMask playerLayer;

    float cooldownTimer;

    public enum WalkableDirection{Left, Right}

    private Vector2 walkDirectionVector = Vector2.left;
    private WalkableDirection _walkDirection = WalkableDirection.Left;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if(_walkDirection != value)
            {
                // Direction flipped
                _walkDirection = value;
                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;

                // Assumes the sprite art faces right by default; swap the signs if yours faces left
                float facing = (value == WalkableDirection.Right) ? 1f : -1f;
                Vector3 s = transform.localScale;
                transform.localScale = new Vector3(Mathf.Abs(s.x) * facing, s.y, s.z);

            }

            _walkDirection = value;
            

        }
    }

    float GetDistanceFromGround()
    {
        RaycastHit hit;
        // Cast a ray straight down from the character position
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            return hit.distance; // Distance to the ground surface
        }
        return -1f; // No ground found
    }

    public void FireProjectile()
    {
        GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        p.GetComponent<DroidProjectile>().SetStraightVelocity(walkDirectionVector);
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

    public void Shoot() => animator.SetTrigger("shoot"); 
    
    public void Die() => animator.SetTrigger("dead");

    public void TakeHit(float damageRecieved)
    {
        animator.SetTrigger("hurt");
        currentHealth -= damageRecieved;
        if (currentHealth <= 0)
        {
            Die();
        } 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        WalkDirection = WalkableDirection.Left;

        GetDistanceFromGround(); 


    }

   

    


    // Update is called once per frame
    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }

    }
}
