using UnityEngine;

public class Upgrade_2 : MonoBehaviour
{
    public GameObject DroidFriendly; // Friendly droid prefab to spawn
    public LayerMask groundLayer;    // Kept for existing scene settings; ground is also detected by tag
    [SerializeField] private float groundSpawnOffset = 0.5f;
    [SerializeField] private float groundProbeDistance = 50f;

    private bool collected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected || !collision.CompareTag("Player")) return;

        if (DroidFriendly == null)
        {
            Debug.LogWarning("Upgrade_2: no DroidFriendly prefab assigned, pickup ignored.", this);
            return;
        }
        collected = true;

        Vector2 spawnPos = transform.position;

        // Ground objects in this project use the "Ground" TAG rather than a dedicated
        // physics layer, and this pickup's layer mask is empty. Find the nearest
        // surface below by tag so the ally actually spawns on the floor.
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, groundProbeDistance);
        float groundY = float.NegativeInfinity;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground") && hit.point.y > groundY)
                groundY = hit.point.y;
        }

        if (float.IsNegativeInfinity(groundY))
            Debug.LogWarning("Upgrade_2: no ground below the pickup; spawning ally at the pickup position.", this);
        else
            spawnPos = new Vector2(transform.position.x, groundY + groundSpawnOffset);

        Instantiate(DroidFriendly, spawnPos, Quaternion.identity);
        Debug.Log("Upgrade_2: friendly droid spawned at " + spawnPos);
        Destroy(gameObject);
    }
}
