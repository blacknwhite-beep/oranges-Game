using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private DroneMovement1 drone;      // drag Drone_Enemy here
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
    private bool IsDead;
    private Collider2D col;

    [ContextMenu("Test Spawn")]
    void TestSpawn() => Instantiate(enemyPrefab, transform.position, Quaternion.identity);


    private Coroutine spawnRoutine;
    private float timer;
    private int alive;
    

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        if (drone == null) { Debug.LogError("EnemySpawner: drone not assigned", this); return; }
        drone.PlayerSpotted += OnSpotted;
        drone.PlayerLost += OnLost;
    }

    void OnDisable()
    {
        if (drone != null)
        {
            drone.PlayerSpotted -= OnSpotted;
            drone.PlayerLost -= OnLost;
        }
        StopSpawning();
    }
    void OnDestroy()
    {
        if (drone == null) return;
        drone.PlayerSpotted -= OnSpotted;
        drone.PlayerLost -= OnLost;
    }

    void OnSpotted(Transform player)
    {
        Debug.Log("Spawner heard PlayerSpotted");
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

        
    }

    IEnumerator SpawnLoop()
    {
        Debug.Log("SpawnLoop started");
        while (true)
        {
            if (alive < maxAlive)
            {
                var droid = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                alive++;
                droid.AddComponent<SpawnTracker>().onDestroyed = () => alive--;

                animator.SetTrigger("Spawn");
                yield return new WaitForSeconds(windUpTime);

                Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                alive++;
            }
            yield return new WaitForSeconds(spawnInterval);
            if (drone == null || drone.GetComponent<DroneHealth>().IsDead) yield break;
            Debug.Log($"SpawnLoop running. alive={alive}, max={maxAlive}, prefab={enemyPrefab}");
        }

        
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
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

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        StopSpawning();                                   // stop the spawn loop and reset the Spawn trigger
        if (col != null) col.enabled = false;             // can't be hit again

        Invoke(nameof(Explode), deathDelay);
    }

    public void OnDroidDied() => alive--;


    void Start()
    {
       

    }

    void Spawn()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        alive++;

        // Let the spawner know when the enemy dies
        var tracker = enemy.AddComponent<SpawnTracker>();
        tracker.onDestroyed = () => alive--;
    }

    // Update is called once per frame
    void Update()
    {
        drone.PlayerSpotted += OnSpotted;
        drone.PlayerLost += OnLost;
    }
}



public class SpawnTracker : MonoBehaviour
{
    public System.Action onDestroyed;
    void OnDestroy() => onDestroyed?.Invoke();
}
