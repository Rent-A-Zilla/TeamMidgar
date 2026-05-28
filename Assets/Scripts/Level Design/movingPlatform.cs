using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    public enum MoveType
    {
        UpDown,
        Sideways
    }
    [SerializeField] MoveType moveType;
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    Vector3 startPos;
    Vector3 targetOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;

        if (moveType == MoveType.UpDown)
        {
            targetOffset = Vector3.up * moveDistance;
        }
        else if (moveType == MoveType.Sideways)
        {
            targetOffset = transform.right * moveDistance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

        transform.position = Vector3.Lerp(startPos, startPos + targetOffset, movement);
    }
}
