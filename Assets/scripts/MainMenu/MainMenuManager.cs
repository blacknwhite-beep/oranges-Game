using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load the specified level of the game
        SceneManager.LoadSceneAsync(1);
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
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
