using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterManager;
using static TileManager;

public class Spike : MapObject
{
    [SerializeField] private Transform _respawnPosition;
    [SerializeField] private bool ManualPosition = false;
    //[SerializeField] private Sprite[] spriteArray;
    //[SerializeField]private SpriteRenderer _sprite;

    // Start is called before the first frame update
    override protected void Start()
    {
        //LoadSprite();
        base.Start();
        if(!ManualPosition){
        LocateSpawnPointToOtherSide();
        }
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if(player._status == Player.PlayerState.me)
            {
                MeDies();
                player.transform.position = _respawnPosition.position;
                if (CanTeleport()){
                    
                    player.ResetVelocity();
                }else{
                    //player.transform.position = _respawnPosition.position;
                    SoulDies();
                    BodyDies(player);
                }
                

            }else if(player._status == Player.PlayerState.soul){
                SoulDies();
                player.transform.position = _respawnPosition.position;
                if (CanTeleport()){   
                    //player.transform.position = _respawnPosition.position;
                    player.ResetVelocity();
                }else{
                    BodyDies(player);
                }
                
            }
        }
    }



    
    
    private bool CanTeleport(){
        if (TileManager.HasTile(_respawnPosition.position))
        {
            return false;
        }
        return true;
    }

    public void LocateSpawnPointToOtherSide(){
        Vector3 pos = CharacterManager.GetOppositePos(transform.position);
        _respawnPosition.position = pos;
    }
    /*
    public void LoadSprite(){
        if (transform.position.y>=-0.5){
            _sprite.sprite = spriteArray[0];
            return;
        }
        _sprite.sprite = spriteArray[1];
        Quaternion currentRotation = _sprite.gameObject.transform.rotation;
        Quaternion newRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z+180);
        _sprite.gameObject.transform.rotation = newRotation;
    }
    */
}
