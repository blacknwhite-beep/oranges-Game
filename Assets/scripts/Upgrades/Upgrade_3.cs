using UnityEngine;

public class Upgrade_3 : MonoBehaviour
{
    [SerializeField] private float invincibilityDuration = 10f; // how long the power will last

    private bool collected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected || !collision.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogWarning("Upgrade_3: player has no PlayerHealth component, pickup ignored.", this);
            return;
        }

        collected = true;
        playerHealth.ActivateInvincibility(invincibilityDuration);
        Debug.Log("Upgrade_3 collected: invincibility active for " + invincibilityDuration + "s.");
        Destroy(gameObject);
    }
}
