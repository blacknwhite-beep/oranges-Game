using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load the specified level of the game
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        // Quit the application
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
