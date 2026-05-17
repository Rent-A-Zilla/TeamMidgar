using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int HP;

    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] float speed;
    

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [SerializeField] int sprintMod;
    [SerializeField] int maxStamina;
    [SerializeField] float staminaDrainRate;
    [SerializeField] float staminaRegenRate;
    [SerializeField] float staminaRegenDelay;

    float currentStamina;

    int jumpCount;
    int HPOrig;

    float shootTimer;

    bool isSprinting;

    Vector3 moveDir;
    Vector3 playerVel;

    Coroutine staminaRegenCoroutine;

    public int getHP()
    {
        return HP;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStamina = maxStamina;
        updatePlayerSprintUI();

        HPOrig = HP;
        updatePlayerHPUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            movement();
        }
        sprint();
        handleSprintUI();
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer > shootRate)
            shoot();

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir.normalized * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;
    }

    void sprint()
    {
        bool sprintButtonHeld = Input.GetButton("Sprint");

        if (sprintButtonHeld && currentStamina > 0)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                speed *= sprintMod;
            }

            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (staminaRegenCoroutine != null)
            {
                StopCoroutine(staminaRegenCoroutine);
                staminaRegenCoroutine = null;
            }

            if (currentStamina <= 0)
            {
                isSprinting = false;
                speed /= sprintMod;
                staminaRegenCoroutine = StartCoroutine(RechargeStaminaAfterDelay(staminaRegenDelay));
            }
        }
        else
        {
            if (isSprinting)
            {
                isSprinting = false;
                speed /= sprintMod;
            }

            if (currentStamina < maxStamina && staminaRegenCoroutine == null)
            {
                staminaRegenCoroutine = StartCoroutine(RechargeStaminaAfterDelay(staminaRegenDelay));
            }
        }

        updatePlayerSprintUI();
    }

    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerHPUI();
        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            HP = 0;
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerHPUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashDamageScreen()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    private IEnumerator RechargeStaminaAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        while (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            updatePlayerSprintUI();
            yield return null;
        }
        staminaRegenCoroutine = null;
    }

    public void updatePlayerSprintUI()
    {
        gameManager.instance.playerSprintBar.fillAmount = (float)currentStamina / maxStamina;
    }

    void handleSprintUI()
    {
        if (isSprinting || currentStamina < maxStamina)
        {

            gameManager.instance.playerSprintUI.SetActive(true);
        }
        else
        {
            gameManager.instance.playerSprintUI.SetActive(false);
        }
    }
    public int healthUP(int amount)
    {
        HP = HP + amount;
        if(HP > HPOrig)
        {
            HP = HPOrig;
        }
        updatePlayerHPUI();
        return HP;
    }
    public void upgradeMaxHealth(int amount)
    {
        HPOrig += amount;
        HP = HPOrig;

        updatePlayerHPUI();
    }

    public void upgradeMaxStamina(int amount)
    {
        maxStamina += amount;
        currentStamina = maxStamina;

        updatePlayerSprintUI();
    }

    public void upgradeJumpMax(int amount)
    {
        jumpMax += amount;
    }

    public int getMaxHealth()
    {
        return HPOrig;
    }

    public int getMaxStamina()
    {
        return maxStamina;
    }

    public int getMaxJumps()
    {
        return jumpMax;
    }
}
