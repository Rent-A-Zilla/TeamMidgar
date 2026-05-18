using System.Collections;
using UnityEngine;

public class fallingFloorTile : MonoBehaviour
{
    [Header("Fall Settings")]
    public float fallDelay = 1f;

    [Tooltip("If true, the tile will return after falling.")]
    public bool respawnTile = false;

    public float respawnDelay = 3f;

    [Header("Shake Settings")]
    public float shakeAmount = 0.04f;
    public float shakeSpeed = 25f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    private bool isFalling = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void StartFalling()
    {
        if (!isFalling)
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true;

        float timer = 0f;

        while (timer < fallDelay)
        {
            float timeLeft = fallDelay - timer;

            if (gameManager.instance != null)
            {
                gameManager.instance.showFallingPlatformTimer();
                gameManager.instance.updateFallingPlatformTimer(timeLeft);
            }

            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float shakeZ = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

            transform.position = startPosition + new Vector3(shakeX, 0f, shakeZ);

            timer += Time.deltaTime;
            yield return null;
        }

        if (gameManager.instance != null)
        {
            gameManager.instance.hideFallingPlatformTimer();
        }

        transform.position = startPosition;

        rb.isKinematic = false;
        rb.useGravity = true;

        if (respawnTile)
        {
            yield return new WaitForSeconds(respawnDelay);
            ResetTile();
        }
    }

    private void ResetTile()
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = startRotation;

        isFalling = false;
    }
}