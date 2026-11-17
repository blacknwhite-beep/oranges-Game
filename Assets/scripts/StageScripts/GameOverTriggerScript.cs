using UnityEngine;

public class GameOverTriggerScript : MonoBehaviour
{
    public LogicManager logic;
    public GameObject spawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("LogicManager").GetComponent<LogicManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Game Over Triggered");
        if (collision.gameObject.layer == 3 && logic.playerLives <= 0)
        {
            Debug.Log("Game Over Triggered");
            logic.gameOver();
            Debug.Log("Logic.gameOver() called"); 

        }
        else if (collision.gameObject.layer == 3 && logic.playerLives > 0)
        {
            Debug.Log("Player Respawn Triggered");
            logic.updateLives(1);

            logic.player.transform.position = spawnPoint.transform.position;
            Debug.Log("Player Respawned at Spawn Point");
        }

    }
}
