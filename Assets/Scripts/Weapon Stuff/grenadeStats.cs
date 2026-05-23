using UnityEngine;

[CreateAssetMenu]
public class grenadeStats : ScriptableObject
{
    public enum grenadeType
    {
        Explosive,
        AntiGravity
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
}