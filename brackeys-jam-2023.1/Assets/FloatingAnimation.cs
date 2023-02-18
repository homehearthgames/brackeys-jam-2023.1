using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    public float amplitude = 0.5f; // amplitude of the float effect
    public float frequency = 0.5f; // frequency of the float effect

    private Vector3 startPosition; // starting position of the object

    private void Start()
    {
        startPosition = transform.position; // store the starting position of the object
    }

    private void Update()
    {
        // calculate the new Y position of the object based on the float effect
        float newY = startPosition.y + amplitude * Mathf.Sin(frequency * Time.time);

        // update the position of the object
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
