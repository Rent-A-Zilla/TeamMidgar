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

    public void respawnPlayer()
    {
        gameManager.instance.playerScript.changPlayerPosition();
        gameManager.instance.stateUnpause();
    }

    public void play()
    {
       
        SceneManager.LoadScene("Final Level Proto 2");
        gameManager.instance.stateUnpause();
    }

    public void tutorial()
    {
       
        SceneManager.LoadScene("Tutorial");
        gameManager.instance.stateUnpause();
    }

    public void quiteToMenu()
    {
        SceneManager.LoadScene("Main Menu");
        gameManager.instance.statePause(); 
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void openOptions()
    {
        gameManager.instance.openOptionsMenu();
    }

    public void closeOptions()
    {
        gameManager.instance.backToPauseMenu();
    }

    public void CameraViewButton()
    {
        gameManager.instance.ToggleCameraView();
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
