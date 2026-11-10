using UnityEngine;

public class Droid01Shoot : MonoBehaviour
{
    private Animator anim;
    private DroidEnemy_01 movement;
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
        movement = GetComponent<DroidEnemy_01>();
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

   
}
