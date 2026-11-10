using UnityEngine;

public class Droid02Health : MonoBehaviour
{
    private Animator anim;

    [Header("Health")]
    public float currentHealth = 50f;

    public bool IsDead { get; private set; }

    public void Die()
    {
        IsDead = true;
        anim.SetBool("isMoving", false);
        anim.SetTrigger("Death");
    }

    public void TakeHit(float damageReceived)
    {
        if (IsDead) return;

        anim.SetTrigger("Hurt");
        currentHealth -= damageReceived;
        if (currentHealth <= 0f) Die();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
