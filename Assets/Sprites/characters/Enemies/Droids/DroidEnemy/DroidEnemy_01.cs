using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class DroidEnemy_01 : MonoBehaviour
{
    public float walkSpeed = 2f;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public float currentHealth = 50f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;
    public float detectRange = 6f;
    public float shootInterval = 2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunShot;

    float shootTimer;

    public LayerMask playerLayer;

    [Header("Hover")]
    [SerializeField] private bool hoverEnabled = true;
    [SerializeField] private float hoverHeight = 0.25f;      // world units above the ground (~25 px at this sprite's 100 PPU)
    [SerializeField] private float hoverBobAmplitude = 0.04f; // small up/down bob so the droid reads as floating
    [SerializeField] private float hoverBobFrequency = 2f;
    [SerializeField] private float hoverSpeed = 4f;
    [SerializeField] private float groundProbeDistance = 30f;
    private Collider2D bodyCollider;

    [Header("Follow (friendly droid)")]
    [SerializeField] private float followDistance = 1.6f;      // how far behind the player to trail
    [SerializeField] private float followStopDistance = 0.35f;
    [SerializeField] private float followMaxSpeed = 4f;
    [SerializeField] private float followResponsiveness = 4f;

    [Header("Vertical follow / rocket boost")]
    [SerializeField] private float verticalFollowOffset = 0f;     // same Y level as the player
    [SerializeField] private float rocketBoostThreshold = 0.5f;   // how much higher the player must be
    [SerializeField] private float rocketBoostSpeed = 6f;
    [SerializeField] private float rocketBoostDuration = 0.45f;
    private float boostTimer;

    private bool isFriendly;
    private Transform playerTransform;
    private SpriteRenderer playerSprite;
    private SpriteRenderer spriteRenderer;

    float cooldownTimer;

    public enum WalkableDirection { Left, Right }

    private Vector2 walkDirectionVector = Vector2.left;
    private WalkableDirection _walkDirection = WalkableDirection.Left;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Direction flipped
                _walkDirection = value;
                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;

                // Assumes the sprite art faces right by default; swap the signs if yours faces left
                float facing = (value == WalkableDirection.Right) ? 1f : -1f;
                Vector3 s = transform.localScale;
                transform.localScale = new Vector3(Mathf.Abs(s.x) * facing, s.y, s.z);

            }

            _walkDirection = value;


        }
    }

    float GetDistanceFromGround()
    {
        RaycastHit hit;
        // Cast a ray straight down from the character position
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            return hit.distance; // Distance to the ground surface
        }
        return -1f; // No ground found
    }

    /// <summary>
    /// Direction for the friendly droid's shots: the nearest enemy that isn't an
    /// ally (allies are tagged Player). Falls back to the walking direction when
    /// no enemy is in range.
    /// </summary>
    private Vector2 GetAimDirection()
    {
        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        float bestDist = detectRange;
        DroidEnemy_01 best = null;

        foreach (DroidEnemy_01 d in FindObjectsByType<DroidEnemy_01>(FindObjectsSortMode.None))
        {
            if (d == this || d.gameObject.CompareTag("Player")) continue;
            float dist = Vector2.Distance(origin, d.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = d;
            }
        }

        if (best == null)
            return walkDirectionVector.sqrMagnitude > 0f ? walkDirectionVector.normalized : Vector2.right;

        Vector2 aim = (Vector2)best.transform.position + Vector2.up * 0.15f - origin;
        return aim.sqrMagnitude < 0.0001f ? Vector2.right : aim.normalized;
    }

    public void FireProjectile()
    {
        // 1. Stop immediately if the Inspector slots are empty
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing projectile or firepoint in the Inspector!");
            return;
        }

        GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // 1b. The friendly droid uses the player's bullet prefab. Aim it at the nearest
        // real enemy instead of letting ProjectileShoot fire blindly to the right.
        ProjectileShoot allyBullet = p.GetComponent<ProjectileShoot>();
        if (allyBullet != null)
        {
            allyBullet.SetDirection(GetAimDirection());
            return;
        }

        // 2. Try to shoot it as a standard enemy Droid bullet
        DroidProjectile droidBullet = p.GetComponent<DroidProjectile>();
        if (droidBullet != null)
        {
            droidBullet.SetStraightVelocity(walkDirectionVector);
        }
        else
        {
            // 3. If it is the Player's bullet (used by the Ally), apply velocity directly to its Rigidbody
            Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Adjust the '10f' to match whatever your normal player bullet speed is
                rb.linearVelocity = walkDirectionVector * 10f;
            }
        }
        audioSource.PlayOneShot(gunShot);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // The friendly droid is tagged Player, unlike the regular enemies. Find the
        // real player by its movement component (not by tag, which would match us).
        isFriendly = CompareTag("Player");
        if (isFriendly)
        {
            foreach (PlayerMovementState p in FindObjectsByType<PlayerMovementState>(FindObjectsSortMode.None))
            {
                if (p.gameObject == gameObject || !p.CompareTag("Player")) continue;
                playerTransform = p.transform;
                playerSprite = p.GetComponent<SpriteRenderer>();
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isFriendly)
            FollowPlayerAndFace();
        else
            rb.linearVelocity = new Vector2(walkDirectionVector.x * walkSpeed, rb.linearVelocity.y);

        animator.SetBool("isMoving", Mathf.Abs(rb.linearVelocity.x) > 0.01f);
        UpdateHover();
    }

    /// <summary>
    /// Friendly droid trails the player and mirrors the direction the player's
    /// sprite faces on the X axis. Vertical movement stays with UpdateHover().
    /// </summary>
    private void FollowPlayerAndFace()
    {
        if (playerTransform == null) return;

        bool playerFacesLeft = playerSprite != null && playerSprite.flipX;
        float behind = playerFacesLeft ? 1f : -1f;
        float targetX = playerTransform.position.x + behind * followDistance;
        float dx = targetX - rb.position.x;
        float vx = Mathf.Abs(dx) <= followStopDistance
            ? 0f
            : Mathf.Clamp(dx * followResponsiveness, -followMaxSpeed, followMaxSpeed);
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

        if (spriteRenderer == null) return;
        float facing = playerFacesLeft ? -1f : 1f;
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * facing;
        transform.localScale = s;
    }

    /// <summary>
    /// Keeps the droid just above whatever ground is beneath it instead of
    /// drifting around at a fixed high altitude. Gravity stays off (the Rigidbody2D
    /// has gravityScale 0), so the vertical position is driven here. Only the
    /// visual hover changes; walking/detection/shooting are untouched.
    /// </summary>
    private void UpdateHover()
    {
        if (!hoverEnabled || rb == null || bodyCollider == null) return;

        // Distance from the transform origin down to the bottom of the collider,
        // in world units (the sprite/collider are scaled up on the transform).
        Bounds bounds = bodyCollider.bounds;
        float bottomOffsetY = (bounds.center.y - transform.position.y) - bounds.extents.y;

        // Look for the closest ground surface below the droid, ignoring its own body.
        Vector2 origin = new Vector2(rb.position.x, rb.position.y + bottomOffsetY - 0.05f);
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, groundProbeDistance);
        float groundY = float.NegativeInfinity;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == bodyCollider) continue;
            if (hit.collider.CompareTag("Ground") && hit.point.y > groundY)
                groundY = hit.point.y;
        }

        float bob = Mathf.Sin(Time.time * hoverBobFrequency) * hoverBobAmplitude;
        float targetY;

        if (isFriendly && playerTransform != null)
        {
            // Follow the player vertically too — in this 2D project "the same level"
            // is the Y axis. Never sink into the ground below.
            targetY = playerTransform.position.y + verticalFollowOffset + bob;
            if (!float.IsNegativeInfinity(groundY))
                targetY = Mathf.Max(targetY, groundY + hoverHeight - bottomOffsetY + bob);

            // If the player is clearly higher, kick off a short rocket boost from
            // underneath so the droid visibly rises instead of teleporting up.
            if (playerTransform.position.y > rb.position.y + rocketBoostThreshold && boostTimer <= 0f)
            {
                boostTimer = rocketBoostDuration;
                SpawnRocketBoost();
            }

            if (boostTimer > 0f)
            {
                boostTimer -= Time.fixedDeltaTime;
                // Move by position, not by Rigidbody velocity: the droid starts almost
                // touching the floor, and the physics solver would otherwise cancel
                // the upward velocity against the ground collider every step.
                float boostedY = Mathf.MoveTowards(rb.position.y, targetY,
                    rocketBoostSpeed * Time.fixedDeltaTime);
                rb.position = new Vector2(rb.position.x, boostedY);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                if (boostedY >= targetY) boostTimer = 0f; // reached the player's level
                return;
            }
        }
        else
        {
            if (float.IsNegativeInfinity(groundY)) return; // over a gap: keep current altitude
            targetY = groundY + hoverHeight - bottomOffsetY + bob;
        }

        float newY = Mathf.MoveTowards(rb.position.y, targetY, hoverSpeed * Time.fixedDeltaTime);
        rb.position = new Vector2(rb.position.x, newY);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    /// <summary>Short downward exhaust puff spawned directly under the droid.</summary>
    private void SpawnRocketBoost()
    {
        GameObject fx = new GameObject("RocketBoostFX");
        fx.transform.position = new Vector3(rb.position.x, bodyCollider.bounds.min.y, 0f);

        ParticleSystem ps = fx.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.duration = 0.35f;
        main.loop = false;
        main.startLifetime = 0.3f;
        main.startSpeed = 3.5f;
        main.startSize = 0.22f;
        main.startColor = new Color(1f, 0.6f, 0.15f, 1f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 80f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 12) });

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.15f;
        shape.rotation = new Vector3(180f, 0f, 0f); // exhaust fires downward

        ParticleSystemRenderer renderer = fx.GetComponent<ParticleSystemRenderer>();
        Material mat = Resources.GetBuiltinResource<Material>("Default-Particle.mat");
        if (mat == null)
        {
            Shader shader = Shader.Find("Particles/Standard Unlit");
            if (shader != null) mat = new Material(shader);
        }
        if (mat != null) renderer.material = mat;

        ps.Play();
        Destroy(fx, 0.8f);
    }

    public void Shoot() => animator.SetTrigger("shoot");

    public void Die()
    {
        animator.SetTrigger("dead");
        GetComponent<Collider2D>().enabled = false;
    }


    public void DestroyBot()
    {
        Destroy(gameObject);
    }

    public void TakeHit(float damageRecieved)
    {
        animator.SetTrigger("hurt");
        currentHealth -= damageRecieved;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        WalkDirection = WalkableDirection.Left;

        GetDistanceFromGround();


    }






    // Update is called once per frame
    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }

    }
}