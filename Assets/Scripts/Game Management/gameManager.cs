using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Menus -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject lavaOverlayUI;
    [SerializeField] GameObject menuShop;


    [Header("----- Cams -----")]
    [SerializeField] GameObject firstPersonCam;
    [SerializeField] GameObject thirdPersonCam;
    [SerializeField] GameObject weaponHolderTPS;
    [SerializeField] MonoBehaviour cameraScript;


    [Header("----- Shop -----")]
    public shopManager shop;

    [Header("----- HUD UI -----")]
    public Image playerHPBar;
    public Image playerSprintBar;
    public GameObject playerSprintUI;
    public GameObject playerDamageScreen;
    public TMP_Text gameGoalCountText;
    public GameObject lavaTimerUI;
    public TMP_Text lavaTimerText;
    public TMP_Text currencyText;
    public TMP_Text shopCurrencyText;
    public Image jumpMaxUpTimer;
    public GameObject jumpMaxTimerUI;
    public Image speedUpTimer;
    public GameObject speedUpTimerUI;
    public TMP_Text grenadeText;

    [Header("Player Check Point Components")]
    public GameObject checkpointPopup;
    public GameObject playerStartPos;

    [Header("----- Ammo UI -----")]
    public TMP_Text ammoText;

    [Header("----- Player -----")]
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public bool isFirstPerson;

    [Header("----- Win Condition / Score -----")]
    public TMP_Text levelTimerText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text achievementText;

    [SerializeField] float levelTimeLimit;
    [SerializeField] int killScoreAmount;
    [SerializeField] int timeBonusMultiplier;

    float levelTimer;
    int score;
    bool levelEnded;

    int gameGoalCount;
    int currency;
    float timeScaleOrig;
    int enemiesInCombat = 0;
    public bool inCombat;
    public bool nearby;
    bool isDying;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        levelTimer = levelTimeLimit;

        if (achievementText != null)
            achievementText.gameObject.SetActive(false);

        updateScoreUI();

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player Not Found");
            return;
        }

        timeScaleOrig = Time.timeScale;


        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
            player = GameObject.FindWithTag("Player");
            firstPersonCam = player.transform.Find("FirstPersonView").gameObject;
            thirdPersonCam = player.transform.Find("ThirdPersonView").gameObject;
            weaponHolderTPS = player.transform.Find("Gun Holder TPS").gameObject;

            isFirstPerson = true;

            firstPersonCam.SetActive(true);
            thirdPersonCam.SetActive(false);
            weaponHolderTPS.SetActive(false);
        }

        playerStartPos = GameObject.FindWithTag("Player Start Pos");

        if (lavaTimerUI != null)
            lavaTimerUI.SetActive(false);

        if (musicManager.instance != null)
            musicManager.instance.playBackgroundMusic();
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

        if (!levelEnded && !isPaused)
        {
            levelTimer -= Time.deltaTime;

            if (levelTimer <= 0)
            {
                levelTimer = 0;
                levelEnded = true;
                youLose();
            }

            updateScoreUI();
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

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount < 0)
            gameGoalCount = 0;

        if (amount < 0)
            addScore(killScoreAmount);

        if (gameGoalCountText != null)
            gameGoalCountText.text = gameGoalCount.ToString("F0");
    }
    public void playerReachedExit()
    {
        if (gameGoalCount <= 0)
        {
            completeLevel();
        }
    }

    void completeLevel()
    {
        if (levelEnded)
            return;

        levelEnded = true;

        int timeBonus = Mathf.RoundToInt(levelTimer * timeBonusMultiplier);
        addScore(timeBonus);

        checkAchievements();

        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }

        updateScoreUI();
        youWin();
    }

    void addScore(int amount)
    {
        score += amount;
        updateScoreUI();
    }

    void updateScoreUI()
    {
        if (levelTimerText != null)
        {
            int minutes = Mathf.FloorToInt(levelTimer / 60);
            int seconds = Mathf.FloorToInt(levelTimer % 60);

            levelTimerText.text = minutes + ":" + seconds.ToString("00");
        }

        if (scoreText != null)
            scoreText.text = score.ToString();

        if (highScoreText != null)
            highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    void checkAchievements()
    {
        if (levelTimer >= levelTimeLimit * 0.5f)
            unlockAchievement("Speed Runner");

        if (gameGoalCount <= 0)
            unlockAchievement("No Survivors");

        if (score >= 1000)
            unlockAchievement("High Roller");
    }

    void unlockAchievement(string achievementName)
    {
        if (PlayerPrefs.GetInt("Achievement_" + achievementName, 0) == 1)
            return;

        PlayerPrefs.SetInt("Achievement_" + achievementName, 1);
        PlayerPrefs.Save();

        if (achievementText != null)
        {
            achievementText.gameObject.SetActive(true);
            achievementText.text = achievementName;
        }
    }

    public void youLose()
    {
        if (isDying)
            return;

        isDying = true;

        StartCoroutine(deathSequence());
    }

    IEnumerator deathSequence()
    {
        if (playerScript != null)
            playerScript.enabled = false;

        if (cameraScript != null)
            cameraScript.enabled = false;

        if (playerScript != null)
            playerScript.enabled = false;

        float timer = 0f;
        float deathFallTime = 2f;

        Quaternion startRot = firstPersonCam.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, 90);

        while (timer < deathFallTime)
        {
            timer += Time.deltaTime;

            float t = timer / deathFallTime;

            firstPersonCam.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        firstPersonCam.transform.rotation = endRot;

        yield return new WaitForSeconds(1f);

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

    public void openOptionsMenu()
    {
        statePause();

        menuPause.SetActive(false);
        menuOptions.SetActive(true);
    }

    public void backToPauseMenu()
    {
        statePause();

        menuOptions.SetActive(false);
        menuPause.SetActive(true);
    }

    public void ToggleCameraView()
    {
        isFirstPerson = !isFirstPerson;

        firstPersonCam.SetActive(isFirstPerson);
        thirdPersonCam.SetActive(!isFirstPerson);

        weaponHolderTPS.SetActive(!isFirstPerson);

        menuOptions.SetActive(false);
        stateUnpause();
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
        statePause();

        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }

        menuActive = menuShop;
        menuActive.SetActive(true);

        shop.updateShopUI();
    }

    public void closeShop()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }

        menuActive = null;

        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    public void updateAmmoUI(int currentAmmo, int maxAmmo)
    {
        ammoText.text = currentAmmo + " / " + maxAmmo;
    }

    public void showLavaTimer()
    {
        lavaTimerUI.gameObject.SetActive(true);
    }

    public void hideLavaTimer()
    {
        lavaTimerUI.gameObject.SetActive(false);
    }
    public void updateGrenadeUI(string grenadeName, int grenadeCount)
    {
        grenadeText.text = grenadeName + " x" + grenadeCount;
    }

    public void EnemyEnteredCombat()
    {
        enemiesInCombat++;
        if (enemiesInCombat == 1)
        {
            //inCombat = true;
            //musicManager.instance.playBattleMusic();
        }
    }

    public void EnemyExitedCombat()
    {
        enemiesInCombat = Mathf.Max(0, enemiesInCombat - 1);
        if(enemiesInCombat == 0)
        {
            //inCombat = false;

            //musicManager.instance.playBackgroundMusic();
        }
    }
}
