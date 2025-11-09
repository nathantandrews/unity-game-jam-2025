using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Music Clips")]
    public AudioClip mainTheme;
    public AudioClip levelTheme;

    private bool isLevelScene = false;
    private GameTimer gameTimer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicSource.volume = savedVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name.ToLower();
        isLevelScene = sceneName.StartsWith("level");

        if (isLevelScene)
        {
            PlayMusic(levelTheme);
            // Try to find GameTimer in the scene
            gameTimer = FindObjectOfType<GameTimer>();
        }
        else
        {
            PlayMusic(mainTheme);
            gameTimer = null;
        }
    }

    void Update()
    {
        if (isLevelScene && gameTimer != null)
        {
            // Match pitch to current speed (smoothly)
            float targetPitch = Mathf.Clamp(gameTimer.currentSpeed, 0.5f, 1.0f);
            musicSource.pitch = Mathf.Lerp(musicSource.pitch, targetPitch, Time.unscaledDeltaTime * 5f);
        }
        else
        {
            // Keep normal speed for menus
            musicSource.pitch = Mathf.Lerp(musicSource.pitch, 1f, Time.unscaledDeltaTime * 2f);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.pitch = 1f;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

}
