using UnityEngine;

public class ProjectileShoot : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    [Header("Hit Feedback")]
    [SerializeField] private float hitStopDuration = 0.12f;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
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

        DroidEnemy_01 enemy = collision.GetComponent<DroidEnemy_01>();
        if (enemy != null)
        {
            enemy.TakeHit(damage);
            HitStop.Request(hitStopDuration);
            Destroy(gameObject); // Destroy the projectile on collision
        }


    }
}
