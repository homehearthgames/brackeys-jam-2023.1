using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;

public class Spike : MonoBehaviour
{
    [SerializeField] private Transform _respawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if(player._status == Player.PlayerState.me)
            {
                MeDies();
                player.transform.position = _respawnPosition.position;
                player.resetVelocity();
            }
        }
    }
}
