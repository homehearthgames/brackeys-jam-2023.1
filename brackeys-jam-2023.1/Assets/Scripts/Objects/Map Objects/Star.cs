using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private AudioManager audioManager;
    private CharacterManager characterManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
        characterManager = CharacterManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            // Play SFX
            audioManager.PlaySound("Collect Star");

            // Call function in CharacterManager
            characterManager.CollectStar();

            // Destroy GameObject
            Destroy(gameObject);
        }
    }

}
