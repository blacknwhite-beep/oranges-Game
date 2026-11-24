using UnityEngine;
using System;
public class PlayerMovementState : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Run,
        Jump,
        DoubleJump,
        Attack1,
        Attack2,
        Attack3,
        Hurt,
        Death,
        Climb,
        Push
    }
    public MovementState currentMovementState { get; private set; }
    
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidBody;
    public bool isAttacking = false;
    private float xPosLastFrame;

    // FinishAttack / FinishHurt are animation events. They never fire if another
    // animator.Play() interrupts the clip (for example jumping while attacking or
    // landing on an enemy mid-swing), which used to strand the player in the Jump
    // (falling) pose forever because Update() early-returned on isAttacking.
    // These watchdogs guarantee the transient states always expire.
    private const float attackStateTimeout = 0.85f; // longest attack clip is ~0.67 s
    private const float hurtStateTimeout = 0.45f;   // Hurt clip is 0.25 s
    private float transientStateStart;

    private void Update()
    {
        if (currentMovementState == MovementState.Death) return;

        // Recover from interrupted attack / hurt animations before applying the
        // "don't change state while busy" guard.
        if (isAttacking && Time.time - transientStateStart > attackStateTimeout)
        {
            isAttacking = false;
        }
        if (currentMovementState == MovementState.Hurt && Time.time - transientStateStart > hurtStateTimeout)
        {
            FinishHurt();
        }

        if (isAttacking || currentMovementState == MovementState.Hurt) return; // Prevents movement state changes while attacking or hurt
        float input = Input.GetAxisRaw("Horizontal");

        //Debug.Log($"Input: {input} | Y-Velocity: {rigidBody.linearVelocity.y} | State: {currentMovementState}"); // debugging for movement state transitions

        if (Mathf.Abs(rigidBody.linearVelocity.y) < 0.5f)
        {
            if (input != 0)
            {
                SetMoveState(MovementState.Run);
            }
            else
            {
                SetMoveState(MovementState.Idle);
            }
        }
        

    }
    private const string idleState = "Idle";
    private const string runState = "Run";
    private const string jumpState = "Jump";
    private const string doubleJumpState = "DoubleJump";
    private const string attack1State = "Attack1";
    private const string attack2State = "Attack2";
    private const string attack3State = "Attack3";
    private const string hurtState = "Hurt";
    private const string deathState = "Death";
    private const string climbState = "Climb";
    private const string pushState = "Push";
    public static Action<MovementState> OnPlayerMovementStateChanged;
    public void FinishAttack()
    {
        isAttacking = false;
        SetMoveState(MovementState.Idle);
    }
    public void SetMoveState(MovementState moveState)
    {
        if (currentMovementState == moveState) return;
        Debug.Log($"State changing from {currentMovementState} to {moveState}");
        switch (moveState)
        {
            case MovementState.Idle:
                HandleIdle();
                break;
            case MovementState.Run:
                HandleRun();
                break;
            case MovementState.Jump:
                HandleJump();
                break;
            case MovementState.DoubleJump:
                HandleDoubleJump();
                break;
            case MovementState.Attack1:
                HandleAttack1();
                break;
            case MovementState.Attack2:
                HandleAttack2();
                break;
            case MovementState.Attack3:
                HandleAttack3();
                break;
            case MovementState.Hurt:
                HandleHurt();
                break;
            case MovementState.Death:
                HandleDeath();
                break;
            case MovementState.Climb:
                HandleClimb();
                break;
            case MovementState.Push:
                HandlePush();
                break;
            default:
                Debug.LogWarning($"Unhandled movement state: {moveState}");
                break;
        }
        OnPlayerMovementStateChanged?.Invoke(moveState);
        currentMovementState = moveState;
    }
    public void HandleIdle()
    {
        currentMovementState = MovementState.Idle;
        animator.Play(idleState);   
    }
    public void HandleRun()
    {
        if (currentMovementState == MovementState.Hurt || currentMovementState == MovementState.Death)
        {
            return;
        }
        currentMovementState = MovementState.Run;
        animator.Play(runState);
    }
    public void HandleJump()
    {
        // A jump interrupts any in-flight attack, so its FinishAttack event will
        // not fire. Drop the stale lock now instead of getting stuck airborne.
        isAttacking = false;
        currentMovementState = MovementState.Jump;
        animator.Play(jumpState);
    }
    public void HandleDoubleJump()
    {
        isAttacking = false;
        currentMovementState = MovementState.DoubleJump;
        animator.Play(doubleJumpState);
    }
    public void HandleAttack1()
    {
        isAttacking = true;
        transientStateStart = Time.time;
        currentMovementState = MovementState.Attack1;
        animator.Play(attack1State);
    }
    public void HandleAttack2()
    {
        isAttacking = true;
        transientStateStart = Time.time;
        currentMovementState = MovementState.Attack2;
        animator.Play(attack2State);
    }
    public void HandleAttack3()
    {
        isAttacking = true;
        transientStateStart = Time.time;
        currentMovementState = MovementState.Attack3;
        animator.Play(attack3State);
    }
    public void HandleHurt()
    {
        Debug.Log("Player is hurt!");
        // If damage interrupts an attack, its FinishAttack animation event will
        // not run. Clear the flag so the player is not locked out of attacking.
        isAttacking = false;
        transientStateStart = Time.time;
        currentMovementState = MovementState.Hurt;
        animator.Play(hurtState);
    }
    public void HandleDeath()
    {
        currentMovementState = MovementState.Death;
        animator.Play(deathState);

        // Optional: Stop the player's physical movement so they don't slide while dying
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        LogicManager logic = FindAnyObjectByType<LogicManager>();
        if (logic != null)
        {
            logic.Invoke("gameOver", 1.5f);
        }
        Destroy(gameObject, 1.5f);
    }
    public void HandleClimb()
    {
        currentMovementState = MovementState.Climb;
        animator.Play(climbState);
    }
    public void HandlePush()
    {
        currentMovementState = MovementState.Push;
        animator.Play(pushState);
    }
    public void FinishHurt()
    {
        SetMoveState(MovementState.Idle);
    }

    /// <summary>
    /// Called by playerJump whenever the player physically lands on any solid
    /// surface, including enemies (which are not tagged "Ground"). Only touches
    /// airborne states so a genuine attack or hurt animation is never cut short.
    /// </summary>
    public void NotifyLanded()
    {
        if (currentMovementState == MovementState.Death) return;
        if (currentMovementState != MovementState.Jump && currentMovementState != MovementState.DoubleJump) return;

        // Clear a stale attack lock left by an interrupted swing.
        isAttacking = false;
        bool moving = rigidBody != null && Mathf.Abs(rigidBody.linearVelocity.x) > 0.1f;
        SetMoveState(moving ? MovementState.Run : MovementState.Idle);
    }

}
