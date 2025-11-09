using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance { get; private set; }

    public int HighestLevelUnlocked { get; private set; } = 0; // Always start at Level 0

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockNextLevel(int justCompletedLevel)
    {
        int nextLevel = justCompletedLevel + 1;
        if (nextLevel > HighestLevelUnlocked)
        {
            HighestLevelUnlocked = nextLevel;
        }
    }

    public void ResetProgress()
    {
        HighestLevelUnlocked = 0;
    }
}
