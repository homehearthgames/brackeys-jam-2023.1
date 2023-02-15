using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RedLineTrigger : MonoBehaviour
{
    public static event Action<Player> PlayerCrossedLine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Hello?");
        if(PlayerCrossedLine!=null && other.gameObject.CompareTag("Player")){
            PlayerCrossedLine.Invoke(other.gameObject.GetComponent<Player>());
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        
    }
}
