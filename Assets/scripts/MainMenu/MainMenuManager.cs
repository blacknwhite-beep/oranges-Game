using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame(string stageName)
    {
        // Load the specified level of the game
        SceneManager.LoadScene(stageName);
    }
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
