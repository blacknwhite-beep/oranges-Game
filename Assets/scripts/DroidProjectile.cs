using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroidProjectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float normalBulletSpeed = 10f;
    [SerializeField] private float DestroyTime = 3f;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f;
        col = GetComponent<Collider2D>();

    }

    void Start()
    {
        

        SetDestroyTime();

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void SetDestroyTime()
    {
        Destroy(gameObject, DestroyTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb.linearVelocity = Vector2.zero;
            col.enabled = false;
            CancelInvoke();
            animator.SetTrigger("isHit");
        }
    }

    public void OnImpactFinished()
    {
        Destroy(gameObject);
    }

    public void SetStraightVelocity(Vector2 direction)
    {
        rb.linearVelocity = direction * normalBulletSpeed;
        if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        Debug.Log($"Velocity set to: {rb.linearVelocity}, rb null? {rb == null}");
    }



}
