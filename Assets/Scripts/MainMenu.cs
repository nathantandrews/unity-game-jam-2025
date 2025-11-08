using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This function loads the Level Select scene
    public void GoToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect"); // The name must match your scene name!
    }
}
