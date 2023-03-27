using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager Instance;
    public AudioMixer audioMixer;
    public AudioMixerGroup audioMixerGroup;

    private void IncreaseVolumeABit()
    {
        audioMixer.GetFloat("volume", out float vol);
        if (vol < 0)
        {
            audioMixer.SetFloat("volume", vol * 0.92f + 0.001f);
        }
    }

    public void IncreaseVolumeOverTime()
    {
        InvokeRepeating(nameof(IncreaseVolumeABit), 0, 0.01f);
    }

    private void Awake()
    {
        #region Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        #endregion

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = audioMixerGroup;
        }
    }

    public void Play (string name)
    {
        //TODO-UGLY: This is ugly as hell
        if (!SettingsModel.Instance.PlaySounds) return;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " wasn't not found!");
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " wasn't not found!");
        }
        s.source.Stop();
    }
}
