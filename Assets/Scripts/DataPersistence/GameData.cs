using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public Vector3 playerPosition;

    public List<string> weaponsCollected;
    public List<string> collectedWeaponNames;

    public GameData()
    {
        playerPosition = Vector3.zero;
        weaponsCollected = new List<string>();
        collectedWeaponNames = new List<string>();
    }

}
