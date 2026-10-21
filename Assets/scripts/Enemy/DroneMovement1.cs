using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneMovement1 : MonoBehaviour
{
    [Header("Hover")]
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;

    [Header("Detection")]
    [SerializeField] private Collider2D scanCone;      // drag ScanLight here
    [SerializeField] private float loseSightDelay = 2f;
    [SerializeField] private float alertTime = 0.5f;


    [Header("Chase Settings")]
    public float chaseSpeed = 5f;
    [SerializeField] private float hoverAbovePlayer = 1.5f;


    private Animator animator;
    private Collider2D playerCol;
    private Transform target;
    private Vector2 basePos;
    private float alertTimer;
    private float loseTimer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        basePos = transform.position;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No object tagged 'Player' found!");
            return;
        }

        playerCol = player.GetComponentInChildren<Collider2D>();
        if (playerCol == null)
            Debug.LogError("Player has no Collider2D!");

        if (scanCone == null)
            Debug.LogError("Scan Cone not assigned on DroneMovement1!");


    }





    // Update is called once per frame
    void Update()
    {
        bool seen = playerCol != null && scanCone != null
                    && scanCone.Distance(playerCol).isOverlapped;

        if (seen)
        {
            loseTimer = loseSightDelay;
            if (target == null)
            {
                target = playerCol.transform;
                alertTimer = alertTime;
                animator.SetTrigger("Scan");
                Debug.Log("Player detected!");
            }
        }
        else if (target != null)
        {
            loseTimer -= Time.deltaTime;
            if (loseTimer <= 0f)
            {
                target = null;
                Debug.Log("Player lost!");
            }
        }

        bool chasing = false;

        if (target != null)
        {
            if (alertTimer > 0f)
            {
                alertTimer -= Time.deltaTime;
            }
            else
            {
                chasing = true;
                Vector2 goal = (Vector2)target.position + Vector2.up * hoverAbovePlayer;
                basePos = Vector2.MoveTowards(basePos, goal, chaseSpeed * Time.deltaTime);

                float dx = target.position.x - basePos.x;
                if (Mathf.Abs(dx) > 0.05f)
                {
                    Vector3 s = transform.localScale;
                    s.x = Mathf.Abs(s.x) * (dx < 0 ? -1 : 1);
                    transform.localScale = s;
                }
            }

        }

        float bob = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(basePos.x, basePos.y + bob, transform.position.z);

        animator.SetBool("isMoving", chasing);

    }
}





