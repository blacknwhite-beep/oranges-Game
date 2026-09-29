using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovementState moveState;
    private int comboStep = 1;
    private float lastAttackTime = 0f;
    private float comboResetTime = 2.5f; // Time in seconds to reset the combo if no further attacks are made
    private void Awake()
    {
        moveState = GetComponent<PlayerMovementState>();
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
}
