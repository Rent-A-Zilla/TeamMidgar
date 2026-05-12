using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = startPosition + new Vector3(movement, 0f, 0f);
    }
}
