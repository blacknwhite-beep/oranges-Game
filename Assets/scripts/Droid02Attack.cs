using UnityEngine;

public class Droid02Attack : MonoBehaviour
{
    private Animator anim;

    [Header("Attack Zone")]
    public DetectionZone attackZone;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    public int damage = 1;
    private float nextAttackTime;

    [Header("Effect")]
    public GameObject explosionPrefab;
    public Transform firepoint;



    public void SpawnExplosion()
    {
        if (explosionPrefab == null || firepoint == null) return;
        GameObject fx = Instantiate(explosionPrefab, firepoint.position, firepoint.rotation);
        Destroy(fx, 1f); // roughly the clip length
        DealDamage();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackZone == null) { Debug.LogWarning("attackZone not assigned"); return; }

        if (attackZone.PlayerDetected && Time.time >= nextAttackTime)
        {
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (!attackZone.PlayerDetected)
        {
            anim.ResetTrigger("Attack");
        }
    }

    private void DealDamage()
    {
        // Only hit if the player is still in range when the attack lands
        if (!attackZone.PlayerDetected || attackZone.Target == null) return;

        PlayerHealth playerHealth = attackZone.Target.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damage);
    }
}
