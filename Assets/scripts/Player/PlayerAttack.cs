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
    [Header("Hit Feedback")]
    [SerializeField] private float hitStopDuration = 0.12f;
    [SerializeField, Range(0f, 1f)] private float comboHitVolume = 1f;
    [Header("AoE Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private AudioClip[] comboHitClips;



    private void Awake()
    {
        moveState = GetComponent<PlayerMovementState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        comboHitClips = new AudioClip[3];
        for (int i = 0; i < comboHitClips.Length; i++)
        {
            comboHitClips[i] = Resources.Load<AudioClip>($"Audio/Player/ComboHit_{i + 1}");
        }
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
        // Detect targets in range; the component check below filters actual enemies.
        // Some enemy prefabs use the Default layer rather than the Enemy layer.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitCenter, attackRange);
        Debug.Log($"Hit check complete at {hitCenter}. Found {hitEnemies.Length} objects inside the red circle.");
        bool damageDelivered = false;
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"[PlayerAttack] Overlap with '{enemy.name}' on layer {enemy.gameObject.layer}.");
            // Apply damage to the enemy
            DroidEnemy_01 droid = enemy.GetComponent<DroidEnemy_01>();
            if (droid != null)
            {
                droid.TakeHit(damageToDeal);
                damageDelivered = true;
                Debug.Log($"Hit {enemy.name} for {damageToDeal} damage.");
            }
        }

        if (damageDelivered)
        {
            HitStop.Request(hitStopDuration);

            int clipIndex = 0;
            if (moveState.currentMovementState == PlayerMovementState.MovementState.Attack2)
            {
                clipIndex = 1;
            }
            else if (moveState.currentMovementState == PlayerMovementState.MovementState.Attack3)
            {
                clipIndex = 2;
            }

            AudioClip hitClip = comboHitClips[clipIndex];
            if (hitClip != null)
            {
                audioSource.PlayOneShot(hitClip, comboHitVolume);
            }
            else
            {
                Debug.LogWarning($"Missing combo hit audio at Resources/Audio/Player/ComboHit_{clipIndex + 1}.wav");
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

