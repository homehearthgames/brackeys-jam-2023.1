using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RedLineTrigger;
using static CharacterManager;
using static Player;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private Player _player;
    [SerializeField] private GroundDetection _gd;
    private Rigidbody2D _rb;
    private AudioManager audioManager;

    [Header("Speed Setting")]
    [SerializeField] private float _speed = 5f;
    private Vector2 inputDirection;
    private float originalScaleX;

    [Header("Jump Settings")]
    // Jump related
    [SerializeField] private float _maxJumpHeight = 2f;
    [SerializeField] private float _jumpCutOff = 2f;
    private float maxJumpForce;
    private float gravityScale;
    
    private bool jumpButtonPress;
    private bool desireJump;
    public bool isJumping = false;
    private float gravityMultiplier = 1.0f;
    public bool isGrounded = false;
    // public bool IsGrounded{get{return isGrounded;}}

    #endregion

    #region Main Methods
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
        originalScaleX = transform.localScale.x;
        // Calculate jump force
        _maxJumpHeight += 0.5f;

        maxJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _maxJumpHeight);

        gravityScale = _rb.gravityScale;
        //call Character Manager to calculate jumpforce for player.
        InitializeJumpForce();
    }

    // Update is called once per frame
    void Update()
    {
        CalcGravity();
        GroundCheck();
        GatherInput();
    }

    void FixedUpdate()
    {
        Flip();
        Jump();
        Move();
    }

    #endregion

    #region Helper Methods

    private void CalcGravity()
    {
        // simple tester code to invert gravity.
        /*
        if(!aboveRedLine()){
            _rb.gravityScale = -gravityScale * gravityMultiplier;
        }else{
            _rb.gravityScale = gravityScale * gravityMultiplier;
        }
        */
        
        //the gravity scale now depends on the player status. 
        switch(_player.Status){
            case PlayerState.me:
                _rb.gravityScale = gravityScale * gravityMultiplier;
            break;
            case PlayerState.soul:
                _rb.gravityScale = -gravityScale * gravityMultiplier;
            break;
            default:
                if(!aboveRedLine()){
                    _rb.gravityScale = -gravityScale * gravityMultiplier;
                }else{
                    _rb.gravityScale = gravityScale * gravityMultiplier;
                }
            break;
        }
        

    }

    private Vector2 GetJumpDirection(){
        Vector2 direction = new Vector2(0, Mathf.Sign(_rb.gravityScale));
        return direction;
    }

    public void GroundCheck()
    {
        bool newGrounded = _gd.GetOnGround();
        if(isGrounded != newGrounded && !isJumping && newGrounded && _player._status != PlayerState.dead)
        {
            // Character landed
            _player.GenerateLandingParticles();
        }
        isGrounded = newGrounded;
    }

    private void GatherInput()
    {
        // isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        
        // Horizontal moevment
        // Get horizontal input direction and jump input from input manager
        float x = 0;
        bool jumpButtonDown = false;
        bool jumpButtonUp = false;

        if(_player._active)
        {
            x = Input.GetAxisRaw("Horizontal");
            jumpButtonDown = Input.GetButtonDown("Jump");
            jumpButtonUp = Input.GetButtonUp("Jump");
            jumpButtonPress = Input.GetButton("Jump");
        }
        else
        {
            jumpButtonPress = false;
        }
        // Horizontal movement
        inputDirection = new Vector2(x, 0);
        if(inputDirection.sqrMagnitude > 1)
        {
            inputDirection.Normalize();
        }
        
        // Jump
        if(jumpButtonDown && isGrounded)
        {
            isJumping = true;
            isGrounded = false;
            desireJump = true;
        }

        if(!jumpButtonPress && isGrounded)
        {
            isJumping = false;
        }
    }

    private void Flip()
    {
        float x = inputDirection.x;
        if(x > 0)
        {
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (x < 0)
        {
            transform.localScale = new Vector3(originalScaleX * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    private void Move()
    {
        _rb.velocity = new Vector2(inputDirection.x * _speed, _rb.velocity.y);
    }

    private void Jump()
    {
        if(desireJump)
        {
            // Jump
            _rb.velocity = (_player._status == Player.PlayerState.me ? Vector2.up : Vector2.down) * maxJumpForce;
            audioManager.PlaySound("Jump");
            _player.GenerateJumpingParticles();

            desireJump = false;
        }

        // Jumping mid-air
        if((_rb.velocity.y > 0.01f && aboveRedLine()) || (_rb.velocity.y < -0.01f && !aboveRedLine()))
        {
            if(isJumping)
            {
                if(jumpButtonPress)
                {
                    gravityMultiplier = 1f;
                }
                else
                {
                    gravityMultiplier = _jumpCutOff;
                }
            } 
        }
        else 
        {
            isJumping = false;
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

    private void InitializeJumpForce(){
         // Calculate jump force
        _maxJumpHeight += 0.5f;
        maxJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _maxJumpHeight);
    }

    public void ResetVelocity()
    {
        _rb.velocity = Vector2.zero;
    }

    public void RevertVelocity()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, -_rb.velocity.y);
    }

    private bool aboveRedLine()
    {
        return transform.position.y >= -0.0f;
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D other) {
        if (_player._status != Player.PlayerState.dead){
            if (other.gameObject.CompareTag("Player"))
            {
                Hold(other.gameObject);
            }
        }   
    }


}