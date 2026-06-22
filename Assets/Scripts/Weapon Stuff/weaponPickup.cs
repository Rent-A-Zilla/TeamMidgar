using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

public class weaponPickup : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;
    private bool pik;

    [ContextMenu("Generate guid for id")]

    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {

        IPickup pickup = other.GetComponent<IPickup>();

        if (pickup != null)
        {
            pickup.getGunStats(gun);

            pik = true;

            DataPersistenceManager.instance.SaveGame();

            gameObject.SetActive(false);
        }
    }

    public void LoadData(GameData data)
    {
        if (data.weaponsCollected.Contains(id))
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (pik && !data.collectedWeaponNames.Contains(gun.name))
        {
            data.collectedWeaponNames.Add(gun.name);
        }
    }
}
