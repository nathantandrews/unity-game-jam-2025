using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public string sceneName;
        public Button button;
        public int levelNumber;
    }

    public LevelButton[] levelButtons;

    void Start()
    {
        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        int highestUnlocked = LevelProgressManager.Instance.HighestLevelUnlocked;

        foreach (LevelButton lb in levelButtons)
        {
            bool unlocked = lb.levelNumber <= highestUnlocked;
            lb.button.interactable = unlocked;

            // Optional: visually indicate locked levels
            var text = lb.button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
            {
                text.text = unlocked ? $"Level {lb.levelNumber}" : $"Locked";
            }
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("HomePage");
    }
}
