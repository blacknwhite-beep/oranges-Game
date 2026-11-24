using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogicManager : MonoBehaviour
{
    public GameObject player;
    public int playerLives = 3;
    public Text livesText;
    public GameObject gameOverScreen;
    public GameObject youWinScreen;
    public GameObject livesUi;
    

    

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    



    [ContextMenu("Lives Left")]
    public void updateLives(int livesLost)
    {
        playerLives -= livesLost;
        livesText.text = playerLives.ToString();
    }

    [ContextMenu("RestartGame")]
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void youWin()
    {
        Debug.Log("You Win Triggered");
        youWinScreen.SetActive(true);
    }


    
}
