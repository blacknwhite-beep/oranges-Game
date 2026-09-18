using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D contactFilter;
    CapsuleCollider2D touchingCol;
    Animator animator;

    RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    [SerializeField]
    private bool isTouchingHorizontal;
    [SerializeField]
    private bool isTouchingVertical;

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}
