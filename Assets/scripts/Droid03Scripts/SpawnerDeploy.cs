using UnityEngine;

[RequireComponent(typeof(DroneMovement1))]
public class SpawnerDeploy : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawnerPrefab;   // drag the EnemySpawner prefab (from Project window)

    [Header("Placement")]
    [Tooltip("Optional. If set, the spawner appears exactly here (e.g. an empty object you placed in the level).")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Used when Spawn Point is empty: offset from the drone's START position.")]
    [SerializeField] private Vector2 offsetFromDrone = new Vector2(2.5f, 0f);

    [Tooltip("Put the spawner on the side the player is on.")]
    [SerializeField] private bool spawnTowardPlayer = true;

    [Header("Ground Snap")]
    [SerializeField] private bool snapToGround = true;
    [SerializeField] private LayerMask groundLayers = ~0;     // set this to your ground/platform layer
    [SerializeField] private float maxDropDistance = 20f;
    [SerializeField] private float heightAboveGround = 0f;    // tweak so the sprite's feet sit on the floor

    [Header("Other")]
    [SerializeField] private bool redeployIfDestroyed = false;

    private DroneMovement1 drone;
    private EnemySpawner spawnerInstance;
    private Vector2 startPos;
    private bool deployed;

    void Awake()
    {
        drone = GetComponent<DroneMovement1>();
        startPos = transform.position;   // the drone bobs and chases, so remember where it was placed
    }

    void OnEnable() => drone.PlayerSpotted += OnSpotted;
    void OnDisable() => drone.PlayerSpotted -= OnSpotted;

    void OnSpotted(Transform player)
    {
        if (spawnerPrefab == null) { Debug.LogError("SpawnerDeployer: spawner prefab not assigned", this); return; }
        if (spawnerInstance != null) return;
        if (deployed && !redeployIfDestroyed) return;

        Vector3 pos = GetSpawnPosition(player);
        spawnerInstance = Instantiate(spawnerPrefab, pos, Quaternion.identity);
        spawnerInstance.Init(drone, player);
        deployed = true;
    }

    Vector3 GetSpawnPosition(Transform player)
    {
        if (spawnPoint != null) return spawnPoint.position;

        Vector2 offset = offsetFromDrone;
        if (spawnTowardPlayer && player != null)
        {
            float side = Mathf.Sign(player.position.x - startPos.x);
            offset.x = Mathf.Abs(offset.x) * side;
        }

        Vector2 pos = startPos + offset;

        if (snapToGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, maxDropDistance, groundLayers);
            if (hit.collider != null)
                pos.y = hit.point.y + heightAboveGround;
        }

        return pos;
    }

    // Shows where the spawner will appear when you select the drone in the Scene view.
    void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying ? startPos : (Vector2)transform.position;
        Gizmos.color = Color.cyan;

        if (spawnPoint != null)
        {
            Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
            return;
        }

        Vector2 right = origin + new Vector2(Mathf.Abs(offsetFromDrone.x), offsetFromDrone.y);
        Vector2 left = origin + new Vector2(-Mathf.Abs(offsetFromDrone.x), offsetFromDrone.y);
        Gizmos.DrawWireSphere(right, 0.3f);
        if (spawnTowardPlayer) Gizmos.DrawWireSphere(left, 0.3f);
        if (snapToGround)
        {
            Gizmos.DrawLine(right, right + Vector2.down * maxDropDistance);
            if (spawnTowardPlayer) Gizmos.DrawLine(left, left + Vector2.down * maxDropDistance);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
