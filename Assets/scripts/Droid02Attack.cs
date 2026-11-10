using UnityEngine;

public class Droid02Attack : MonoBehaviour
{
    private Animator anim;

    [Header("Attack Zone")]
    public DetectionZone attackZone;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    private float nextAttackTime;

    [Header("Effect")]
    public GameObject explosionPrefab;
    public Transform firepoint;



    public void SpawnExplosion()
    {
        if (explosionPrefab == null || firepoint == null) return;
        GameObject fx = Instantiate(explosionPrefab, firepoint.position, firepoint.rotation);
        Destroy(fx, 1f); // roughly the clip length
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackZone.PlayerDetected && Time.time >= nextAttackTime)
        {
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }

        if (attackZone == null) { Debug.LogWarning("attackZone not assigned"); return; }

        if (attackZone.PlayerDetected)
            Debug.Log("Player in attack zone");

        if (attackZone.PlayerDetected && Time.time >= nextAttackTime)
        {
            Debug.Log("Attack trigger set");
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
