using UnityEngine;
using TMPro; // for TextMeshPro

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 60f; // Starting time in seconds
    private float currentTime;

    [Header("Speed Settings")]
    public float normalSpeed = 1f;
    public float movingSpeed = 2f;

    [Header("UI Reference")]
    public TextMeshProUGUI timerText; // Drag your TimerText object here in Inspector

    private bool isPlayerMoving = false;

    void Start()
    {
        currentTime = startTime;

        // Optional safety check
        if (timerText == null)
        {
            Debug.LogWarning("TimerText not assigned in GameTimer script!");
        }
    }

    void Update()
    {
        if (currentTime <= 0)
            return;

        // Determine timer speed
        float speed = isPlayerMoving ? movingSpeed : normalSpeed;

        // Decrease time
        currentTime -= Time.deltaTime * speed;
        currentTime = Mathf.Max(currentTime, 0f);

        // Display only whole seconds (no decimals)
        int displayTime = Mathf.CeilToInt(currentTime);
        if (timerText != null)
        {
            timerText.text = displayTime.ToString();
        }
    }

    // Called by PlayerScript
    public void SetPlayerMoving(bool moving)
    {
        isPlayerMoving = moving;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}
