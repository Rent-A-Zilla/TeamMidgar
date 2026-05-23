using UnityEngine;

public class RollingRockTrigger : MonoBehaviour
{
    public RollingRock[] rocksToRelease;
    public bool triggerOnlyOnce = true;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        foreach (RollingRock rock in rocksToRelease)
        {
            if (rock != null)
            {
                rock.StartRolling();
            }
        }
    }
}