using UnityEngine;

public class VerticalMovingPlatform : MonoBehaviour
{
    [Header("Vertical Movement Settings")]

    // How far the platform moves up and down from its starting position
    public float moveDistance = 2f;

    // How fast the platform moves up and down
    public float moveSpeed = 1f;

    // Stores the platform's original position when the game starts
    private Vector3 startPosition;

    void Start()
    {
        // Save the starting position so the platform moves around this point
        startPosition = transform.position;
    }

    void Update()
    {
        // Creates smooth up-and-down movement over time
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        // Move the platform up and down on the Y axis
        // X = left/right, Y = up/down, Z = forward/backward
        transform.position = startPosition + new Vector3(0f, movement, 0f);
    }
}
