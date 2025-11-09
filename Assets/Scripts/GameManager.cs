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
    public GameTimer gameTimer = null;

     [Header("Audio Settings")]
    [Range(0f, 1f)] public float startVolume = 0.3f; // Default quieter start volume (30%)

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        AudioListener.volume = startVolume;
    }

    void Start()
    {
        HideAllPanels();
        ResumeGame();
    }

    void Update()
    {

        // Detect which scene we’re currently in
        string currentScene = SceneManager.GetActiveScene().name;

        // === MAIN MENU ESC HANDLER ===
        if (currentScene == "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitGame();
            }
        }

        // === Universal Key Shortcuts ===
        if (!isGameOver)
        {
            // ESC → Go back to Level Select
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoToLevelSelect();
            }

            // R → Restart current level
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }

            // Timer check for death
            if (gameTimer != null && gameTimer.GetCurrentTime() <= 0)
            {
                EndLevel(false);
            }
        }
        else
        {
            // SPACE → Go to next level (only if victorious)
            if (Input.GetKeyDown(KeyCode.Space) && victoryPanel != null && victoryPanel.activeSelf)
            {
                GoToNextLevel();
            }

            // ESC → Back to Level Select even after win/lose
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoToLevelSelect();
            }

            // R → Restart even after death
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
        }
    }

    // ------------------- Scene Navigation -------------------
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
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

    public void GoToControls()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("ControlsPage");
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

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void GoToNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("No more levels available!");
            GoToLevelSelect();
        }
    }

    // ------------------- Game Logic -------------------
    public void EndLevel(bool passedLevel)
    {
        if (isGameOver) return;

        isGameOver = true;
        FreezeGame();

        if (passedLevel)
        {

            SoundManager.Instance?.PlayGoalReached();
            // Get current level number
            string currentScene = SceneManager.GetActiveScene().name;
            int currentLevel = ParseLevelNumber(currentScene);

            // ✅ Unlock next level
            if (LevelProgressManager.Instance != null)
            {
                LevelProgressManager.Instance.UnlockNextLevel(currentLevel);
            }

            ShowVictoryPanel();
        }
        else
        {
            SoundManager.Instance?.PlayPlayerDeath();
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

    private int ParseLevelNumber(string sceneName)
    {
        if (sceneName.StartsWith("Level"))
        {
            string numberPart = sceneName.Substring(5);
            if (int.TryParse(numberPart, out int result))
                return result;
        }
        return 1; // fallback
    }
}
