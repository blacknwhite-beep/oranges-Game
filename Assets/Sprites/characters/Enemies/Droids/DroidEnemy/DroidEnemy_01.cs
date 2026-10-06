using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class DroidEnemy_01 : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float stopDistance = 1f;      // stops walking when this close to the player

    [Header("Detection")]
    public DetectionZone detectionZone;  // drag the DetectionZone child here

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;

    [Header("Health")]
    public float currentHealth = 50f;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    float shootTimer;
    bool isDead;

    public enum WalkableDirection { Left, Right }

    private Vector2 walkDirectionVector = Vector2.left;
    private WalkableDirection _walkDirection = WalkableDirection.Left;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                _walkDirection = value;
                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;

                // Assumes the sprite art faces right by default; swap the signs if yours faces left
                float facing = (value == WalkableDirection.Right) ? 1f : -1f;
                Vector3 s = transform.localScale;
                transform.localScale = new Vector3(Mathf.Abs(s.x) * facing, s.y, s.z);
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (detectionZone == null)
            Debug.LogWarning("DroidEnemy_01: Detection Zone is not assigned.", this);

        WalkDirection = WalkableDirection.Left;
    }

    private bool PlayerInZone => detectionZone != null && detectionZone.PlayerDetected;

    private void Update()
    {
        if (isDead || !PlayerInZone) return;

        // Face the player
        float dx = detectionZone.Target.position.x - transform.position.x;
        WalkDirection = dx > 0 ? WalkableDirection.Right : WalkableDirection.Left;

        // Shoot on a timer, only while the player is inside the zone
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    private void FixedUpdate()
    {
        bool shouldMove = false;

        if (!isDead && PlayerInZone)
        {
            float distance = Mathf.Abs(detectionZone.Target.position.x - transform.position.x);
            shouldMove = distance > stopDistance;
        }

        float vx = shouldMove ? walkDirectionVector.x * walkSpeed : 0f;
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
        animator.SetBool("isMoving", shouldMove);
    }

    public void Shoot() => animator.SetTrigger("shoot");

    // Call this from an Animation Event on the Droid_Shoot clip
    public void FireProjectile()
    {
        if (isDead) return;

        GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        p.GetComponent<DroidProjectile>().SetStraightVelocity(walkDirectionVector);
    }

    public void TakeHit(float damageReceived)
    {
        if (isDead) return;

        animator.SetTrigger("hurt");
        currentHealth -= damageReceived;
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        animator.SetTrigger("dead");
    }
}