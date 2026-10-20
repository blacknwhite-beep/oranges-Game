using UnityEngine;

// Helper droid spawned by Upgrade_2. It follows the player and shoots at enemies.
[RequireComponent(typeof(Rigidbody2D))]
public class AllyDroid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Combat")]
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float shootInterval = 1.5f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private float playerHeightThreshold = 1.2f;
    [SerializeField] private LayerMask enemyLayers;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Transform target;
    private float shootTimer;
    private Vector2 aimDirection = Vector2.left;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // The droid prefab is authored with no gravity - the ally needs it to stand on the ground
        rb.gravityScale = 1f;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        SnapToGround();
    }

    private void Update()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        target = FindNearestEnemy();
        shootTimer -= Time.deltaTime;

        float moveX = 0f;

        if (target != null)
        {
            Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;
            float distance = toTarget.magnitude;

            // Close the distance before shooting
            if (distance > attackRange)
                moveX = toTarget.x > 0f ? 1f : -1f;

            if (shootTimer <= 0f && distance <= attackRange)
            {
                Shoot(toTarget);
                shootTimer = shootInterval;
            }
        }
        else if (player != null)
        {
            // No enemies nearby - keep up with the player
            float dx = player.position.x - transform.position.x;
            if (Mathf.Abs(dx) > followDistance)
                moveX = dx > 0f ? 1f : -1f;

            // Hop up when the player is on a higher platform
            float dy = player.position.y - transform.position.y;
            if (dy > playerHeightThreshold && Mathf.Abs(dx) < 3f && rb.linearVelocity.y <= 0.01f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }

        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        if (spriteRenderer != null && moveX != 0f)
            spriteRenderer.flipX = moveX < 0f;

        if (animator != null)
            animator.SetBool("isMoving", Mathf.Abs(rb.linearVelocity.x) > 0.01f);
    }

    // Drops the droid straight onto the ground below its spawn point
    private void SnapToGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + Vector3.up * 0.5f, Vector2.down, 20f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || !hit.collider.CompareTag("Ground"))
                continue;

            float feetOffset = 0f;
            Collider2D own = GetComponent<Collider2D>();
            if (own != null)
                feetOffset = own.bounds.min.y - transform.position.y;

            transform.position = new Vector3(transform.position.x, hit.point.y - feetOffset, transform.position.z);
            return;
        }
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayers);

        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.transform == transform)
                continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hit.transform;
            }
        }

        return closest;
    }

    private void Shoot(Vector2 direction)
    {
        aimDirection = direction.normalized;
        if (animator != null)
            animator.SetTrigger("shoot");
    }

    // Called by the FireProjectile animation event on the shoot clip
    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        AllyProjectile projectile = p.GetComponent<AllyProjectile>();
        if (projectile != null)
            projectile.SetVelocity(aimDirection);
    }
}
