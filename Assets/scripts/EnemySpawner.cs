using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private DroneMovement1 drone;      // drag Drone_Enemy here
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxAlive = 10;

    [ContextMenu("Test Spawn")]
    void TestSpawn() => Instantiate(enemyPrefab, transform.position, Quaternion.identity);


    private Coroutine spawnRoutine;
    private float timer;
    private int alive;


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
            }
            yield return new WaitForSeconds(spawnInterval);
            Debug.Log($"SpawnLoop running. alive={alive}, max={maxAlive}, prefab={enemyPrefab}");
        }

        
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
        
    }
}

public class SpawnTracker : MonoBehaviour
{
    public System.Action onDestroyed;
    void OnDestroy() => onDestroyed?.Invoke();
}
