using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider2D;

    private void Awake() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
    Vector2 colliderSize = boxCollider2D.size;
    colliderSize.x = spriteRenderer.size.x;
    colliderSize.y = spriteRenderer.size.y;
    boxCollider2D.size = colliderSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
