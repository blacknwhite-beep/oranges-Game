using UnityEngine;

// Shot by the ally droid. Only hurts enemies - never the player.
public class AllyProjectile : MonoBehaviour
{
    [SerializeField] private float normalBulletSpeed = 10f;
    [SerializeField] private float destroyTime = 3f;
    [SerializeField] private int damageAmount = 10;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    public void SetVelocity(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * normalBulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Never hurt the player
        if (other.CompareTag("Player"))
            return;

        // Ground is a tag in this project, not a layer
        if (other.CompareTag("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Impact();
            return;
        }

        DroidEnemy_01 droid = other.GetComponentInParent<DroidEnemy_01>();
        if (droid != null)
        {
            droid.TakeHit(damageAmount);
            Impact();
        }
    }

    private void Impact()
    {
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
        if (animator != null)
            animator.SetTrigger("isHit");
    }

    // Called by the impact animation event
    public void OnImpactFinished()
    {
        Destroy(gameObject);
    }
}
