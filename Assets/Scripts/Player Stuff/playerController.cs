using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickup, IGrenade
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int HP;

    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] float speed;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModelFPS;
    [SerializeField] GameObject gunModelTPS;

    [SerializeField] int sprintMod;
    [SerializeField] int maxStamina;
    [SerializeField] float staminaDrainRate;
    [SerializeField] float staminaRegenRate;
    [SerializeField] float staminaRegenDelay;

    [SerializeField] List<grenadeStats> grenadeList = new List<grenadeStats>();
    [SerializeField] List<int> grenadeCounts = new List<int>();
    [SerializeField] Transform grenadeThrowPoint;

    [SerializeField] AudioSource audPlayer;

    float currentStamina;

    int gunListPos;
    int grenadeListPos;
    int jumpMaxOrig;
    int jumpCount;
    int HPOrig;
    float speedOrig;

    float shootTimer;

    bool isSprinting;

    Vector3 moveDir;
    Vector3 playerVel;

    Coroutine staminaRegenCoroutine;

    //Getters
    public int getHP()
    {
        return HP;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        currentStamina = maxStamina;
        jumpMaxOrig = jumpMax;
        speedOrig = speed;
        updatePlayerHPUI();
        updatePlayerSprintUI();
        
        gameManager.instance.jumpMaxTimerUI.SetActive(false);
        gameManager.instance.speedUpTimerUI.SetActive(false);

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

    //Player functions
    void movement()
    {
        if (gunList.Count > 0)
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunList[gunListPos].shootDist, Color.blue);

        shootTimer += Time.deltaTime;

        if (gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer > gunList[gunListPos].shootRate)
        {
            if (gunList[gunListPos].gunFireType == gunStats.fireType.FullAuto && Input.GetButton("Fire1"))
            {
                shoot();
            }
            else if (gunList[gunListPos].gunFireType == gunStats.fireType.SemiAuto && Input.GetButtonDown("Fire1"))
            {
                shoot();
            }
        }

        if (Input.GetButtonDown("Throw"))
        {
            throwGrenade();
        }

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

        selectGun();
        reload();
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && gunList.Count > 0)
        {
            gunStats gun = gunList[gunListPos];

            int ammoNeeded = gun.ammoMax - gun.ammoCur;

            if (gun.ammoReserve <= 0 || ammoNeeded <= 0)
                return;

            int ammoToReload = Mathf.Min(ammoNeeded, gun.ammoReserve);

            gun.ammoCur += ammoToReload;
            gun.ammoReserve -= ammoToReload;

            updateAmmoUI();
        }
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
        gunList[gunListPos].ammoCur--;

        updateAmmoUI();

        for (int i = 0; i < gunList[gunListPos].pellets; i++)
        {
            Vector3 direction = Camera.main.transform.forward;

            direction += Camera.main.transform.right * Random.Range(-gunList[gunListPos].spreadAmount, gunList[gunListPos].spreadAmount);
            direction += Camera.main.transform.up * Random.Range(-gunList[gunListPos].spreadAmount, gunList[gunListPos].spreadAmount);

            Debug.DrawRay(Camera.main.transform.position, direction * gunList[gunListPos].shootDist, Color.red, 1f);

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, gunList[gunListPos].shootDist, ~ignoreLayer))
            {
                Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(gunList[gunListPos].shootDamage);
                }
            }
        }
    }

    void throwGrenade()
    {
        if (grenadeList.Count <= 0)
        {
            return;
        }

        grenadeStats grenade = grenadeList[grenadeListPos];

        if (grenadeCounts[grenadeListPos] <= 0)
        {
            return;
        }

        Vector3 spawnPos = transform.position + transform.forward * 1.5f + Vector3.up * 1.0f;

        GameObject grenadeObj = Instantiate(grenade.grenadePrefab, spawnPos, Camera.main.transform.rotation);

        Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        if (rb != null)
        {
            rb.AddForce(Camera.main.transform.forward * grenade.throwForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * grenade.upwardForce, ForceMode.Impulse);
        }

        grenadeProjectile projectile = grenadeObj.GetComponent<grenadeProjectile>();

        if (projectile != null)
        {
            projectile.setStats(grenade);
        }

        grenadeCounts[grenadeListPos]--;

        if (grenadeCounts[grenadeListPos] <= 0)
        {
            grenadeList.RemoveAt(grenadeListPos);
            grenadeCounts.RemoveAt(grenadeListPos);

            if (grenadeListPos > 0)
            {
                grenadeListPos--;
            }
        }
    }

    //Player handling
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

    public void getGunStats(gunStats gun)
    {
        int existingGun = gunList.IndexOf(gun);

        if (existingGun >= 0)
        {
            gun.ammoReserve = gun.ammoReserveMax;
            gunListPos = existingGun;
        }
        else
        {
            gun.ammoCur = gun.ammoMax;
            gun.ammoReserve = gun.ammoReserveMax;

            gunList.Add(gun);
            gunListPos = gunList.Count - 1;
        }

        changeGun();
    }

    void changeGun()
    {
        gunModelFPS.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModelFPS.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        gunModelTPS.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModelTPS.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        updateAmmoUI();
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
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

    //Power Ups (Temporary)
    public void healthUP(int amount)
    {
        HP = HP + amount;
        if (HP > HPOrig)
        {
            HP = HPOrig;
        }
        updatePlayerHPUI();
    }

    public void jumpMaxUp(int amount, float duration)
    {
        StartCoroutine(jumpMaxUPRoutine(amount, duration));
    }
    private IEnumerator jumpMaxUPRoutine(int amount, float duration)
    {

        gameManager.instance.jumpMaxTimerUI.SetActive(true);
        jumpMax += amount;
        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            gameManager.instance.jumpMaxUpTimer.fillAmount = timer / duration;

            yield return null;
        }
        jumpMax = jumpMaxOrig;

        gameManager.instance.jumpMaxTimerUI.SetActive(false);
    }
    public void speedUp(float amount, float duration)
    {
        StartCoroutine(speedUPRoutine(amount, duration));
    }
    private IEnumerator speedUPRoutine(float amount, float duration)
    {

        gameManager.instance.speedUpTimerUI.SetActive(true);
        speed += amount;
        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            gameManager.instance.speedUpTimer.fillAmount = timer / duration;

            yield return null;
        }
        speed = speedOrig;

        gameManager.instance.speedUpTimerUI.SetActive(false);
    }

    //Player Upgrades (Permanent)
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
        jumpMaxOrig += amount;
        jumpMax = jumpMaxOrig;
    }

    public void applyGrenadeEffects(grenadeStats grenade, Vector3 explosionPoint)
    {
        if (grenade.type == grenadeStats.grenadeType.Explosive)
        {
            takeDamage(grenade.damage);
        }
        else if (grenade.type == grenadeStats.grenadeType.AntiGravity)
        {
            playerVel.y = grenade.effectForce;

            controller.Move(Vector3.up * 0.2f);
        }
    }

    public void getGrenadeStats(grenadeStats grenade)
    {
        int existingGrenade = grenadeList.IndexOf(grenade);

        if (existingGrenade >= 0)
        {
            grenadeCounts[existingGrenade]++;
        }
        else
        {
            grenadeList.Add(grenade);
            grenadeCounts.Add(1);

            grenadeListPos = grenadeList.Count - 1;
        }
    }

    void updateAmmoUI()
    {
        if (gunList.Count <= 0)
            return;

        gameManager.instance.updateAmmoUI(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoReserve);
    }
}
