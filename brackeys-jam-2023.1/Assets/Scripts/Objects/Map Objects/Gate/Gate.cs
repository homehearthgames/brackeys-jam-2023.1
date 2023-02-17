using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private Collider2D _coll;
    private Animator animator;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimateOpen()
    {
        animator.SetBool("Open", true);
        // Play gate open SFX
        audioManager.PlaySound("Gate Open");
    }

    public void AnimateClose()
    {
        animator.SetBool("Open", false);
        // Play gate close SFX
        audioManager.PlaySound("Gate Close");
    }

    public void Open()
    {
        _coll.enabled = false;
    }

    public void Close()
    {
        _coll.enabled = true;
    }
}
