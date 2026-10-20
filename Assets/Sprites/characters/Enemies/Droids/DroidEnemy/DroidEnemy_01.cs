using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Droid01Movement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Droid01Health health;
    public LayerMask groundLayer;
    private float lostTimer;
    

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float stopDistance = 1f;
    public float loseTargetDelay = 0.75f;
    private Transform lastTarget;

    [Header("Detection")]
    public DetectionZone detectionZone;

    // 1 = facing right, -1 = facing left. Droid01Shoot reads this to fire the right way.
    public int FacingDir { get; private set; } = -1;

    
    




    // in FixedUpdate, replacing the Y part of your velocity:






    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<Droid01Health>();
    }

    private bool PlayerInZone => detectionZone != null && detectionZone.PlayerDetected;

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (health != null && health.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetBool("isMoving", false);
            return;
        }

        if (detectionZone.PlayerDetected && detectionZone.Target)
        {
            lastTarget = detectionZone.Target;
            lostTimer = loseTargetDelay;
            Debug.Log($"Switching to target: {detectionZone.Target.name}");
        }
        else
        {
            lostTimer -= Time.fixedDeltaTime;
            Debug.Log($"PlayerDetected: {detectionZone.PlayerDetected}. detectZoneTarget: {detectionZone.Target}");
        }

        bool shouldMove = false;

        if (lostTimer > 0f && lastTarget != null)
        {
            float dx = lastTarget.position.x - transform.position.x;

            // Always face the player, even when standing still to shoot
            if (Mathf.Abs(dx) > 0.01f)
            {
                FacingDir = (int)Mathf.Sign(dx);
                sr.flipX = FacingDir > 0; // sprite faces LEFT by default (change to < 0 if yours faces right)
            }

            if (Mathf.Abs(dx) > stopDistance)
            {
                shouldMove = true;
                rb.linearVelocity = new Vector2(FacingDir * walkSpeed, rb.linearVelocity.y);
            }
        }

        if (!shouldMove)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        anim.SetBool("isMoving", shouldMove);

        


    }

    internal void TakeHit(float damageToDeal)
    {
        throw new NotImplementedException();
    }
}