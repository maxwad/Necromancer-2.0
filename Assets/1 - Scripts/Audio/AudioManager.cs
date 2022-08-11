using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instanse;

    public enum AudioChannel {Master, Music, Effects};

    public AudioMixer mixer;
    public AudioMixerSnapshot normal;
    public AudioMixerSnapshot menu;

    //Mixer's volume is from max = 1 to min = 0 
    private float defaultMasterVolume = 0.7f;
    private float defaultMusicVolume = 0.5f;
    private float defaultEffectsVolume = 0.3f;

    public GameObject musicSourceObj;
    public GameObject effectsAudioSourceObj;
    public GameObject iEffectsAudioSourceObj;
    [HideInInspector] public AudioSource musicSource;
    [HideInInspector] public AudioSource effectsAudioSource;
    [HideInInspector] public AudioSource iEffectsAudioSource;
    public Transform audioListener;

    private SoundLibrary library;

    private void Awake()
    {        
        if (instanse != null)
        {
            Destroy(gameObject);            
        }
        else
        {
            instanse = this;
            DontDestroyOnLoad(instanse);

            library = GetComponent<SoundLibrary>();
            musicSource = musicSourceObj.GetComponent<AudioSource>();
            effectsAudioSource = effectsAudioSourceObj.GetComponent<AudioSource>();
            iEffectsAudioSource = iEffectsAudioSourceObj.GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        //in Awake method we can't do it, because it's a unity bag
        LoadVolumes();
    }

    private void LoadVolumes()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("masterVolume")) * 20);
        }
        else
        {
            PlayerPrefs.SetFloat("masterVolume", defaultMasterVolume);
            mixer.SetFloat("MasterVolume", defaultMasterVolume);
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume")) * 20);
        }
        else
        {
            PlayerPrefs.SetFloat("musicVolume", defaultMusicVolume);
            mixer.SetFloat("MusicVolume", defaultMusicVolume);
        }

        if (PlayerPrefs.HasKey("effectsVolume"))
        {
            mixer.SetFloat("EffectsVolume", Mathf.Log10(PlayerPrefs.GetFloat("effectsVolume")) * 20);
        }
        else
        {
            PlayerPrefs.SetFloat("effectsVolume", defaultEffectsVolume);
            mixer.SetFloat("EffectsVolume", defaultEffectsVolume);
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        //volumePercent = Mathf.Log10(volumePercent) * 20;
        switch (channel)
        {
            case AudioChannel.Master:
                mixer.SetFloat("MasterVolume", Mathf.Log10(volumePercent) * 20);
                PlayerPrefs.SetFloat("masterVolume", volumePercent);
                break;

            case AudioChannel.Music:
                mixer.SetFloat("MusicVolume", Mathf.Log10(volumePercent) * 20);
                PlayerPrefs.SetFloat("musicVolume", volumePercent);
                break;

            case AudioChannel.Effects:
                mixer.SetFloat("EffectsVolume", Mathf.Log10(volumePercent) * 20);
                PlayerPrefs.SetFloat("effectsVolume", volumePercent);
                break;
        }
        PlayerPrefs.Save();
    } 

    public void SoundMode(int mode)
    {
        // 0 - pause mode
        // 1 - normal mode
        if (mode == 0)
            normal.TransitionTo(0.5f);
        else
            menu.TransitionTo(0.5f);

    }
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // method for all sound effects
    public void PlaySound(AudioClip clip, int important = 0)
    {
        if (important == 0)
            effectsAudioSource.PlayOneShot(clip);
        else
            iEffectsAudioSource.PlayOneShot(clip);
    }

    public void Play3DSound(AudioClip clip, Vector2 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }

    // extension method for UI effect
    public void PlayUISound(string nameClip, int important = 0)
    {
        if (important == 0)
            effectsAudioSource.PlayOneShot(library.GetSpecificClipFromName(nameClip));
        else
            iEffectsAudioSource.PlayOneShot(library.GetSpecificClipFromName(nameClip));
    }

    // extension method for special effect
    public void PlaySpecificSound(string nameClip, int important = 0)
    {
        PlaySound(library.GetSpecificClipFromName(nameClip), important);        
    }

    public void StopSound()
    {
        effectsAudioSource.Stop();
        iEffectsAudioSource.Stop();
    }
}
