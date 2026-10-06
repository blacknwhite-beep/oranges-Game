using UnityEngine;

public class ProjectileShoot : MonoBehaviour
{
    public float speed = 10f;
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
}
