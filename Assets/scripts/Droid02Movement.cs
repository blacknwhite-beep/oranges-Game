using UnityEngine;

public class Droid02Movement : MonoBehaviour
{

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float lostTimer;


    [Header("Movement")]
    public float walkSpeed = 2f;
    public float stopDistance = 1f;
    public float loseTargetDelay = 0.75f;
    private Transform lastTarget;


    [Header("Detection")]
    public DetectionZone detectionZone;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

    }

    private void FixedUpdate()
    {
        if (detectionZone.PlayerDetected && detectionZone.Target != null)
        {
            lastTarget = detectionZone.Target;
            lostTimer = loseTargetDelay;
        }
        else
        {
            lostTimer -= Time.fixedDeltaTime;
        }

        bool shouldMove = false;

        if (lostTimer > 0f && lastTarget != null)
        {
            float dx = lastTarget.position.x - transform.position.x;
            if (Mathf.Abs(dx) > stopDistance)
            {
                shouldMove = true;
                float dir = Mathf.Sign(dx);
                rb.linearVelocity = new Vector2(dir * walkSpeed, rb.linearVelocity.y);
                sr.flipX = dir > 0;
            }
        }

        if (!shouldMove)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        anim.SetBool("isMoving", shouldMove);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
