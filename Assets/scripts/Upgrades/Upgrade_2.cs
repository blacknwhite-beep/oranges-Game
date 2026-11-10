using UnityEngine;

public class Upgrade_2 : MonoBehaviour
{
    public GameObject DroidFriendly; // Reference to the DroidFriendly prefab
    public LayerMask groundLayer; // Layer mask for the ground layer
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 spawnPos = transform.position; // Get the position of the upgrade
            //shoot a raycast downwards to find the ground position
            RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,50f, groundLayer);
            if (hit.collider != null)
            {
                spawnPos = new Vector2(transform.position.x, hit.point.y + 0.5f);
            }
            Instantiate(DroidFriendly, spawnPos, Quaternion.identity); // Instantiate the DroidFriendly at the ground position
            // Destroy the collectable icon
            Destroy(gameObject);
        }
    }
}
