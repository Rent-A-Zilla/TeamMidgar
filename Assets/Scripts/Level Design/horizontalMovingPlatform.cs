using UnityEngine;

public class horizontalMovingPlatform : MonoBehaviour
{
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    private float movement;

    Vector3 startPos;
    Vector3 lastPos;
    Vector3 platformMove;

    CharacterController playerController;

    void Start()
    {
        startPos = transform.position;
        lastPos = transform.position;
    }

    private void Update()
    {
        movement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = startPos + new Vector3(movement, 0f, 0f);

        platformMove = transform.position - lastPos;
        lastPos = transform.position;

        if (playerController != null )
        {
            playerController.Move(platformMove);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerController = null;
        }
    }

}