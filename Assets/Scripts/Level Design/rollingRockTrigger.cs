using System.Collections;
using UnityEngine;

public class RollingRockTrigger : MonoBehaviour
{
    [Header("Rocks To Release")]
    public RollingRock[] rocksToRelease;

    [Header("Trigger Settings")]
    public bool triggerOnlyOnce = true;

    [Header("Chain Reaction Settings")]
    public bool useChainReaction = true;
    public float delayBetweenRocks = 0.5f;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (useChainReaction)
        {
            StartCoroutine(ReleaseRocksOneByOne());
        }
        else
        {
            ReleaseAllRocksAtOnce();
        }
    }

    private IEnumerator ReleaseRocksOneByOne()
    {
        foreach (RollingRock rock in rocksToRelease)
        {
            if (rock != null)
            {
                rock.StartRolling();
            }

            yield return new WaitForSeconds(delayBetweenRocks);
        }
    }

    private void ReleaseAllRocksAtOnce()
    {
        foreach (RollingRock rock in rocksToRelease)
        {
            if (rock != null)
            {
                rock.StartRolling();
            }
        }
    }
}