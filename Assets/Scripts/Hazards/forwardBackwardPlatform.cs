using UnityEngine;

public class ForwardBackwardPlatform : MonoBehaviour
{
    [Header("Back-and-forth Movement Settings")]

    // How far the platform moves forward and backward from its starting position
    public float moveDistance = 2f;

    // How fast the platform moves
    public float moveSpeed = 1f;

    // Stores the original position of the platform when the game starts
    private Vector3 startPosition;

    void Start()
    {
        // Save the platform's starting position so it can move around this point
        startPosition = transform.position;
    }

    void Update()
    {
        // Mathf.Sin creates a smooth back-and-forth movement over time
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        // Move the platform forward and backward on the Z axis
        // X = left/right, Y = up/down, Z = forward/backward
        transform.position = startPosition + new Vector3(0f, 0f, movement);
    }
}
