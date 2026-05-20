using System.Collections;
using UnityEngine;

public class fallingWallTriggerZone : MonoBehaviour
{
    [Header("Walls To Fall")]
    public fallingWall[] wallsToFall;

    [Header("Trigger Settings")]
    public bool triggerOnlyOnce = true;
    public float delayBetweenWalls = 0.3f;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered)
                return;

            hasTriggered = true;
            StartCoroutine(TriggerWalls());
        }
    }

    private IEnumerator TriggerWalls()
    {
        foreach (fallingWall wall in wallsToFall)
        {
            if (wall != null)
            {
                wall.StartFalling();
            }

            yield return new WaitForSeconds(delayBetweenWalls);
        }
    }
}