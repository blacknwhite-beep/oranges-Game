using UnityEngine;

public class CheckPointScript : MonoBehaviour
{

    private GameOverTriggerScript gameOverScript;
    private BoxCollider2D boxCollider;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverScript = GameObject.FindGameObjectWithTag("GameOverScript").GetComponent<GameOverTriggerScript>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Checkpoint Triggered");
        
        if (collision.gameObject.layer == 3 )
        {
            Debug.Log("Checkpoint Updated");
            gameOverScript.updatedSpawnPoint = this.gameObject;
            boxCollider.enabled = false; // Disable the collider so you cant re trigger an early checkpoint
        }
    }
}
