using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

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
        }

        // Play background music on start
        // Potentially play 2 tracks of music and change one volume to 0

        PlaySound("UpMusic");
        PlaySound("DownMusic");
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
