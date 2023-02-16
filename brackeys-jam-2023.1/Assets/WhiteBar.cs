using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBar : MonoBehaviour
{
    [SerializeField] private Collider2D _fixedUp;
    [SerializeField] private Collider2D _fixedDown;
    [SerializeField] private Collider2D _up;
    [SerializeField] private Collider2D _down;

    [SerializeField] private GameObject _bar;

    [SerializeField] private int height = 0;

    void Awake()
    {
        _fixedUp.enabled = true;
        _fixedDown.enabled = true;
        _up.enabled = false;
        _down.enabled = false;

        height = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        height = 0;
    }

    // If increase = false, then Decrement height
    public void IncrementHeight(bool increase)
    {
        if(increase)
        {
            height += 1;
            _bar.transform.position += Vector3.up;
        }
        else
        {
            height -= 1;
            _bar.transform.position += Vector3.down;
        }

        if(height > 0)
        {
            _fixedUp.enabled = false;
            _fixedDown.enabled = true;
            _up.enabled = true;
            _down.enabled = false;
        }
        else if(height < 0)
        {
            _fixedUp.enabled = true;
            _fixedDown.enabled = false;
            _up.enabled = false;
            _down.enabled = true;
        }
        // else
        // {
        //     _fixedUp.enabled = true;
        //     _fixedDown.enabled = true;
        //     _up.enabled = false;
        //     _down.enabled = false;
        // }
    }
}
