using UnityEngine;

public class weaponManager : MonoBehaviour
{
    [SerializeField] Transform weaponHolder;

    GameObject currentWeapon;

    public void pickupWeapon(GameObject weaponPrefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponPrefab, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }

}
