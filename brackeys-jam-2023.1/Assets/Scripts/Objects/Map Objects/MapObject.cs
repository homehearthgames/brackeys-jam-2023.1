using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    //Not really map object, but objects that needs to be upside down when needed. 
    [SerializeField] protected Sprite[] spriteArray;
    [SerializeField]protected SpriteRenderer _sprite;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        LoadSprite();
    }


    public void LoadSprite(){
        _sprite.sprite = spriteArray[0];
        if (transform.position.y>=-0.5){
            return;
        }
        if(spriteArray.Length>1){
            _sprite.sprite = spriteArray[1];
        }
        Quaternion currentRotation = _sprite.gameObject.transform.rotation;
        Quaternion newRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z+180);
        _sprite.gameObject.transform.rotation = newRotation;
    }
}
