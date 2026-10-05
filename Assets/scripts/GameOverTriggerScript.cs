using UnityEngine;

public class GameOverTriggerScript : MonoBehaviour
{
    public LogicManager logic;
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
        if (collision.gameObject.layer == 3)
        {
            
            logic.gameOver();

        }

    }
}
