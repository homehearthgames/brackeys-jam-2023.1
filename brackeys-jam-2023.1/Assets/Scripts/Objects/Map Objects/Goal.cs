using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Goal : MapObject
{
    private Animator animator;
    private CharacterManager characterManager;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        characterManager = CharacterManager.instance;
        animator = transform.GetComponent<Animator>();
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
                // "Freeze" player
                Player player = other.transform.GetComponent<Player>();
                player._active = false;
                // Time.timeScale = 0f;
                // Wait for a second
                StartCoroutine(LevelCompleteWait(other));
                
            }
        }
    }

    IEnumerator LevelCompleteWait(Collider2D other)
    {
        Debug.Log("Start Wait");
        yield return new WaitForSeconds(1);
        Debug.Log("Wait Finish");
        // Destroy the character touching the portal (entered the portal)
        Destroy(other.gameObject);
        // Trigger aniamtion
        animator.SetTrigger("Close");
    }

    public void PortalClosed()
    {
        // Resume time
        Time.timeScale = 1f;
        // Load transition
        characterManager.LevelComplete();
    }
}
