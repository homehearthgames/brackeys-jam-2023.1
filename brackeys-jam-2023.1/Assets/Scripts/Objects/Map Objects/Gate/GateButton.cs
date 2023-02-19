using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateButton : MonoBehaviour
{
    [SerializeField] private Gate _gate;

    [Header("Sprite Related")]
    [SerializeField] private SpriteRenderer _sr;
    private AudioManager audioManager;

    [SerializeField] private Sprite buttonUp;
    [SerializeField] private Sprite buttonDown;

    [SerializeField] private bool _active;

    private int numObjectOnTop = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        numObjectOnTop = 0;

        if(_gate == null)
        {
            _gate.gameObject.GetComponentInParent<Gate>();
        }

        audioManager = AudioManager.instance;
    }

    private void SetActive(bool newActive)
    {
        // set active to its state
        _active = newActive;

        // change spirte
        // trigger sound
        // call _door to open/close
        if(newActive == true)
        {
            _sr.sprite = buttonDown;
            // audioManager.PlaySound("Button Press");
            _gate.AnimateOpen();
        }
        else
        {
            _sr.sprite = buttonUp; 
            // audioManager.PlaySound("Button Release");
            _gate.AnimateClose();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            numObjectOnTop += 1;
            SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            numObjectOnTop -= 1;
            if(numObjectOnTop == 0)
            {
                SetActive(false);
            }
        }
    }
}
