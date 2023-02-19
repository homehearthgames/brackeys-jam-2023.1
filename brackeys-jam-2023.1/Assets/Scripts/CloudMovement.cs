using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour {

    public float minSpeed;
    public float maxSpeed;
    public bool moveRight;

    public float deathPosition = 20f;

    private float speed;

    void Start() {
        speed = Random.Range(minSpeed, maxSpeed);
        if (moveRight) {
            speed *= 1;
        } else
        {
            speed *= -1;
        }
    }

    void Update () {
        transform.Translate(speed * Time.deltaTime, 0f, 0f);

        if (transform.position.x < -deathPosition || transform.position.x > deathPosition) {
            Destroy(gameObject);
        }
    }
}



