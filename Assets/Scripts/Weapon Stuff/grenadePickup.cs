using UnityEngine;

public class grenadePickup : MonoBehaviour
{
    [SerializeField] grenadeStats grenade;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null)
        {
            pik.getGrenadeStats(grenade);
            Destroy(gameObject);
        }
    }
}