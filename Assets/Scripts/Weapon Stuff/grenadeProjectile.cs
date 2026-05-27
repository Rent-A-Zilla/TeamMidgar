using UnityEngine;
using System.Collections;

public class grenadeProjectile : MonoBehaviour
{
    grenadeStats stats;
    bool hasExploded;

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
        if (stats == null)
        {
            return;
        }
        // Prevent grenade from exploding immediately on player
        if (obj.CompareTag("Player"))
        {
            return;
        }
        // Anti-gravity and knockback grenades explode on impact
        if (stats.type == grenadeStats.grenadeType.AntiGravity ||stats.type == grenadeStats.grenadeType.Knockback)
        {
            explode();
        }
    }

    void explode()
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;

        if (stats.explosionSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(stats.explosionSound[Random.Range(0, stats.explosionSound.Length)], transform.position, stats.explosionSoundVol );
        }

        if (stats.explosionEffect != null)
        {
            Instantiate(stats.explosionEffect, transform.position, Quaternion.identity);
        }

        // Find all colliders inside explosion radius
        Collider[] hits = Physics.OverlapSphere(transform.position, stats.radius);

        foreach (Collider hit in hits)
        {
            IGrenade effect = hit.GetComponent<IGrenade>();

            if (effect != null)
            {
                effect.applyGrenadeEffects(stats, transform.position);
            }
        }

        Destroy(gameObject);
    }
}