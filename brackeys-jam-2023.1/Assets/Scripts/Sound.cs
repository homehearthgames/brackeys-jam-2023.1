using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;

    private AudioSource source;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 2f)] public float pitch = 1f;

    public bool loop = false;

    public void SetSource(AudioSource source)
    {
        this.source = source; 
        this.source.clip = clip;
        this.source.loop = loop;
    }

    public void Play()
    {
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void Mute(bool mute)
    {
        source.mute = mute;
    }

    public void ChangeVolume(float newVolume)
    {
        source.volume = newVolume;
    }

}
