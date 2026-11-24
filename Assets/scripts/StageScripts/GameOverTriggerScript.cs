using UnityEngine;

public class GameOverTriggerScript : MonoBehaviour
{
    public LogicManager logic;
    public GameObject spawnPoint;
    public GameObject updatedSpawnPoint = null;

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
            updatedSpawnPoint = null; // Reset the updated spawn point when game is over
            logic.gameOver();
            Debug.Log("Logic.gameOver() called"); 

        }
        else if (collision.gameObject.layer == 3 && logic.playerLives > 0)
        {
            Debug.Log("Player Respawn Triggered");
            logic.updateLives(1);

            if (updatedSpawnPoint != null)
            {
                logic.player.transform.position = updatedSpawnPoint.transform.position;  //spawn here if the spawn point has been updated
            }
            else
            {
                logic.player.transform.position = spawnPoint.transform.position; //spawn here by default or if spawn point has not been updated
            }

            Debug.Log("Player Respawned at Spawn Point");
        }

    }
}
