using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class volumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audMix;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider masterSlider;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("SoundEffects") && PlayerPrefs.HasKey("MusicAudio") && PlayerPrefs.HasKey("MasterVolume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SoundEffects");
            musicSlider.value = PlayerPrefs.GetFloat("MusicAudio");
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            sfxSlider.value = 1;
            musicSlider.value = 1;
            masterSlider.value = 1;
        }
    }
    private void Update()
    {
        setSfxVolume();
        setMusicVolume();
        setMasterVolume();
    }
    void setSfxVolume()
    {
        if(sfxSlider.value == 0)
        {
            audMix.SetFloat("SoundEffects", -80f);
        }
        else
        {
            audMix.SetFloat("SoundEffects", Mathf.Log10(sfxSlider.value) * 20);
        }
        PlayerPrefs.SetFloat("SoundEffects", sfxSlider.value);
    }
    void setMusicVolume()
    {
        
        if (musicSlider.value == 0)
        {
            audMix.SetFloat("MusicAudio", -80f);
        }
        else
        {
            audMix.SetFloat("MusicAudio", Mathf.Log10(musicSlider.value) * 20);
        }
        PlayerPrefs.SetFloat("MusicAudio", musicSlider.value);
    }
    void setMasterVolume()
    {

        if (masterSlider.value == 0)
        {
            audMix.SetFloat("MasterVolume", -80f);
        }
        else
        {
            audMix.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
        }
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }
}
