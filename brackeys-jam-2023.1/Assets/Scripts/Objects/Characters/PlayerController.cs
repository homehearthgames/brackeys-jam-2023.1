using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer _haloSprite;
    private Rigidbody2D _rb;

    [Header("Speed Setting")]
    [SerializeField] private float _speed = 5f;
    private Vector2 inputDirection;

    [Header("Jump Settings")]
    // Jump related
    // [SerializeField] private float _minJumpHeight = 1.5f;
    [SerializeField] private float _maxJumpHeight = 2f;
    [SerializeField, Range(0.0f, 1.0f)] private float _jumpVelocityRatio = 0.5f; // The ratio of the minjumpforce to maintain
    private float minJumpForce;
    private float maxJumpForce;
    [SerializeField] private float maxHeight = 0;
    [SerializeField] private float jumpCutOff;
    private float gravityScale;

    // private bool jumpButtonDown;
    // private bool jumpButtonUp;
    public bool isJumping = false;
    // private float jumpTime;
    // private float jumpTimeCounter;
    private float gravityMultiplier = 1.0f;
    private bool isGrounded = false;
    public bool IsGrounded{get{return isGrounded;}}
    [SerializeField] private Transform feetPos;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;

    // PlayerState related
    public enum PlayerState
    {
        me = 0,
        soul = 1,
        dead = 2
    };
    [Header("PlayerState Settings")]
    [SerializeField] private PlayerState _status;
    public PlayerState Status {
        get{return _status;}
        private set {
            _status = value;
            OnStatusChanged(_status);
        }
        }
    [Header("Sprites Related")]
    [SerializeField] private Sprite[] spriteArray;

    #endregion

    #region Main Methods
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // Calculate jump force
        _maxJumpHeight += 0.5f;
        // _minJumpHeight += 0.25f;

        maxJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _maxJumpHeight);
        // minJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _minJumpHeight);

        gravityScale = _rb.gravityScale;
        
        this.Status = _status;
    }

    // Update is called once per frame
    void Update()
    {
        CalcGravity();
        GatherInput();
        CalcMaxHeight();
    }

    void FixedUpdate()
    {
        Jump();
        Move();
    }
    #endregion

    #region Helper Methods

    private void CalcGravity()
    {
        // simple tester code to invert gravity.
        if(transform.position.y< -0.5){
            _rb.gravityScale = -gravityScale * gravityMultiplier;
        }else{
            _rb.gravityScale = gravityScale * gravityMultiplier;
        }
    }
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
            case PlayerState.me:
                // When the character is Me
                // Get Axis from "A" and "D"
                x = Input.GetAxisRaw("Horizontal");
                jumpButtonDown = Input.GetButtonDown("MeJump");
                jumpButtonUp = Input.GetButtonUp("MeJump") || Input.GetButtonUp("SoulJump");
                break;
            case PlayerState.soul:
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
            // jumpTimeCounter = jumpTime;
            _rb.AddForce(Vector2.up * maxJumpForce * _rb.mass, ForceMode2D.Impulse);
            // _rb.AddForce(Vector2.up * minJumpForce * _rb.mass, ForceMode2D.Impulse);
            // _rb.velocity = Vector2.up * maxJumpForce;
            // _rb.velocity = Vector2.up * minJumpForce;
        }
        if(jumpButtonUp)
        {
            isJumping = false;
        }
    }

    private void CalcMaxHeight()
    {
        if(isGrounded)
        {
            maxHeight = 0;
        }
        else
        {
            maxHeight = Mathf.Max(maxHeight, transform.position.y);
        }
    }

    private void Move()
    {
        _rb.velocity = new Vector2(inputDirection.x * _speed, _rb.velocity.y);
    }

    private void Jump()
    {
        if(_rb.velocity.y > 0.01f)
        {
            if(isJumping)
            {
                gravityMultiplier = 1f;
            }
            else
            {
                gravityMultiplier = jumpCutOff;
            }
        }
        else 
        {
            gravityMultiplier = 1f;
        }
    }
    
    private void Hold(GameObject target)
    {
        //gradually sychronizes the vertical velocity of target object (the dead body) to velocity of character. 
        
        // simple test version: simply change veclocity
        Rigidbody2D targetRigid = target.GetComponent<Rigidbody2D>();
        if (targetRigid == null){
            return;
        }  
        targetRigid.velocity = new Vector2(targetRigid.velocity.x,_rb.velocity.y) ;
    }

    // Precondition: Me will always change to Soul, Soul will always change to Dead, and Dead never changes state
    public void ChangeState()
    {
        if(_status == PlayerState.me)
        {
            // Change to soul
            // Change Sprite (Call OnStatusChange)
            // Change gravity (if necessary)
            // Flip the character flip (set scale.y to -1 )
        }
        else if(_status == PlayerState.soul)
        {
            // Change to dead
            // Change Sprite
            // deactivate (if necessary)
        }
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D other) {
        if (_status!=PlayerState.dead){
        if (other.gameObject.CompareTag("Player"))
        {
            Hold(other.gameObject);
        }
        }
        
    }

    private void OnStatusChanged(PlayerState newStatus){
        // Load Player sprite depending on initial status
        _sr.sprite = spriteArray[(int)_status];
        switch (_status){
            case PlayerState.soul:
                _haloSprite.enabled = true;
                break;
            default:
                _haloSprite.enabled = false;
                break;
        }
    }
}