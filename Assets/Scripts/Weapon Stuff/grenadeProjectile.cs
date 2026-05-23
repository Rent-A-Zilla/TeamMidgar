using UnityEngine;
using System.Collections;

public class grenadeProjectile : MonoBehaviour
{
    grenadeStats stats;

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

    void explode()
    {
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