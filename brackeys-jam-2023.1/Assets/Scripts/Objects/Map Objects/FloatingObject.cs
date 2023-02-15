using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    // This class is degenerate if the inspector is set up correctly
    // Just making sure the Connected Anchor is the original transform position of the object
    [SerializeField] private SpringJoint2D _sj;
    private Vector3 originalPosition;
    // Start is called before the first frame update
    void Awake()
    {
        originalPosition = transform.position;
        _sj.connectedAnchor = originalPosition;
    }
}
