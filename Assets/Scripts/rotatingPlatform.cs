using UnityEngine;

public class rotatingPlatform : MonoBehaviour
{
    [Header("Rotation Settings")]

    // Controls how fast the platform rotates
    public float rotationSpeed = 50f;

    // Choose which axis the platform rotates around
    // X = forward/backward tilt
    // Y = left/right spinning rotation
    // Z = side-to-side tilt
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Rotate the platform around the selected axis
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
