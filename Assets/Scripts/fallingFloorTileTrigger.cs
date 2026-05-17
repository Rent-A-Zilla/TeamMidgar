using UnityEngine;

public class fallingFloorTileTrigger : MonoBehaviour
{
    private fallingFloorTile fallingTile;

    void Start()
    {
        fallingTile = GetComponentInParent<fallingFloorTile>();

        if (fallingTile == null)
        {
            Debug.LogError("No FallingFloorTile script found on parent object.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fallingTile.StartFalling();
        }
    }
}