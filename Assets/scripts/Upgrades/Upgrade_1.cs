using UnityEngine;

public class Upgrade_1 : MonoBehaviour
{
    private bool collected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected || !collision.CompareTag("Player")) return;

        PlayerShooting shootingScript = collision.GetComponent<PlayerShooting>();
        if (shootingScript == null)
        {
            Debug.LogWarning("Upgrade_1: player has no PlayerShooting component, pickup ignored.", this);
            return;
        }

        collected = true;
        shootingScript.unlockGun();
        Debug.Log("Upgrade_1 collected: gun unlocked.");
        Destroy(gameObject);
    }
}
