using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RedLineTrigger;
using static CharacterManager;

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
    // [SerializeField, Range(0.0f, 1.0f)] private float _jumpVelocityRatio = 0.5f; // The ratio of the minjumpforce to maintain
    [SerializeField] private float jumpCutOff = 2f;
    // private float minJumpForce;
    private float maxJumpForce;
    [SerializeField] private float maxHeight = 0;
    private float gravityScale;

    // private bool jumpButtonDown;
    // private bool jumpButtonUp;
    private bool jumpButtonPress;
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
        set {
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
        PlayerCrossedLine += OnPlayerCrossedLine;
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
        //call Character Manager to calculate jumpforce for player.
       InitializeJumpForce();
        
        //Triggers the setter function to update Status related changes.
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

    void OnDestroy(){
        PlayerCrossedLine -= OnPlayerCrossedLine;
    }
    #endregion

    #region Helper Methods

    private void CalcGravity()
    {
        // simple tester code to invert gravity.
        if(!aboveRedLine()){
            _rb.gravityScale = -gravityScale * gravityMultiplier;
        }else{
            _rb.gravityScale = gravityScale * gravityMultiplier;
        }
    }

    private Vector2 GetJumpDirection(){
        Vector2 direction = new Vector2(0, Mathf.Sign(_rb.gravityScale));
        //Debug.Log(direction);
        return direction;
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
                jumpButtonPress = Input.GetButton("MeJump");
                break;
            case PlayerState.soul:
                // When the character is Soul
                // Get Axis from "Left" and "Right"
                x = Input.GetAxisRaw("Horizontal");
                jumpButtonDown = Input.GetButtonDown("SoulJump");
                jumpButtonUp = Input.GetButtonUp("MeJump") || Input.GetButtonUp("SoulJump");
                jumpButtonPress = Input.GetButton("SoulJump");
                break;
            default:
                // When the character is dead
                x = 0;
                jumpButtonDown = false;
                jumpButtonUp = false;
                jumpButtonPress = false;
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
            //_rb.AddForce(Vector2.up * maxJumpForce * _rb.mass, ForceMode2D.Impulse);
            // _rb.AddForce(GetJumpDirection() * maxJumpForce * _rb.mass, ForceMode2D.Impulse);
            
            // _rb.AddForce(Vector2.up * minJumpForce * _rb.mass, ForceMode2D.Impulse);
            _rb.velocity = (_status == PlayerState.me ? Vector2.up : Vector2.down) * maxJumpForce;
            // _rb.velocity = Vector2.up * minJumpForce;
        }

        if(!jumpButtonPress && isGrounded)
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
                    gravityMultiplier = jumpCutOff;
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

    // Precondition: Me will always change to Soul, Soul will always change to Dead, and Dead never changes state
    private void InitializeJumpForce(){
         // Calculate jump force
        _maxJumpHeight += 0.5f;
        // _minJumpHeight += 0.25f;

        maxJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _maxJumpHeight);
        // minJumpForce = CharacterManager.CalculateJumpForce(Physics2D.gravity.magnitude * _rb.gravityScale, _minJumpHeight);

        // Calculate the jump time
        // jumpTime = CharacterManager.CalculateJumpTime(Physics2D.gravity.magnitude * _rb.gravityScale, minJumpForce, _jumpVelocityRatio, _maxJumpHeight, _minJumpHeight);
        // Debug.Log(_maxJumpHeight - _minJumpHeight);
        // Debug.Log(minJumpForce);
        //Debug.Log(jumpTime);
    }

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

    private bool aboveRedLine()
    {
        return transform.position.y >= -0.5;
    }

    public void FlipY(){
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y*-1, gameObject.transform.localScale.z);
    }

    public  void FlipX(){
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x*-1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
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

    private void OnPlayerCrossedLine(PlayerController playerInstance){
        if (playerInstance==this){
            //_rb.velocity.normalized.y;


        }
    }


}