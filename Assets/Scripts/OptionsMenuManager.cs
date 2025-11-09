using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Load saved values or defaults
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        // Apply immediately
        MusicManager.Instance.SetMusicVolume(musicVol);
        SoundManager.Instance.SetSFXVolume(sfxVol);

        // Add listeners
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMusicVolumeChanged(float value)
    {
        MusicManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
