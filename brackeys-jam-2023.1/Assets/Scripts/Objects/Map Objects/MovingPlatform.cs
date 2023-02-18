using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    

    [SerializeField] private Vector2 _startPoint;
    [SerializeField] private Vector2 _endPoint;
    private float totalDistance;

    [SerializeField, Range(0.0f, 1.0f)] private float _accelerationRatio = 0.5f; // accelerate to full speed in 2 seconds (1 / accel)
    [SerializeField, Range(0.0f, 1.0f)] private float _decelerationRatio = 0.5f;
    [SerializeField] private float _maxSpeed;
    private float currentSpeed = 0.0f;
    [SerializeField] private bool _forward = true;
    [SerializeField] private float _waitTime; // time to wait in seconds for the platform to return

    private float deceDistance;

    private bool accelerating;
    private bool decelerating;

    void Awake()
    {
        
        if(_startPoint != (Vector2)transform.position)
        {
            Debug.LogError("Platform \"" + gameObject.name + "\" start point not match with startin transform postiion!");
        }

        totalDistance = (_endPoint - _startPoint).magnitude;
        // Distance for acceleration
        float acceDistance = _maxSpeed / (2 * _accelerationRatio);
        deceDistance = _maxSpeed / (2 * _decelerationRatio);
        if(acceDistance + deceDistance >= totalDistance)
        {
            Debug.LogError("Moving Platform " + gameObject.name + " distance too short due to acceleration time & deceleration time");
        }
        // Distance for deceleration
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = 0.0f;
        accelerating = true;
    }

    void Update()
    {
        if(((Vector2)transform.position - (_forward ? _endPoint : _startPoint)).magnitude <= deceDistance && decelerating == false)
        {
            decelerating = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(accelerating)
        {
            currentSpeed += Mathf.Min(_accelerationRatio * _maxSpeed * Time.deltaTime, _maxSpeed);
            if(currentSpeed >= _maxSpeed)
            {
                accelerating = false;
            }
        }
        else if (decelerating)
        {
            currentSpeed -= Mathf.Max(_decelerationRatio * _maxSpeed * Time.deltaTime, 0.0f);
            if(currentSpeed <= 0)
            {
                _forward = !_forward;
                decelerating = false;
                currentSpeed = 0;
                StartCoroutine(Turn());
            }
        }

        Vector3 pos;
        if(_forward)
        {
            pos = Vector2.MoveTowards(transform.position, _endPoint, currentSpeed * Time.deltaTime);
        }
        else
        {
            pos = Vector2.MoveTowards(transform.position, _startPoint, currentSpeed * Time.deltaTime);
        }

        transform.position = pos;
    }

    private IEnumerator Turn()
    {
        yield return new WaitForSeconds(_waitTime);
        accelerating = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.SetParent(null);
    }
}
