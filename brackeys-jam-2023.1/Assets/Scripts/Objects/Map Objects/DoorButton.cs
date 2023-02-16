using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] private Door _door;

    [Header("Sprite Related")]
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Sprite buttonUp;
    [SerializeField] private Sprite buttonDown;

    [SerializeField] private bool _active;

    private int numObjectOnTop = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        numObjectOnTop = 0;

        if(_door == null)
        {
            _door.gameObject.GetComponentInParent<Door>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
            _door.Open();
        }
        else
        {
            _sr.sprite = buttonUp;
            _door.Close();
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
