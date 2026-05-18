using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            weaponManager manager = other.GetComponent<weaponManager>();

            if (manager != null )
            {
                manager.pickupWeapon(weaponPrefab);
                Destroy(gameObject);
            }
        }
    }

}
