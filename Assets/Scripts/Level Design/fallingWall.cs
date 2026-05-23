using System.Collections;
using UnityEngine;

public class fallingWall : MonoBehaviour
{
    [Header("Fall Settings")]
    public float fallDelay = 0f;
    public float fallSpeed = 90f;

    [Tooltip("Direction the wall falls: Forward, Backward, Left, or Right.")]
    public FallDirection fallDirection = FallDirection.Forward;

    [Tooltip("How far the wall rotates when falling.")]
    public float fallAngle = 90f;

    [Header("Damage Settings")]
    public int damageAmount = 20;
    public bool damageOnlyOnce = true;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip fallingSound;
    public bool playSoundOnlyOnce = true;

    private bool isFalling;
    private bool hasDamaged;
    private bool hasPlayedSound;

    private Quaternion startRotation;
    private Quaternion targetRotation;

    public enum FallDirection
    {
        Forward,
        Backward,
        Left,
        Right
    }

    void Start()
    {
        startRotation = transform.rotation;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
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

        yield return new WaitForSeconds(fallDelay);

        PlayFallingSound();

        Vector3 rotationAxis = GetFallAxis();

        targetRotation = Quaternion.AngleAxis(fallAngle, rotationAxis) * startRotation;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                fallSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = targetRotation;
    }

    private void PlayFallingSound()
    {
        if (audioSource == null || fallingSound == null)
            return;

        if (playSoundOnlyOnce && hasPlayedSound)
            return;

        audioSource.PlayOneShot(fallingSound);
        hasPlayedSound = true;
    }

    private Vector3 GetFallAxis()
    {
        switch (fallDirection)
        {
            case FallDirection.Forward:
                return transform.right;

            case FallDirection.Backward:
                return -transform.right;

            case FallDirection.Left:
                return transform.forward;

            case FallDirection.Right:
                return -transform.forward;

            default:
                return transform.right;
        }
    }

    public bool CanDamage()
    {
        if (!isFalling)
            return false;

        if (damageOnlyOnce && hasDamaged)
            return false;

        return true;
    }

    public void DamageTarget(IDamage target)
    {
        if (!CanDamage())
            return;

        target.takeDamage(damageAmount);
        hasDamaged = true;
    }
}