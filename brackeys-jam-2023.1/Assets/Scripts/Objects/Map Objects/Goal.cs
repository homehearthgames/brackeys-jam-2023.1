using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Goal : MapObject
{
    private Animator animator;
    private CharacterManager characterManager;

    [SerializeField] private ParticleSystem portalCloseParticle;
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
                // Time.timeScale = 0f;
                // Wait for a second
                Destroy(other.gameObject);
                // Trigger aniamtion
                animator.SetTrigger("Close");
            }
        }
    }

    public void PortalClosed()
    {
        ParticleSystem portalParticle = Instantiate(portalCloseParticle, transform.position, Quaternion.identity, transform); 
        portalParticle.Play();
        // Resume time
        Time.timeScale = 1f;
        // Load transition
        characterManager.LevelComplete();
    }
}
