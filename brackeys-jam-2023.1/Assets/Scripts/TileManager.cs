using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    public Tilemap tileMap;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        tileMap = GetComponent<Tilemap>();
    }

    public static bool HasTile(Vector3 position){

        return instance.tileMap.HasTile(instance.tileMap.WorldToCell(new Vector2(position.x, position.y)));
        
    }

}
