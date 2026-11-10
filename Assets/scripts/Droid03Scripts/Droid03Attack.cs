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
            ChangeState("Droid03Attack");
        }

    }
}
