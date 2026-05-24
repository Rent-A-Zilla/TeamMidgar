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

        if (obj.CompareTag("Player"))
        {
            return;
        }

        if (stats.type == grenadeStats.grenadeType.AntiGravity)
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