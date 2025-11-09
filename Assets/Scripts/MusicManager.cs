using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Music Clips")]
    public AudioClip mainTheme;      // Menu music
    public AudioClip levelTheme;     // Normal level music
    public AudioClip level7Theme;    // Special Level 7 music
    public AudioClip victoryTheme;   // Victory screen music

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
        // Default quieter startup volume (change 0.3f to what feels right)
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        musicSource.volume = savedVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name.ToLower();
        isLevelScene = sceneName.StartsWith("level");

        if (sceneName == "victorymenu")
        {
            PlayMusic(victoryTheme);
            gameTimer = null;
        }
        else if (sceneName.StartsWith("level7"))
        {
            PlayMusic(level7Theme);
            gameTimer = FindObjectOfType<GameTimer>();
        }
        else if (isLevelScene)
        {
            PlayMusic(levelTheme);
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
        // Adjust pitch dynamically during gameplay
        if (isLevelScene && gameTimer != null)
        {
            float targetPitch = Mathf.Clamp(gameTimer.currentSpeed, 0.5f, 1.0f);
            musicSource.pitch = Mathf.Lerp(musicSource.pitch, targetPitch, Time.unscaledDeltaTime * 5f);
        }
        else
        {
            // Restore to normal pitch for menus or non-level scenes
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
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }
}
