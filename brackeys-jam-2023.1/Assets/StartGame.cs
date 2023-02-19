using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        audioManager = AudioManager.instance;
        if(audioManager != null && !audioManager.isMenuPlaying)
        {
            audioManager.PlaySound("MenuMusic");
            audioManager.isMenuPlaying = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("LevelSelectionScene");
        }
    }
}
