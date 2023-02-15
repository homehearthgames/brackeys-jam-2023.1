using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RedLineTrigger;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer _haloSprite;

    // [SerializeField] private PlayerController _pc;

    // PlayerState related
    public enum PlayerState
    {
        me = 0,
        soul = 1,
        dead = 2
    };
    [Header("PlayerState Settings")]
    [SerializeField] public PlayerState _status;
    public PlayerState Status {
        get{return _status;}
        set {
            _status = value;
            OnStatusChanged(_status);
        }
    }

    // Sprites
    [Header("Sprites Related")]
    [SerializeField] private Sprite[] spriteArray;

    void Awake()
    {
        PlayerCrossedLine += OnPlayerCrossedLine;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Triggers the setter function to update Status related changes.
        this.Status = _status;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDestroy(){
        PlayerCrossedLine -= OnPlayerCrossedLine;
    }

    public void FlipY(){
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y*-1, gameObject.transform.localScale.z);
    }

    public  void FlipX(){
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x*-1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
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

    private void OnPlayerCrossedLine(Player playerInstance){
        if (playerInstance==this){
            //_rb.velocity.normalized.y;


        }
    }
}