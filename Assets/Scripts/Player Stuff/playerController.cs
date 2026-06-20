using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


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

    [Header("-----Weapon IK-----")]
    [SerializeField] Transform rightHandTarget;
    [SerializeField] Transform leftHandTarget;
    [SerializeField] Transform weaponHolderFPS;
    [SerializeField] weaponIKPoints noWeaponPose;
    GameObject currentGunFPS;
    weaponIKPoints currentIKPoints;

    [Header("-----Dodge-----")]
    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeDuration;
    [SerializeField] float dodgeCooldown;

    [SerializeField] Transform playerCamera;
    [SerializeField] int crouchMod;
    [SerializeField] float crouchCameraOffset;
    [SerializeField] float crouchLerpSpeed;
    [SerializeField] float standLerpSpeed;
    [SerializeField] float crouchHeight;
    [SerializeField] float standHeight;
    [SerializeField] int sprintMod;
    [SerializeField] int maxStamina;
    [SerializeField] float staminaDrainRate;
    [SerializeField] float staminaRegenRate;
    [SerializeField] float staminaRegenDelay;
    [SerializeField] float knockbackForce = 6f;
    [SerializeField] float knockbackDrag = 8f;

    [SerializeField] List<grenadeStats> grenadeList = new List<grenadeStats>();
    [SerializeField] List<int> grenadeCounts = new List<int>();

    [SerializeField] WeaponProceduralMovement weaponProcedural;
    [SerializeField] GameObject weaponArms;

    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] float audStepsVol;

    [Header("-----Parry-----")]
    [SerializeField] GameObject parryArms;
    [SerializeField] Animator parryAnimator;
    [SerializeField] float parryAnimTime = 0.25f;

    bool isParrying;
    bool parryIFrames;

    bool isDodging;
    bool canDodge = true;

    float currentStamina;
    float speedBoostAmount;

    int gunListPos;
    int grenadeListPos;
    int jumpMaxOrig;
    int jumpCount;
    int HPOrig;

    float shootTimer;

    bool isSprinting;
    bool isCrouching;
    bool isStandingUp;
    bool isReloading;

    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 playerCenterOrig;
    Vector3 cameraStartPos;
    Vector3 knockbackVelocity;

    bool isplayingStep;

    Coroutine staminaRegenCoroutine;
    Coroutine speedUpCoroutine;

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
    public bool getParryIFrames()
    {
        return parryIFrames;
    }

    void Start()
    {
        
        controller = GetComponent<CharacterController>();
        HPOrig = HP;
        currentStamina = maxStamina;
        jumpMaxOrig = jumpMax;
        cameraStartPos = playerCamera.localPosition;
        standHeight = controller.height;
        playerCenterOrig = controller.center;
        currentIKPoints = noWeaponPose;

        changPlayerPosition();
        updatePlayerSprintUI();

        gameManager.instance.jumpMaxTimerUI.SetActive(false);
        gameManager.instance.speedUpTimerUI.SetActive(false);

        parryArms.SetActive(false);
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            movement();
            sprint();
            crouch();
            dodge();
            crouchVisual();
            standUpLerp();
            handleSprintUI();

            if (Input.GetButtonDown("Parry") && !isParrying)
            {
                StartCoroutine(parry());
            }
        }
    }

    void movement()
    {
        if (gunList.Count > 0)
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunList[gunListPos].shootDist, Color.blue);

        shootTimer += Time.deltaTime;

        if (!isParrying && !isReloading && !isSprinting && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer > gunList[gunListPos].shootRate)
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

            if (moveDir.magnitude > 0.3f && !isplayingStep)
                StartCoroutine(playStep());
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        float currentSpeed = speed + speedBoostAmount;

        if (isSprinting)
        {
            currentSpeed *= sprintMod;
        }

        jump();

        Vector3 finalMove = moveDir.normalized * currentSpeed;
        finalMove += knockbackVelocity;
        finalMove += playerVel;

        controller.Move(finalMove * Time.deltaTime);

        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDrag * Time.deltaTime);

        playerVel.x = Mathf.Lerp(playerVel.x, 0, Time.deltaTime * 5f);
        playerVel.z = Mathf.Lerp(playerVel.z, 0, Time.deltaTime * 5f);

        playerVel.y -= gravity * Time.deltaTime;

        selectGun();
        selectGrenade();
        reload();
        
    }

    IEnumerator playStep()
    {
        isplayingStep = true;
        audPlayer.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isplayingStep = false;
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && gunList.Count > 0 && !isReloading)
        {
            gunStats gun = gunList[gunListPos];

            int ammoNeeded = gun.ammoMax - gun.ammoCur;

            if (gun.ammoReserve <= 0 || ammoNeeded <= 0)
                return;

            StartCoroutine(reloadRoutine());
        }
    }
    IEnumerator reloadRoutine()
    {
        isReloading = true;

        gunStats gun = gunList[gunListPos];

        if (weaponProcedural != null)
            weaponProcedural.StartReload();

        yield return new WaitForSeconds(1f);

        int ammoNeeded = gun.ammoMax - gun.ammoCur;
        int ammoToReload = Mathf.Min(ammoNeeded, gun.ammoReserve);

        gun.ammoCur += ammoToReload;
        gun.ammoReserve -= ammoToReload;

        updateAmmoUI();

        isReloading = false;
    }

    void sprint()
    {
        bool sprintButtonHeld = Input.GetButton("Sprint");

        if (sprintButtonHeld && currentStamina > 0)
        {
            isSprinting = true;

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
                staminaRegenCoroutine = StartCoroutine(RechargeStaminaAfterDelay(staminaRegenDelay));
            }
        }
        else
        {
            isSprinting = false;

            if (currentStamina < maxStamina && staminaRegenCoroutine == null)
            {
                staminaRegenCoroutine = StartCoroutine(RechargeStaminaAfterDelay(staminaRegenDelay));
            }
        }

        updatePlayerSprintUI();
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            audPlayer.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void shoot()
    {
        shootTimer = 0;

        gunStats gun = gunList[gunListPos];

        gun.ammoCur--;

        if (currentGunFPS != null)
        {
            Transform muzzle = currentGunFPS.transform.Find("muzzlePoint");

            if (muzzle != null && gun.muzzleFlash != null)
            {
                GameObject flash = Instantiate(gun.muzzleFlash, muzzle.position, muzzle.rotation);
                Destroy(flash, 0.5f);
            }

            if (muzzle != null && gun.tracerBullet != null)
            {
                Instantiate(gun.tracerBullet, muzzle.position, Quaternion.LookRotation(Camera.main.transform.forward));
            }
        }

        if (weaponProcedural != null)
        {
            weaponProcedural.AddRecoil();
        }

        audPlayer.PlayOneShot(
            gun.shootSound[Random.Range(0, gun.shootSound.Length)],
            gun.shootSoundVol
        );

        updateAmmoUI();

        for (int i = 0; i < gun.pellets; i++)
        {
            Vector3 direction = Camera.main.transform.forward;

            direction += Camera.main.transform.right * Random.Range(-gun.spreadAmount, gun.spreadAmount);
            direction += Camera.main.transform.up * Random.Range(-gun.spreadAmount, gun.spreadAmount);

            Debug.DrawRay(
                Camera.main.transform.position, direction * gun.shootDist, Color.red, 1f);

            RaycastHit hit;

            if (Physics.Raycast(
                Camera.main.transform.position, direction, out hit, gun.shootDist, ~ignoreLayer))
            {
                Debug.Log(hit.collider.name);

                if (gun.hiteffect != null)
                    Instantiate(gun.hiteffect, hit.point, Quaternion.identity);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(gun.shootDamage);
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

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

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

        updateGrenadeUI();
    }

    public void takeDamage(int amount)
    {
        if (parryIFrames)
        {
            return;
        }

        HP -= amount;
        updatePlayerHPUI();
        StartCoroutine(flashDamageScreen());

        audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

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
    
   

    public void changPlayerPosition()
    {
        controller.transform.position = gameManager.instance.playerStartPos.transform.position;
        Physics.SyncTransforms();
        HP = HPOrig;
        updatePlayerHPUI();
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
            return;
        }
        
            gun.ammoCur = gun.ammoMax;
            gun.ammoReserve = gun.ammoReserveMax;

            gunList.Add(gun);

        if (gunList.Count == 1)
        {
            gunListPos = 0;
            changeGun();
        }

    }

    void changeGun()
    {
        if (noWeaponPose != null)
            noWeaponPose.gameObject.SetActive(false);

        Vector3 spawnPos = Vector3.zero;

        if (weaponHolderFPS.childCount > 0)
        {
            spawnPos = weaponHolderFPS.GetChild(0).localPosition;
        }

        if (currentGunFPS != null)
            Destroy(currentGunFPS);

        currentGunFPS = Instantiate(gunList[gunListPos].gunModel, weaponHolderFPS);

        currentGunFPS.transform.localPosition = spawnPos;

        currentIKPoints = currentGunFPS.GetComponent<weaponIKPoints>();

        if (weaponProcedural != null && currentIKPoints != null)
        {
            weaponProcedural.SetADSPoint(currentIKPoints.adsPoint);
        }

        updateAmmoUI();
    }

    void LateUpdate()
    {
        if (currentIKPoints == null)
            return;

        if (rightHandTarget != null && currentIKPoints.rightHandGrip != null)
        {
            rightHandTarget.position = currentIKPoints.rightHandGrip.position;
            rightHandTarget.rotation = currentIKPoints.rightHandGrip.rotation;
        }

        if (leftHandTarget != null && currentIKPoints.leftHandGrip != null)
        {
            leftHandTarget.position = currentIKPoints.leftHandGrip.position;
            leftHandTarget.rotation = currentIKPoints.leftHandGrip.rotation;
        }
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
        if (speedUpCoroutine != null)
        {
            StopCoroutine(speedUpCoroutine);
        }

        speedUpCoroutine = StartCoroutine(speedUPRoutine(amount, duration));
    }

    private IEnumerator speedUPRoutine(float amount, float duration)
    {
        gameManager.instance.speedUpTimerUI.SetActive(true);

        speedBoostAmount = amount;

        float timer = duration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            gameManager.instance.speedUpTimer.fillAmount = timer / duration;
            yield return null;
        }

        speedBoostAmount = 0;
        gameManager.instance.speedUpTimerUI.SetActive(false);

        speedUpCoroutine = null;
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
        else if (grenade.type == grenadeStats.grenadeType.Knockback)
        {
            Vector3 direction = transform.position - explosionPoint;

            float distance = direction.magnitude;
            float forcePercent = 1 - (distance / grenade.radius);
            forcePercent = Mathf.Clamp01(forcePercent);

            direction.Normalize();

            direction.x *= grenade.horizontalForceMult;
            direction.z *= grenade.horizontalForceMult;

            direction.y = Mathf.Abs(direction.y) + grenade.upwardBonus;

            direction.Normalize();

            playerVel += direction * grenade.effectForce * forcePercent;

            controller.Move(direction * 0.2f);
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

        updateGrenadeUI();
    }

    void updateAmmoUI()
    {
        if (gunList.Count <= 0)
            return;

        gameManager.instance.updateAmmoUI(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoReserve);
    }

    void selectGrenade()
    {
        if (grenadeList.Count <= 1)
        {
            return;
        }

        if (Input.GetButtonDown("Toggle"))
        {
            grenadeListPos++;

            if (grenadeListPos >= grenadeList.Count)
            {
                grenadeListPos = 0;
            }

            updateGrenadeUI();
        }
    }

    void updateGrenadeUI()
    {
        if (grenadeList.Count <= 0)
        {
            gameManager.instance.updateGrenadeUI("No Grenade", 0);
            return;
        }

        gameManager.instance.updateGrenadeUI(grenadeList[grenadeListPos].name, grenadeCounts[grenadeListPos]);
    }

    public void parryIFramesOn()
    {
        parryIFrames = true;
    }

    public void parryIFramesOff()
    {
        parryIFrames = false;
    }

    IEnumerator parry()
    {
        isParrying = true;

        if (currentGunFPS != null)
            currentGunFPS.SetActive(false);

        if (weaponArms != null)
            weaponArms.SetActive(false);

        parryArms.SetActive(true);

        parryAnimator.ResetTrigger("Parry");
        parryAnimator.SetTrigger("Parry");

        yield return new WaitForSeconds(parryAnimTime);

        parryArms.SetActive(false);

        if (weaponArms != null)
            weaponArms.SetActive(true);

        if (currentGunFPS != null)
            currentGunFPS.SetActive(true);

        isParrying = false;
    }

    void crouch()
    {
        if (isStandingUp)
        return;
        

        if (!Input.GetButtonDown("Crouch"))
        return;
        

        bool wantToCrouch = !isCrouching; 

        if(wantToCrouch)
        {
            isCrouching = true;
            isStandingUp = false;

            controller.height = crouchHeight;
            controller.center = new Vector3(controller.center.x, crouchHeight / 2f, controller.center.z);
        }
        else
        {
            Vector3 rayStart = transform.position + Vector3.up * controller.height;
            float rayDistance = standHeight - controller.height;

            if (Physics.Raycast(rayStart, Vector3.up, rayDistance))
                return;

            isCrouching = false;
            isStandingUp = true;
        }
    }

    void crouchVisual()
    {
        Vector3 targetPos = cameraStartPos;

        if (isCrouching)
        {
            targetPos.y -= crouchCameraOffset;
        }

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetPos, Time.deltaTime);
    }

    void standUpLerp()
    {
        if (!isStandingUp)
            return;

        controller.height = Mathf.Lerp(controller.height, standHeight, standLerpSpeed * Time.deltaTime);
        controller.center = Vector3.Lerp(controller.center, playerCenterOrig, standLerpSpeed * Time.deltaTime);

        if(Mathf.Abs(controller.height - standHeight) < 0.01f)
        {
            controller.height = standHeight;
            controller.center = playerCenterOrig;
            isStandingUp = false;
        }
    }

    void dodge()
    {
        if(Input.GetButtonDown("Dodge") && canDodge && !isDodging)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 dodgeDir = new Vector3(x, 0f, z).normalized;

            if (dodgeDir == Vector3.zero)
            {
                dodgeDir = transform.forward;
            }
            else
            {
                dodgeDir = transform.TransformDirection(dodgeDir);
            }

            StartCoroutine(PlayerDodge(dodgeDir));
        }
    }

    private IEnumerator PlayerDodge(Vector3 direction)
    {
        isDodging = true;
        canDodge = false;

        float timer = 0f;
        while (timer < dodgeDuration)
        {
            controller.Move(direction * dodgeSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        
        isDodging = false;
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
    public void startKnockback(Vector3 sourcePosition)
    {
        Vector3 dir = transform.position - sourcePosition;
        dir.y = 0f;
        dir.Normalize();

        knockbackVelocity = dir * knockbackForce;
    }
    public void startKnockbackFromDirection(Vector3 hitDirection)
    {
        hitDirection.y = 0f;
        hitDirection.Normalize();

        knockbackVelocity = hitDirection * knockbackForce;
    }
}