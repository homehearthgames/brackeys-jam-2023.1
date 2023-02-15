using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] private PlayerController _pc;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    public bool GetOnGround()
    {
        return Physics2D.OverlapArea(_pointA.position, _pointB.position, _whatIsGround);
    }
}
