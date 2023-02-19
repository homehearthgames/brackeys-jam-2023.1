using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public bool isThemePlaying = false;

    [SerializeField] private Sound[] sounds;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            GameObject soundObject = new GameObject("Sound_" + i + "_" + sounds[i].soundName);
            sounds[i].SetSource(soundObject.AddComponent<AudioSource>());
            soundObject.transform.parent = gameObject.transform;
        }
    }

    public void PlayTheme()
    {
        if(isThemePlaying)
        {
            Debug.LogWarning("Theme is already playing!");
        }
        else
        {
            isThemePlaying = true;
            PlaySound("UpMusic");
            PlaySound("DownMusic");
            StopSound("MenuMusic");
            MuteSound("DownMusic", true);
        }
    }

    public void PlaySound(string name)
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].soundName == name)
            {
                sounds[i].Play();
                return;
            }
        }
        Debug.LogWarning("AudioManger: Sound not found in list: " + name);
    }

    public void StopSound(string name)
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].soundName == name)
            {
                sounds[i].Stop();
                return;
            }
        }
        Debug.LogWarning("AudioManger: Sound not found in list: " + name);
    }

    public void MuteSound(string name, bool mute)
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].soundName == name)
            {
                sounds[i].Mute(mute);
                return;
            }
        }
        Debug.LogWarning("AudioManger: Sound not found in list: " + name);
    }

    public void ChangeVolume(string name, float volume)
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].soundName == name)
            {
                sounds[i].ChangeVolume(volume);
                return;
            }
        }
        Debug.LogWarning("AudioManger: Sound not found in list: " + name);
    }
}
