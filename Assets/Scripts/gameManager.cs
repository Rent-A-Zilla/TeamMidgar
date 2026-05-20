using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject lavaOverlayUI;
    [SerializeField] GameObject menuShop;


    public shopManager shop;
    public Image playerHPBar;
    public Image playerSprintBar;
    public GameObject playerSprintUI;
    public GameObject playerDamageScreen;
    public TMP_Text gameGoalCountText;
    public TMP_Text lavaTimerText;
    public TMP_Text currencyText;
    public TMP_Text shopCurrencyText;
    public Image jumpMaxUpTimer;
    public GameObject jumpMaxTimerUI;
    public Image speedUpTimer;
    public GameObject speedUpTimerUI;

    public GameObject fallingPlatformTimerUI;
    public TMP_Text fallingPlatformTimerText;


    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    int gameGoalCount;
    int currency;
    float timeScaleOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        

        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void stateUnpause()
    {
        
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");

    }

    public void youLose()
    {
        lavaOverlayUI.SetActive(false);

        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        statePause();

        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void updateLavaTimer(float time)
    {
        lavaTimerText.text = time.ToString("F0");
    }

    public void showLavaOverlay()
    {
        lavaOverlayUI.SetActive(true);
    }

    public void hideLavaOverlay()
    {
        lavaOverlayUI.SetActive(false);
    }
    public void addCurrency(int amount)
    {
        currency += amount;
        updateCurrencyUI();
    }

    void updateCurrencyUI()
    {
        currencyText.text = currency.ToString("F0");

        if (shopCurrencyText != null)
        {
            shopCurrencyText.text = currency.ToString("F0");
        }
    }

    public bool spendCurrency(int amount)
    {
        if (currency >= amount)
        {
            currency -= amount;
            updateCurrencyUI();
            return true;
        }
        return false;
    }

    public void openShop()
    {
        menuActive.SetActive(false);

        menuActive = menuShop;
        menuActive.SetActive(true);

        shop.updateShopUI();
    }

    public void closeShop()
    {
        menuActive.SetActive(false);

        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void updateFallingPlatformTimer(float time)
    {
        if (fallingPlatformTimerText != null)
        {
            fallingPlatformTimerText.text = time.ToString("F1");
        }
    }

    public void showFallingPlatformTimer()
    {
        if (fallingPlatformTimerUI != null)
        {
            fallingPlatformTimerUI.SetActive(true);

            CanvasGroup canvasGroup = fallingPlatformTimerUI.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }

    public void hideFallingPlatformTimer()
    {
        if (fallingPlatformTimerUI != null)
        {
            CanvasGroup canvasGroup = fallingPlatformTimerUI.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            else
            {
                fallingPlatformTimerUI.SetActive(false);
            }
        }
    }
}
