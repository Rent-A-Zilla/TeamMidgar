using UnityEngine;

public class RollingRock : MonoBehaviour
{
    [Header("Movement Settings")]
    public float rollForce = 20f;
    public bool rollTowardPlayer = true;
    public Vector3 manualRollDirection = Vector3.forward;

    [Header("Damage Settings")]
    public int damageAmount = 25;
    public bool damageOnlyOnce = true;

    [Header("Visual Settings")]
    public Color rockColor = Color.gray;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip rollingSound;
    public AudioClip hitSound;
    public bool loopRollingSound = true;

    private Rigidbody rb;
    private Renderer rockRenderer;
    private MaterialPropertyBlock propertyBlock;

    private bool hasStartedRolling;
    private bool hasDamaged;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rockRenderer = GetComponent<Renderer>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        rb.useGravity = true;
        rb.isKinematic = true;

        ApplyRockColor();
    }

    void Start()
    {
        ApplyRockColor();
    }

    public void StartRolling()
    {
        if (hasStartedRolling)
            return;

        hasStartedRolling = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        PlayRollingSound();

        Vector3 rollDirection = manualRollDirection.normalized;

        if (rollTowardPlayer && gameManager.instance != null && gameManager.instance.player != null)
        {
            rollDirection = gameManager.instance.player.transform.position - transform.position;
            rollDirection.y = 0f;
            rollDirection.Normalize();
        }

        rb.AddForce(rollDirection * rollForce, ForceMode.Impulse);
    }

    private void PlayRollingSound()
    {
        if (audioSource == null || rollingSound == null)
            return;

        audioSource.clip = rollingSound;
        audioSource.loop = loopRollingSound;
        audioSource.Play();
    }

    private void PlayHitSound()
    {
        if (audioSource == null || hitSound == null)
            return;

        audioSource.PlayOneShot(hitSound);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasStartedRolling)
            return;

        PlayHitSound();

        IDamage damageable = collision.gameObject.GetComponent<IDamage>();

        if (damageable != null)
        {
            if (damageOnlyOnce && hasDamaged)
                return;

            damageable.takeDamage(damageAmount);
            hasDamaged = true;
        }
    }

    private void OnValidate()
    {
        ApplyRockColor();
    }

    private void ApplyRockColor()
    {
        if (rockRenderer == null)
        {
            rockRenderer = GetComponent<Renderer>();
        }

        if (rockRenderer == null)
            return;

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        rockRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_BaseColor", rockColor);
        propertyBlock.SetColor("_Color", rockColor);

        rockRenderer.SetPropertyBlock(propertyBlock);
    }
}