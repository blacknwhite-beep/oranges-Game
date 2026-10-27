using UnityEngine;

public class Upgrade_3 : MonoBehaviour
   
{
    public float invincibilityDuration = 10f; // how long the power will last
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerhealth = collision.GetComponent<PlayerHealth>();
            if (playerhealth != null)
            {
                playerhealth.ActivateInvincibility(invincibilityDuration);
                Destroy(gameObject);
            }
        }
    }
}
