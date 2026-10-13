using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame(string stageName)
    {
        // Load the specified level of the game
        SceneManager.LoadScene(stageName);
    }
    public void Settings()
    {
        // Load the settings scene
        SceneManager.LoadScene("Settings");
    }
    public void BackToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Main_Menu");
    }
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
