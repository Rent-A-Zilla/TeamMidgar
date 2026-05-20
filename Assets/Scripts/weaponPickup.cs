using System;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Transform gunHolder;
    [SerializeField] GameObject weaponVisual;


    void Start()
    {
        GameObject visual = Instantiate(weaponVisual, gunHolder);

        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
    }

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
