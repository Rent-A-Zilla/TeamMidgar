using UnityEngine;

public class verticalMovingPlatform : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = startPosition + new Vector3(0f, movement, 0f);
    }
}
