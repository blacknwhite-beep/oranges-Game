using UnityEngine;

public class Droid03Attack : MonoBehaviour
{
    private Animator anim;
    private Droid03Movement movement;
    private Droid03Health health;
    private string currentState;

    


    [Header("Shoot Zone")]
    public DetectionZone shootZone;   // can be the same DetectionZone as movement, or a separate one

    [Header("Shooting")]
    public float shootCooldown = 2f;
    public float projectileSpeed = 6f;
    private float nextShootTime;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    void ChangeState(string newState)
    {
        if (currentState == newState) return;   // already playing it
        anim.Play(newState);
        currentState = newState;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<Droid03Movement>();
        health = GetComponent<Droid03Health>();

    }

    private Transform attackTarget;   // add this with your other fields

    void Update()
    {
        if (shootZone == null) { Debug.LogWarning("shootZone not assigned"); return; }
        if (health != null && health.IsDead) return;

        if (shootZone.PlayerDetected && shootZone.Target != null && Time.time >= nextShootTime)
        {
            StartAttack(shootZone.Target);
        }
    }

    void StartAttack(Transform target)
    {
        nextShootTime = Time.time + shootCooldown;
        attackTarget = target;                 // remember who we're shooting at
        anim.Play("Droid03Attack", 0, 0f);     // just play the animation, no bullet yet
    }

    // Called by the Animation Event, so it must be PUBLIC
    public void FireProjectile()
    {
        if (health != null && health.IsDead) return;

        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile Prefab or Fire Point not assigned");
            return;
        }

        // Aim at the target if we still have one, otherwise shoot the way we're facing
        float dir;
        dir = GetComponent<SpriteRenderer>().flipX ? 1f : -1f;
        if (attackTarget != null)
            dir = Mathf.Sign(attackTarget.position.x - firePoint.position.x);
        else
            dir = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;

        GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
        if (prb != null) prb.linearVelocity = new Vector2(dir * projectileSpeed, 0f);

        SpriteRenderer psr = p.GetComponent<SpriteRenderer>();
        if (psr != null) psr.flipX = dir < 0;
    }
}
