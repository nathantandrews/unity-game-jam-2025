using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject victoryPanel = null;
    public GameObject deathPanel = null;

    [Header("References")]
    public GameTimer gameTimer = null; // Assign your GameTimer script here

    private bool isGameOver = false;

    void Awake()
    {
        // Simple singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        HideAllPanels();
        ResumeGame(); // make sure time scale is normal
    }

    void Update()
    {
        // Escape key: go back to Main Menu directly
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }

        // If timer hits zero and the game isn't already over
        if (gameTimer != null && gameTimer.GetCurrentTime() <= 0 && !isGameOver)
        {
            EndLevel(false);
        }
    }

    // ------------------- Scene Navigation -------------------
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // reset just in case
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void GoToOptions()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Options");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void StartLevel(int levelId)
    {
        SceneManager.LoadScene("Level" + levelId);
    }
    

    // ------------------- Game Logic -------------------
    public void EndLevel(bool passedLevel)
    {
        if (isGameOver) return;

        isGameOver = true;
        FreezeGame();

        if (passedLevel)
        {
            ShowVictoryPanel();
        }
        else
        {
            ShowDeathPanel();
        }
    }

    private void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        HideAllPanels();
    }

    private void HideAllPanels()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (deathPanel != null) deathPanel.SetActive(false);
    }

    private void ShowVictoryPanel()
    {
        HideAllPanels();
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    private void ShowDeathPanel()
    {
        HideAllPanels();
        if (deathPanel != null) deathPanel.SetActive(true);
    }


    public void RestartLevel()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            // Optional: Go back to main menu if it's the last level
            GoToMainMenu();
        }
    }
}
