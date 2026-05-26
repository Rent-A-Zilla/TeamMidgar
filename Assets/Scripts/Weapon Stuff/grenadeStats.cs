using UnityEngine;

[CreateAssetMenu]
public class grenadeStats : ScriptableObject
{
    public enum grenadeType
    {
        Explosive,
        AntiGravity,
        Knockback
    }

    public grenadeType type;

    public GameObject grenadePrefab;

    [Header("Throw Settings")]
    public float throwForce = 15f;
    public float upwardForce = 3f;
    public float fuseTime = 3f;

    [Header("Effect Settings")]
    public float radius = 6f;
    public int damage = 50;
    public float effectForce = 10f;
    public float horizontalForceMult = 2f;
    public float upwardBonus = 0.35f;

    [Header("-----Effects-----")]
    public GameObject explosionEffect;
    public AudioClip[] explosionSound;
    public float explosionSoundVol;
}