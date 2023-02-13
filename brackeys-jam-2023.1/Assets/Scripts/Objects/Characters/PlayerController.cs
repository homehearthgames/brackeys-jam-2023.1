using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Speed Setting")]
    [SerializeField] private float _speed = 5f;
    private Vector2 inputDirection;

    [Header("Jump Settings")]
    // Jump related
    [SerializeField] private float _minJumpHeight = 1.5f;
    [SerializeField] private float _maxJumpHeight = 2f;
    [SerializeField, Range(0.0f, 1.0f)] private float _jumpVelocityRatio = 0.5f; // The ratio of the minjumpforce to maintain
    private float minJumpForce;
    private float maxJumpForce;
    [SerializeField] private float maxHeight = 0;

    private bool jumpButtonDown;
    private bool jumpButtonUp;
    private bool isJumping = false;
    private float jumpTime;
    private float jumpTimeCounter;
    private bool isGrounded = false;
    public bool IsGrounded{get{return isGrounded;}}
    [SerializeField] private Transform feetPos;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;

    // Status related
    public enum Status
    {
        me = 0,
        soul = 1,
        dead = 2
    };
    [Header("Status Settings")]
    [SerializeField] private Status _status;

    [Header("Sprites Related")]
    [SerializeField] private Sprite[] spriteArray;

    #endregion

    #region Main Methods

    // Start is called before the first frame update
    void Start()
    {
        // Calculate jump force
        _maxJumpHeight += 0.5f;
        _minJumpHeight += 0.25f;

        maxJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _maxJumpHeight);
        minJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _minJumpHeight);

        // Calculate the jump time
        jumpTime = CharacterManager.CalculateJumpTime(Physics2D.gravity.magnitude * _rb.gravityScale, minJumpForce, _jumpVelocityRatio, _maxJumpHeight, _minJumpHeight);
        // Debug.Log(_maxJumpHeight - _minJumpHeight);
        // Debug.Log(minJumpForce);
        Debug.Log(jumpTime);
        
        // Load Player sprite depending on initial status
        _sr.sprite = spriteArray[(int)_status];
    }

    // Update is called once per frame
    void Update()
    {
        GatherInput();
        if(isGrounded)
        {
            maxHeight = 0;
        }
        else
        {
            maxHeight = Mathf.Max(maxHeight, transform.position.y);
        }
    }

    void FixedUpdate()
    {
        Jump();
        Move();
    }
    #endregion

    #region Helper Methods
    private void GatherInput()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        // Horizontal moevment
        // Get horizontal input direction and jump input from input manager
        float x;
        bool jumpButtonDown;
        bool jumpButtonUp;

        switch (_status)
        {
            case Status.me:
                // When the character is Me
                // Get Axis from "A" and "D"
                x = Input.GetAxisRaw("Horizontal");
                jumpButtonDown = Input.GetButtonDown("MeJump");
                jumpButtonUp = Input.GetButtonUp("MeJump") || Input.GetButtonUp("SoulJump");
                break;
            case Status.soul:
                // When the character is Soul
                // Get Axis from "Left" and "Right"
                x = Input.GetAxisRaw("Horizontal");
                jumpButtonDown = Input.GetButtonDown("SoulJump");
                jumpButtonUp = Input.GetButtonUp("MeJump") || Input.GetButtonUp("SoulJump");
                break;
            default:
                // When the character is dead
                x = 0;
                jumpButtonDown = false;
                jumpButtonUp = false;
                break;
        }
        
        // Horizontal movement
        inputDirection = new Vector2(x, 0);
        if(inputDirection.sqrMagnitude > 1)
        {
            inputDirection.Normalize();
        }
        
        // Jump
        // Since holding down jump button jumps higher, this 
        if(jumpButtonDown && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            // _rb.AddForce(Vector2.up * maxJumpForce * _rb.mass, ForceMode2D.Impulse);
            _rb.AddForce(Vector2.up * minJumpForce * _rb.mass, ForceMode2D.Impulse);
            // _rb.velocity = Vector2.up * maxJumpForce;
            // _rb.velocity = Vector2.up * minJumpForce;
        }
        if(jumpButtonUp)
        {
            isJumping = false;
        }
    }

    private void Move()
    {
        _rb.velocity = new Vector2(inputDirection.x * _speed, _rb.velocity.y);
    }

    private void Jump()
    {
        if(isJumping)
        {
            if(jumpTimeCounter >= 0)
            {
                _rb.velocity = Vector2.up * Mathf.Max(_rb.velocity.y, minJumpForce * _jumpVelocityRatio);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }   
    }
        
    #endregion
}