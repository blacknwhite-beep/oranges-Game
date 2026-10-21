using UnityEngine;

public class Upgrade_1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Find the shooting script on the player
            PlayerShooting shootingScript = collision.GetComponent<PlayerShooting>();
            Debug.Log("Player collided with Upgrade_1");

            if (shootingScript != null)
            {
                Debug.Log("Found PlayerShooting script on player"); 
                // Tell the player to unlock the gun
                shootingScript.unlockGun();

                // Destroy the collectable icon
                Destroy(gameObject);
            }
        }
    }
}
