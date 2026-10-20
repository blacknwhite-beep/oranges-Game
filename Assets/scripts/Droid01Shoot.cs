using UnityEngine;

public class Droid01Shoot : MonoBehaviour
{
    private Animator anim;
    private Droid01Movement movement;
    private Droid01Health health;


    [Header("Shoot Zone")]
    public DetectionZone shootZone;   // can be the same DetectionZone as movement, or a separate one

    [Header("Shooting")]
    public float shootCooldown = 2f;
    private float nextShootTime;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<Droid01Movement>();
        health = GetComponent<Droid01Health>();

    }

    // Update is called once per frame
    void Update()
    {
        if (shootZone == null)
        {
            Debug.LogWarning("shootZone not assigned"); return;
        }

        if (health != null && health.IsDead) return;

        if (shootZone.PlayerDetected && Time.time >= nextShootTime)
        {
            anim.SetTrigger("shoot");
            nextShootTime = Time.time + shootCooldown;
        }
    }

    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;
        if (health != null && health.IsDead) return;

        int dir = movement != null ? movement.FacingDir : -1;

        // firePoint is a child, so mirror its X offset when the droid turns around
        Vector3 offset = new Vector3(Mathf.Abs(firePoint.localPosition.x) * dir, firePoint.localPosition.y, 0f);
        GameObject p = Instantiate(projectilePrefab, transform.position + offset, Quaternion.identity);
        p.GetComponent<DroidProjectile>().SetStraightVelocity(new Vector2(dir, 0f));
    }
}
