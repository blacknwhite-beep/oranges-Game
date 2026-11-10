using UnityEngine;

public class Droid03Movement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
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
        ChangeState("Droid03Idle");

    }

    // Update is called once per frame
    void Update()
    {
        Transform target = lastTarget; // e.g. detectionZone.GetTarget();

        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.position);

            if (dist > stopDistance)
            {
                float dir = Mathf.Sign(target.position.x - transform.position.x);
                rb.linearVelocity = new Vector2(dir * walkSpeed, rb.linearVelocity.y);
                sr.flipX = dir < 0;// face the player
                ChangeState("Droid03Move");

            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                ChangeState("Droid03Attack");
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            ChangeState("Droid03Idle");

        }
    }

}

