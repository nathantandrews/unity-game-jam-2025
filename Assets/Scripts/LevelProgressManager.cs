using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance { get; private set; }

    private const string ProgressKey = "HighestLevelUnlocked";
    public int HighestLevelUnlocked { get; private set; } = 1;

    void Awake()
    {
        // Singleton pattern (keeps one instance between scenes)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    public void UnlockNextLevel(int justCompletedLevel)
    {
        int nextLevel = justCompletedLevel + 1;
        if (nextLevel > HighestLevelUnlocked)
        {
            HighestLevelUnlocked = nextLevel;
            SaveProgress();
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(ProgressKey, HighestLevelUnlocked);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        HighestLevelUnlocked = PlayerPrefs.GetInt(ProgressKey, 1);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(ProgressKey);
        HighestLevelUnlocked = 1;
    }
}
