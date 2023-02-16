using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBarCollider : MonoBehaviour
{
    [SerializeField] private WhiteBar _wb;
    [SerializeField] private bool _up;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TriggerEnter: " + gameObject.name);
        if(other.CompareTag("Player"))
        {
            _wb.IncrementHeight(!_up);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("TriggerExit: " + gameObject.name);
        if(other.CompareTag("Player"))
        {
            _wb.IncrementHeight(_up);
        }
    }
}
