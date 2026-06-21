using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class optionsManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void Start()
    {
        float musicValue = PlayerPrefs.GetFloat("MusicVolumeValue", 1f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolumeValue", 1f);

        musicSlider.SetValueWithoutNotify(musicValue);
        sfxSlider.SetValueWithoutNotify(sfxValue);

        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);
    }

    public void SetMusicVolume(float value)
    {
        float volume = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolumeValue", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float volume = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolumeValue", value);
        PlayerPrefs.Save();
    }
}
