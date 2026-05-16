using System.Collections;
using UnityEngine;

public class fallingPlatform : MonoBehaviour
{
    [Header("Fall Settings")]
    public float fallDelay = 1f;

    [Tooltip("If true, the platform will return after falling.")]
    public bool respawnPlatform = true;

    public float respawnDelay = 3f;

    [Header("Shake Settings")]
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 25f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    private bool isFalling = false;

    private fallingPlatformTimerUI timerUI;

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

        timerUI = FindFirstObjectByType<fallingPlatformTimerUI>();

        if (timerUI != null)
        {
            timerUI.HideTimer();
        }
        else
        {
            Debug.LogWarning("No FallingPlatformTimerUI found in the scene.");
        }
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

            if (timerUI != null)
            {
                timerUI.ShowTimer(timeLeft);
            }

            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float shakeZ = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

            transform.position = startPosition + new Vector3(shakeX, 0f, shakeZ);

            timer += Time.deltaTime;
            yield return null;
        }

        if (timerUI != null)
        {
            timerUI.HideTimer();
        }

        transform.position = startPosition;

        rb.isKinematic = false;
        rb.useGravity = true;

        if (respawnPlatform)
        {
            yield return new WaitForSeconds(respawnDelay);
            ResetPlatform();
        }
    }

    private void ResetPlatform()
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