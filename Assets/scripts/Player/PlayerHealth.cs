using System;
using UnityEngine;
using static PlayerMovementState;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private int health = 10;
    public int currentHealth { get; private set; }
    public int maxHealth { get; private set; }
    public static Action<int> OnPlayerTakeDamage;

    

    void Awake()
    {
        currentHealth = health;
        maxHealth = health;
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnPlayerTakeDamage?.Invoke(currentHealth);

        PlayerMovementState playerMovementState = GetComponent<PlayerMovementState>();
        if (playerMovementState != null)
        {
            playerMovementState.SetMoveState(MovementState.Hurt);
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

}
