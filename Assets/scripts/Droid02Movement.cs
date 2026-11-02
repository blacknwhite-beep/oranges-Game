using UnityEngine;

public class Droid02Movement : MonoBehaviour
{

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float lostTimer;
    private float moveDir; 


    [Header("Movement")]
    public float walkSpeed = 2f;
    public float stopDistance = 1f;
    public float loseTargetDelay = 0.75f;
    private Transform lastTarget;


    [Header("Smoke")]
    [SerializeField] private GameObject SmokePrefab;
    [SerializeField] private float smokeInterval = 0.25f;
    [SerializeField] private Vector2 smokeOffset = new Vector2(0f, -0.4f);
    private float smokeTimer;



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
                moveDir = dir;
            }
        }

        if (!shouldMove)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        
        

        anim.SetBool("isMoving", shouldMove);
        HandleSmoke(shouldMove);
    }

    private void HandleSmoke(bool isMoving)
    {
        if (!isMoving || SmokePrefab == null)
        {
            smokeTimer = 0f; // spawn right away when movement starts
            return;
        }

        smokeTimer -= Time.fixedDeltaTime;
        if (smokeTimer <= 0f)
        {
            // Put the smoke behind the droid, based on which way it's facing
            float side = sr.flipX ? -1f : 1f;
            Vector3 pos = transform.position + new Vector3(-moveDir * smokeOffset.x * side, smokeOffset.y, 0f);

            GameObject smoke = Instantiate(SmokePrefab, pos, Quaternion.identity);

            var smokeSr = smoke.GetComponent<SpriteRenderer>();
            if (smokeSr != null) smokeSr.flipX = moveDir > 0;

            Destroy(smoke, 1f); // cleanup, adjust to your animation length

            smokeTimer = smokeInterval;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
