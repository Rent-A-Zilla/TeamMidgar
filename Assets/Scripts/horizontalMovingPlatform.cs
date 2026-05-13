using UnityEngine;

public class horizontalMovingPlatform : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]

    // How far the platform moves left and right from its starting position
    public float moveDistance = 2f;

    // How fast the platform moves left and right
    public float moveSpeed = 2f;

    // Stores the platform's original position when the game starts
    private Vector3 startPosition;

    void Start()
    {
        // Save the starting position so the platform moves around this point
        startPosition = transform.position;
    }

    void Update()
    {
        // Creates smooth back-and-forth movement over time
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        // Move the platform left and right on the X axis
        // X = left/right, Y = up/down, Z = forward/backward
        transform.position = startPosition + new Vector3(movement, 0f, 0f);
    }
}