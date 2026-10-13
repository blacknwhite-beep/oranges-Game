using System;
using UnityEngine;
using static PlayerMovementState;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private int health = 10;
    public int currentHealth { get; private set; }
    public int maxHealth { get; private set; }
    public static Action<int> OnPlayerTakeDamage;
    public HealthBar healthBar;
    void Start()
    {
        // Your other start code...

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(currentHealth); // Or whatever your max health variable is named
        }
    }



    void Awake()
    {
        currentHealth = health;
        maxHealth = health;
        
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        OnPlayerTakeDamage?.Invoke(currentHealth);

        PlayerMovementState playerMovementState = GetComponent<PlayerMovementState>();
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        if (playerMovementState != null)
        {
            playerMovementState.SetMoveState(MovementState.Hurt);
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

}
