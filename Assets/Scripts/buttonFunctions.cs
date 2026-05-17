using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void openShop()
    {
        gameManager.instance.openShop();

    }

    public void closeShop()
    {
        gameManager.instance.closeShop();
    }

    public void buyHealthUpgrade()
    {
        if (gameManager.instance.spendCurrency(100))
        {
            gameManager.instance.playerScript.upgradeMaxHealth(25);

            gameManager.instance.shop.updateShopUI();
        }
    }

    public void buyStaminaUpgrade()
    {
        if (gameManager.instance.spendCurrency(100))
        {
            gameManager.instance.playerScript.upgradeMaxStamina(25);

            gameManager.instance.shop.updateShopUI();
        }
    }

    public void buyJumpUpgrade()
    {
        if (gameManager.instance.spendCurrency(150))
        {
            gameManager.instance.playerScript.upgradeJumpMax(1);

            gameManager.instance.shop.updateShopUI();
        }
    }
}
