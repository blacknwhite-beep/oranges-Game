using System;
using System.Collections; // Required for Coroutines
using UnityEngine;
using static PlayerMovementState;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 10;
    public int currentHealth { get; private set; }
    public int maxHealth { get; private set; }
    public static Action<int> OnPlayerTakeDamage;
    public HealthBar healthBar;

    public bool isInvincible = false;
    private SpriteRenderer spriteRenderer; // Fixed syntax
    private Color originalColor;

    void Awake()
    {
        currentHealth = health;
        maxHealth = health;
    }

    void Start()
    {
        // Setup the sprite renderer for color flashing
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        // Abort the damage completely if the player is invincible or already dead
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        OnPlayerTakeDamage?.Invoke(currentHealth);

        PlayerMovementState playerMovementState = GetComponent<PlayerMovementState>();

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            // Call the death animation state instead of destroying instantly
            if (playerMovementState != null)
            {
                playerMovementState.HandleDeath();
            }
        }
        else
        {
            // Only play the hurt animation if the player survived the hit
            if (playerMovementState != null)
            {
                playerMovementState.SetMoveState(MovementState.Hurt);
            }
        }
    }

    // --- INVINCIBILITY LOGIC ---

    public void ActivateInvincibility(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(InvincibilityRoutine(duration));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;
        float timer = 0f;
        Color goldColor = new Color(1f, 0.84f, 0f);

        while (timer < duration)
        {
            // Rapidly flash between Gold and White
            if (Mathf.FloorToInt(timer * 10) % 2 == 0)
                spriteRenderer.color = goldColor;
            else
                spriteRenderer.color = Color.white;

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    // Instantly kill enemies if using standard physical colliders
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

    // Instantly kill enemies if using trigger colliders
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInvincible && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}