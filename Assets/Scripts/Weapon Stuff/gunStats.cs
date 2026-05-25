using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public enum fireType
    {
        SemiAuto,
        FullAuto
    }

    [Header("-----Gun Model-----")]
    public GameObject gunModel;

    [Header("-----Gun Settings-----")]
    public fireType gunFireType;
    [Range(1, 100)] public int shootDamage;
    [Range(5, 1000)] public int shootDist;
    [Range(0.1f, 2)] public float shootRate;

    [Header("-----Ammo-----")]
    public int ammoCur;
    [Range(0, 100)] public int ammoMax;
    public int ammoReserve;
    public int ammoReserveMax;

    [Header("-----Pellets / Spread-----")]
    public int pellets = 1;
    public float spreadAmount;

    [Header("-----Effects-----")]
    public ParticleSystem hiteffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
}