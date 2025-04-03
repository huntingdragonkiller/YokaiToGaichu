using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume") || PlayerPrefs.HasKey("MusicVolume") ||
            PlayerPrefs.HasKey("AmbienceVolume") || PlayerPrefs.HasKey("SFXVolume"))
        {
            Debug.Log("volume loaded");
            LoadVolume();
        }
        else
        {
            Debug.Log("default volume loaded");
            SetMasterVolume();
            SetMusicVolume();
            SetAmbienceVolume();
            SetSFXVolume();
        }
    }
    
    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("Master", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    public void SetAmbienceVolume()
    {
        float volume = ambienceSlider.value;
        audioMixer.SetFloat("Ambience", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("AmbienceVolume", volume);
    }
    
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        ambienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        
        SetMasterVolume();
        SetMusicVolume();
        SetAmbienceVolume();
        SetSFXVolume();
    }
}
