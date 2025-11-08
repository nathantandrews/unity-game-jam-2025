using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }    
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
    public void StartLevel(int levelId)
    {
        return;
    }
    public void EndLevel(bool passedLevel)
    {
        return;
    }

}
