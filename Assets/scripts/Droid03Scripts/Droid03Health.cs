using UnityEngine;

public class Droid03Health : MonoBehaviour
{
    private Animator anim;
    private string currentState;

    [Header("Health")]
    public float currentHealth = 50f;

    public bool IsDead { get; private set; }

    void ChangeState(string newState)
    {
        if (currentState == newState) return;   // already playing it
        anim.Play(newState);
        currentState = newState;
    }

    public void Die()
    {
        IsDead = true;
        ChangeState("Droid03Death");
    }

    public void TakeHit(float damageReceived)
    {
        if (IsDead) return;

        ChangeState("Droid03Hurt");
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
