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
        float vol;
        audioMixer.GetFloat("volume", out vol);
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
        Play("theme");
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " wasn't not found!");
        }
        s.source.Play();
    }
}
