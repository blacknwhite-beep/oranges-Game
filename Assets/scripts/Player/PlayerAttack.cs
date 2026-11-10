using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovementState moveState;
    [Header("Combo Settings")]
    private int comboStep = 1;
    private float lastAttackTime = 0f;
    private float comboResetTime = 2.5f; // Time in seconds to reset the combo if no further attacks are made

    [Header ("Attack Settings")]
    [SerializeField] private float attack1Damage = 10f;
    [SerializeField] private float attack2Damage = 15f;
    [SerializeField] private float attack3Damage = 20f;
    [Header("AoE Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    private SpriteRenderer spriteRenderer;



    private void Awake()
    {
        moveState = GetComponent<PlayerMovementState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //reset the combo if not attacking
        if (Time.time - lastAttackTime > comboResetTime && !moveState.isAttacking)
        {
            comboStep = 1;
        }
        //if clicking and not midswing
        if (Input.GetButtonDown("Fire1") && !moveState.isAttacking)
        {
            lastAttackTime = Time.time;
            if (comboStep == 1)
            {
                moveState.SetMoveState(PlayerMovementState.MovementState.Attack1);
                comboStep = 2;
            }
            else if (comboStep ==2)
            {
                moveState.SetMoveState(PlayerMovementState.MovementState.Attack2);
                comboStep = 3;
            }
            else if (comboStep == 3)
            {
                moveState.SetMoveState(PlayerMovementState.MovementState.Attack3);
                comboStep = 1; // Reset to the first attack after the third
            }
        }
    }
    public void ExecuteDamage()
    {
        Debug.Log("ExecuteDamage was triggered!");
        if (attackPoint == null)     return;
        //to decide which attack damage to use based on the current combo step
        float damageToDeal = attack1Damage; // Default to attack1Damage
        if (moveState.currentMovementState == PlayerMovementState.MovementState.Attack2)
        {
            damageToDeal = attack2Damage;
        }
        else if (moveState.currentMovementState == PlayerMovementState.MovementState.Attack3)
        {
            damageToDeal = attack3Damage;
        }
        //flip the hit circle to the left if the player is facing left
        Vector2 hitCenter = attackPoint.position;
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            float xOffset = attackPoint.localPosition.x;
            hitCenter = new Vector2(transform.position.x - xOffset, attackPoint.position.y);
        }
        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitCenter, attackRange, enemyLayers);
        Debug.Log($"Hit check complete. Found {hitEnemies.Length} objects inside the red circle."); // Debugging line to check how many enemies were hit
        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage to the enemy
            DroidEnemy_01 droid = enemy.GetComponent<DroidEnemy_01>();
            if (droid != null)
            {
                droid.TakeHit(damageToDeal);
                Debug.Log($"Hit {enemy.name} for {damageToDeal} damage.");
            }
        }
    }
    //draws a red circle in the scene view to see the attack range
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Vector3 drawCenter = attackPoint.position;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.flipX)
        {
            drawCenter = new Vector3(transform.position.x - attackPoint.localPosition.x, attackPoint.position.y, 0);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(drawCenter, attackRange);
    }
}

