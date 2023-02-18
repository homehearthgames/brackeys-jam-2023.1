using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    public void FinishLoading()
    {
        // Call character manager to open portal
        CharacterManager.instance.OpenSpawnPortal();
    }

    public void StartLoading()
    {
        animator.SetTrigger("LevelComplete");
    }

    public void StartLoadingComplete()
    {
        CharacterManager.instance.LoadNextLevel();
    }
}
