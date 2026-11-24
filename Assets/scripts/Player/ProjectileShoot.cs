using UnityEngine;

public class ProjectileShoot : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    [Header("Hit Feedback")]
    [SerializeField] private float hitStopDuration = 0.12f;
    private Rigidbody2D rb;
    private bool directionAssigned;
    private Vector2 assignedDirection;

    /// <summary>
    /// Lets a shooter (e.g. the friendly droid) aim this bullet. Must be called
    /// before Start so the default "transform.right" velocity doesn't overwrite it.
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        assignedDirection = direction.sqrMagnitude < 0.0001f ? Vector2.right : direction.normalized;
        directionAssigned = true;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = assignedDirection * speed;
        transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f,
            Mathf.Atan2(assignedDirection.y, assignedDirection.x) * Mathf.Rad2Deg);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!directionAssigned)
            rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, 3f); // Destroy the projectile after 3 seconds
    }

    // Update is called once per frame
    void Update()   
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        // Bullets should stop when they hit the level, not fly through it.
        if (collision.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        DroidEnemy_01 enemy = collision.GetComponent<DroidEnemy_01>();
        if (enemy != null)
        {
            enemy.TakeHit(damage);
            HitStop.Request(hitStopDuration);
            Destroy(gameObject); // Destroy the projectile on collision
        }


    }
}
