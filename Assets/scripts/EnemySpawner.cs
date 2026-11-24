using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private DroneMovement1 drone;      // set automatically by SpawnerDeployer (can also be dragged in)
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxAlive = 10;
    [SerializeField] private Animator animator;
    [SerializeField] private float windUpTime = 0.5f;

    [Header("Health")]
    public float currentHealth = 5f;
    [SerializeField] private GameObject explosionPrefab;   // drag DestructionAnim prefab here
    [SerializeField] private float explosionLifetime = 1f;
    [SerializeField] private float deathDelay = 0.5f;

    private bool isDead;
    private bool subscribed;
    private Collider2D col;
    private Coroutine spawnRoutine;
    private int alive;

    [ContextMenu("Test Spawn")]
    void TestSpawn() => Spawn();

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    // Called by SpawnerDeployer right after Instantiate.
    // The player is already spotted at this point, so start spawning straight away.
    public void Init(DroneMovement1 owner, Transform player)
    {
        Unsubscribe();
        drone = owner;
        Subscribe();
        OnSpotted(player);

        Debug.Log("EnemySpawner.Init called", this);
    }

    void OnEnable() => Subscribe();     // only does something if drone was already assigned
    void OnDisable() { Unsubscribe(); StopSpawning(); }

    void Subscribe()
    {
        if (drone == null || subscribed) return;
        drone.PlayerSpotted += OnSpotted;
        drone.PlayerLost += OnLost;
        subscribed = true;
    }

    void Unsubscribe()
    {
        if (drone == null || !subscribed) return;
        drone.PlayerSpotted -= OnSpotted;
        drone.PlayerLost -= OnLost;
        subscribed = false;
    }

    // ---------- Drone events ----------

    void OnSpotted(Transform player)
    {
        if (isDead) return;
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    void OnLost() => StopSpawning();

    void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
        if (animator != null) animator.ResetTrigger("Spawn");
    }

    // ---------- Spawning ----------

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (drone == null) yield break;
            var droneHealth = drone.GetComponent<DroneHealth>();
            if (droneHealth != null && droneHealth.IsDead) yield break;

            if (alive < maxAlive)
            {
                if (animator != null) animator.SetTrigger("Spawn");
                yield return new WaitForSeconds(windUpTime);
                Spawn();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void Spawn()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        alive++;
        enemy.AddComponent<SpawnTracker>().onDestroyed = () => alive--;

        Debug.Log("Spawning droid", this);
    }

    // ---------- Health ----------

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StopSpawning();
        if (col != null) col.enabled = false;
        Invoke(nameof(Explode), deathDelay);
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(fx, explosionLifetime);
        }
        Destroy(gameObject);
    }
}

public class SpawnTracker : MonoBehaviour
{
    public System.Action onDestroyed;
    void OnDestroy() => onDestroyed?.Invoke();
}