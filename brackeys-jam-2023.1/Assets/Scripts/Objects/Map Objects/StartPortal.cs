using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPortal : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    public void OpenPortal()
    {
        animator.SetTrigger("Open");
    }
    public void StartPortalOpen()
    {
        CharacterManager.SpawnMe(transform.position);
        AudioManager.instance.PlaySound("Portal");
        CharacterManager.instance.me._active = true;
    }
}
