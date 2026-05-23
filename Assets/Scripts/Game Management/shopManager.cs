using UnityEngine;
using TMPro;

public class shopManager : MonoBehaviour
{
    [SerializeField] TMP_Text maxHealthValue;
    [SerializeField] TMP_Text maxStaminaValue;
    [SerializeField] TMP_Text maxJumpValue;

    public void updateShopUI()
    {
        maxHealthValue.text =
            gameManager.instance.playerScript.getMaxHealth().ToString();

        maxStaminaValue.text =
            gameManager.instance.playerScript.getMaxStamina().ToString();

        maxJumpValue.text =
            gameManager.instance.playerScript.getMaxJumps().ToString();
    }
}