using UnityEngine;

// Second upgrade: picking it up deploys a friendly droid that fights alongside the player.
public class Upgrade_2 : MonoBehaviour
{
    [SerializeField] private GameObject allyDroidPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (allyDroidPrefab == null)
        {
            Debug.LogWarning("Upgrade_2 has no ally droid prefab assigned!");
            return;
        }

        // Spawn the helper droid next to the player
        Vector3 spawnPosition = collision.transform.position + new Vector3(1.2f, 0f, 0f);
        GameObject ally = Instantiate(allyDroidPrefab, spawnPosition, Quaternion.identity);
        ally.name = "AllyDroid";
        Debug.Log("Player collected Upgrade_2 - ally droid deployed!");

        // Destroy the collectable icon
        Destroy(gameObject);
    }
}
