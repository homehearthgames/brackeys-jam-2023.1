using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Goal : MapObject
{
    private CharacterManager characterManager;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        characterManager = CharacterManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerState status = other.gameObject.GetComponent<Player>()._status;
            if(status == PlayerState.me || status == PlayerState.soul)
            {
                // Level Completed
                Debug.Log("Level Complete!");
                // Animation stuffs
                
                // Level Transition
                // Music things

                // Load next level
                characterManager.LoadNextLevel();
            }
        }
    }
}
