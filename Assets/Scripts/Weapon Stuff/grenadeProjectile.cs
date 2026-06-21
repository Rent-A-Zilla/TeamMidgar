using UnityEngine;
using System.Collections;

public class grenadeProjectile : MonoBehaviour
{
    grenadeStats stats;
    bool hasExploded;
    AudioSource aud;

    void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    public void setStats(grenadeStats grenadeStats)
    {
        stats = grenadeStats;
        StartCoroutine(explodeTimer());
    }

    IEnumerator explodeTimer()
    {
        yield return new WaitForSeconds(stats.fuseTime);
        explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        checkCollisionExplosion(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        checkCollisionExplosion(collision.gameObject);
    }

    void checkCollisionExplosion(GameObject obj)
    {
        if (stats == null) return;
        if (obj.CompareTag("Player")) return;

        if (stats.type == grenadeStats.grenadeType.AntiGravity ||
            stats.type == grenadeStats.grenadeType.Knockback)
        {
            explode();
        }
    }

    void explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        AudioClip clip = null;

        if (stats.explosionSound.Length > 0 && aud != null)
        {
            clip = stats.explosionSound[Random.Range(0, stats.explosionSound.Length)];
            aud.PlayOneShot(clip, stats.explosionSoundVol);
        }

        if (stats.explosionEffect != null)
        {
            Instantiate(stats.explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, stats.radius);

        foreach (Collider hit in hits)
        {
            IGrenade effect = hit.GetComponent<IGrenade>();

            if (effect != null)
            {
                effect.applyGrenadeEffects(stats, transform.position);
            }
        }

        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null)
            mesh.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        Destroy(gameObject, clip != null ? clip.length : 2f);
    }
}