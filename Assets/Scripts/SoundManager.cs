using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Sound Effects")]
    public AudioClip playerDeathClip;
    public AudioClip goalReachedClip;
    public AudioClip playerWalkClip;

    private bool isWalkingSoundPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure AudioSources exist
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSource.volume = savedVolume;
    }

    // ---- Sound Effect Methods ----
    public void PlayPlayerDeath()
    {
        PlaySFX(playerDeathClip);
    }

    public void PlayGoalReached()
    {
        PlaySFX(goalReachedClip);
    }

    public void HandleWalkingSound(bool isMoving)
    {
        if (isMoving && !isWalkingSoundPlaying)
        {
            sfxSource.clip = playerWalkClip;
            sfxSource.loop = true;
            sfxSource.Play();
            isWalkingSoundPlaying = true;
        }
        else if (!isMoving && isWalkingSoundPlaying)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            isWalkingSoundPlaying = false;
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
}
