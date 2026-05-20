using UnityEngine;

public class fallingWallDamageZone : MonoBehaviour
{
    private fallingWall fallingWall;

    void Start()
    {
        fallingWall = GetComponentInParent<fallingWall>();

        if (fallingWall == null)
        {
            Debug.LogError("No FallingWall script found on parent object.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null && fallingWall != null)
        {
            fallingWall.DamageTarget(damageable);
        }
    }
}