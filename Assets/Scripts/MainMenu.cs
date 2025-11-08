using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This function loads the Level Select scene
    public void GoToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect"); // The name must match your scene name!
    }

    public void GoToOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;  // Stop play mode if in editor
        #else
                Application.Quit(); // Quit the app if built
        #endif
    }
}
