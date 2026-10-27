using Unity.VisualScripting;
using UnityEngine;

public class DroneHealth : MonoBehaviour
{

   

    private Animator anim;




    [Header("Health")]
    public float currentHealth = 10f;
    [SerializeField] private GameObject explosionPrefab;   // drag DestructionAnim prefab here
    [SerializeField] private float explosionLifetime = 1f; // length of the explosion clip
    [SerializeField] private float deathDelay = 0.5f;

    
    private Collider2D col;
    private DroneMovement1 movement;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        movement = GetComponent<DroneMovement1>();
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        anim.SetTrigger("Death");
        col.enabled = false;
        movement.enabled = false;   // stop detecting and chasing

        Invoke(nameof(Explode), deathDelay);
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(fx, explosionLifetime);
        }
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
