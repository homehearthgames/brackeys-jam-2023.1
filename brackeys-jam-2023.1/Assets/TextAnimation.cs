using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{
    public float rotationSpeed = 50.0f; // speed at which the object rotates left and right
    public float maxRotationAngle = 15.0f; // maximum angle of rotation on the Z axis
    public float rotationWobbleFrequency = 1.0f; // frequency of the rotation wobble effect
    public float rotationWobbleAmplitude = 0.2f; // amplitude of the rotation wobble effect

    private float rotationDirection = 1.0f; // direction of rotation (1 for clockwise, -1 for counterclockwise)
    private TMP_Text textMesh; // reference to the TextMeshProUGUI component
    private float rotationWobbleOffset = 0.0f; // current offset for the rotation wobble effect

    private void Start()
    {
        textMesh = GetComponent<TMP_Text>(); // get reference to TextMeshProUGUI component
    }

    private void Update()
    {
        // rotation
        float rotationAngle = Mathf.Sin(Time.time * rotationWobbleFrequency + rotationWobbleOffset) * rotationWobbleAmplitude;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, maxRotationAngle * rotationAngle * rotationDirection);
        if (Mathf.Abs(transform.rotation.z) > 0.99f * maxRotationAngle) // check if rotation limit is reached
        {
            rotationDirection = -rotationDirection; // switch rotation direction
            rotationWobbleOffset = Time.time * rotationWobbleFrequency;
        }
    }
}
