using UnityEngine;

public class Droid03Movement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Droid01Health health;
    private float lostTimer;
    private float moveDir;
    private string currentState;


    [Header("Movement")]
    public float walkSpeed = 2f;
    public float stopDistance = 1f;
    public float loseTargetDelay = 0.75f;
    private Transform lastTarget;

    [Header("Detection")]
    public DetectionZone detectionZone;



    void ChangeState(string newState)
    {
        if (currentState == newState) return;   // already playing it
        anim.Play(newState);
        currentState = newState;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<Droid01Health>(); 
        ChangeState("Droid03Idle");

    }

    // Update is called once per frame
    void Update()
    {
        // Stop everything if dead
        if (health != null && health.IsDead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // THIS replaces "Transform target = lastTarget;"
        // Read the player from the DetectionZone
        if (detectionZone != null && detectionZone.Target != null)
        {
            lastTarget = detectionZone.Target;
            lostTimer = loseTargetDelay;
        }
        else if (lastTarget != null)
        {
            lostTimer -= Time.deltaTime;
            if (lostTimer <= 0f) lastTarget = null;
        }

        // Move toward the target
        if (lastTarget != null)
        {
            float dx = lastTarget.position.x - transform.position.x;

            if (Mathf.Abs(dx) > stopDistance)
            {
                float dir = Mathf.Sign(dx);
                rb.linearVelocity = new Vector2(dir * walkSpeed, rb.linearVelocity.y);
                sr.flipX = dir > 0;
                ChangeState("Droid03Move");
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                ChangeState("Droid03Idle");
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            ChangeState("Droid03Idle");
        }
    }

}

