using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class kamikazeEnemyAI : MonoBehaviour, IDamage, IGrenade
{
    [Header("----- Components -----")]
    [SerializeField] Renderer rend;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject healthBarCanvas;
    [SerializeField] Image enemyHPBarFill;

    [Header("----- Stats -----")]
    [Range(1, 1000)][SerializeField] int HP;
    [Range(1, 50)][SerializeField] int faceTargetSpeed = 10;
    [Range(5, 180)][SerializeField] int FOV;

    [Header("----- Roam Stats -----")]
    [Range(5, 500)][SerializeField] int roamDist;
    [Range(0, 10)][SerializeField] int roamPauseTimer;


    [Header("----- Explosion Settings -----")]
    public GameObject explosionEffect;
    [SerializeField] int damage;
    [SerializeField] float tikInterval;
    [SerializeField] float duration;
    [SerializeField] float explosiveRadius;
    [SerializeField] float explodeTriggerDist;
    [SerializeField] float explodeDelay;
    [SerializeField] float chargeSpeed;



    [Header("----- Loot -----")]
    [SerializeField] GameObject[] objectsToDrop;
    [SerializeField] int amountToDrop;
    [SerializeField] int dropRate;
    [SerializeField] int dropDist;
    [SerializeField] int cashPerHit;
    [SerializeField] int cashOnDeath;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSearch;
    [SerializeField] float audSearchVol;
    [SerializeField] AudioClip[] audExplosion;
    [SerializeField] float audExplosionVol;
    [SerializeField] AudioClip[] audTik;
    [SerializeField] float audTikVol;

    [Header("----- Animator -----")]
    [SerializeField] Animator anim;

    Color colorOrig;

    int HPOrig;

    float shootTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    float roamTimer;

    private float lastHurtSoundTime = 0f;
    private float hurtSoundCooldown = .2f;
    bool playerInTrigger;
    bool wasChasing;
    bool isSearching;
    bool isDead = false;
    bool hasExploded;
    bool isCharging;
    bool isExploding;

    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 lastKnownPlayerPos;
    Vector3 deathLocation;

    Coroutine knockbackCoroutine;
    Vector3 knockbackVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;

        healthBarCanvas = transform.Find("EnemyHpBarCanvas").gameObject;
        enemyHPBarFill = transform.Find("EnemyHpBarCanvas/EnemyHpBar/HpBarFill").GetComponent<UnityEngine.UI.Image>();
        healthBarCanvas.SetActive(false);

        colorOrig = rend.material.color;
        //gameManager.instance.updateGameGoal(1);

        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0) return;
        if (isCharging || isExploding) return;

        float distToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        if(!isCharging && distToPlayer <= explodeTriggerDist)
        {
            StartCoroutine(kamikazeCharge());
            return;
        }

        if (HP > 0)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                anim.SetFloat("Speed", agent.velocity.magnitude);
            }
            bool canSee = canSeePlayer();
            if (playerInTrigger && canSee)
            {
                wasChasing = true;

                StopCoroutine("searchForPlayer");
                isSearching = false;
            }
            else if (playerInTrigger && !canSee)
            {
                if(wasChasing && !isSearching)
                {
                    StartCoroutine(searchForPlayer());
                }
                else if (!isSearching)
                {
                    checkRoam();
                }
            }
            else if (!playerInTrigger && !isSearching)
            {
                checkRoam();
            }
        }
        if (healthBarCanvas != null && healthBarCanvas.activeSelf)
        {
            healthBarCanvas.transform.LookAt(Camera.main.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0);
        }
    }

    IEnumerator searchForPlayer()
    {
        isSearching = true;
        wasChasing = false;


        audPlayer.PlayOneShot(audSearch[Random.Range(0, audSearch.Length)], audSearchVol);

        int originalRoamDist = roamDist;
        int originalRoamTimer = roamPauseTimer;

        roamDist = 5;
        roamPauseTimer = 0;

        float searchDuration = 5f;
        float timer = 0;

        while(timer < searchDuration)
        {
            timer += Time.deltaTime;
            checkRoam();
            yield return null;
        }
        roamDist = originalRoamDist;
        roamPauseTimer = originalRoamTimer;
        isSearching = false;

    }
    void checkRoam()
    {
        if (agent.enabled && agent.isOnNavMesh && agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;

            if (roamTimer >= roamPauseTimer)
            {
                roam();
            }
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 centerPoint = isSearching ? lastKnownPlayerPos : startingPos;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += centerPoint;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
        {
            return false;
        }

        playerDir = gameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                gameManager.instance.EnemyEnteredCombat();
                lastKnownPlayerPos = gameManager.instance.player.transform.position;

                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }

                rotateToTarget();

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        gameManager.instance.EnemyExitedCombat();
        agent.stoppingDistance = 0;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            agent.stoppingDistance = 0;
        }
    }

    public void takeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (!audPlayer.isPlaying && Time.time >= lastHurtSoundTime + hurtSoundCooldown)
        {
            audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
            lastHurtSoundTime = Time.time;
        }

        anim.SetTrigger("Hit");

        HP -= amount;

        updateHealthBar();

        gameManager.instance.addCurrency(cashPerHit);

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }


        if (HP <= 0 && !isDead)
        {
            isDead = true;
            gameManager.instance.addCurrency(cashOnDeath);
            gameManager.instance.updateGameGoal(-1);

            deathLocation = transform.position;
            StartCoroutine(deathDrops(deathLocation));
           
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = colorOrig;
    }

    void rotateToTarget()
    {

        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void updateHealthBar()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(true);

        if (enemyHPBarFill != null)
            enemyHPBarFill.fillAmount = (float)HP / HPOrig;
    }

    public void applyGrenadeEffects(grenadeStats grenade, Vector3 explosionPoint)
    {
        // Explosive grenade damages enemy
        if (grenade.type == grenadeStats.grenadeType.Explosive)
        {
            takeDamage(grenade.damage);
        }

        // Anti-gravity grenade launches enemy upward
        else if (grenade.type == grenadeStats.grenadeType.AntiGravity)
        {
            if (knockbackCoroutine != null)
                StopCoroutine(knockbackCoroutine);

            knockbackCoroutine = StartCoroutine(
                knockbackEnemy(Vector3.up * grenade.effectForce)
            );
        }

        // Knockback grenade pushes enemy away from explosion
        else if (grenade.type == grenadeStats.grenadeType.Knockback)
        {
            // Get direction away from explosion
            Vector3 direction = transform.position - explosionPoint;

            // Scale force based on distance from explosion
            float distance = direction.magnitude;
            float forcePercent = 1 - (distance / grenade.radius);
            forcePercent = Mathf.Clamp01(forcePercent);

            // Normalize before modifying values
            direction.Normalize();

            // Increase horizontal propulsion
            direction.x *= grenade.horizontalForceMult;
            direction.z *= grenade.horizontalForceMult;

            // Add upward boost
            direction.y = Mathf.Abs(direction.y) + grenade.upwardBonus;

            // Normalize again after edits
            direction.Normalize();

            if (knockbackCoroutine != null)
                StopCoroutine(knockbackCoroutine);

            // Apply knockback force
            knockbackCoroutine = StartCoroutine(
                knockbackEnemy(direction * grenade.effectForce * forcePercent)
            );
        }
    }
    IEnumerator knockbackEnemy(Vector3 force)
    {
        // Disable NavMesh while enemy is being launched
        agent.enabled = false;

        knockbackVelocity = force;

        float timer = 1.5f;

        while (timer > 0)
        {
            // Calculate movement for this frame
            Vector3 move = knockbackVelocity * Time.deltaTime;

            // Check for walls before moving
            if (Physics.SphereCast(transform.position,0.5f,move.normalized,out RaycastHit wallHit, move.magnitude))
            {
                // Stop horizontal movement if hitting wall
                if (!wallHit.collider.isTrigger)
                {
                    knockbackVelocity.x = 0;
                    knockbackVelocity.z = 0;
                }
            }
            else
            {
                // Move enemy if no wall detected
                transform.position += move;
            }

            // Smoothly reduce horizontal knockback
            knockbackVelocity.x = Mathf.Lerp(knockbackVelocity.x, 0, Time.deltaTime * 5f);
            knockbackVelocity.z = Mathf.Lerp(knockbackVelocity.z, 0, Time.deltaTime * 5f);

            // Apply gravity
            knockbackVelocity.y -= 30f * Time.deltaTime;

            // Check for ground below enemy
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, 1.1f))
            {
                // Stop falling once enemy reaches ground
                if (knockbackVelocity.y <= 0)
                {
                    transform.position = groundHit.point;
                    break;
                }
            }

            timer -= Time.deltaTime;

            yield return null;
        }

        // Reposition enemy back onto NavMesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit navHit, 3f, NavMesh.AllAreas))
        {
            transform.position = navHit.position;

            agent.enabled = true;

            agent.Warp(navHit.position);
        }

        knockbackCoroutine = null;
    }
    IEnumerator deathDrops(Vector3 deathLocation)
    {
        int dropped = 0;

        while (dropped < amountToDrop)
        {
            Vector3 ranPos = Random.insideUnitSphere * dropDist;
            ranPos += deathLocation;

            NavMeshHit hit;
            bool found = NavMesh.SamplePosition(ranPos,out hit, dropDist, NavMesh.AllAreas);
            if (!found) 
            {
                continue;
            }
            NavMesh.SamplePosition(ranPos, out hit, dropDist, 1);
            Vector3 dropPos = hit.position + Vector3.up * 0.35f;
            Instantiate(objectsToDrop[Random.Range(0, objectsToDrop.Length)], dropPos, 
                Quaternion.Euler(0, Random.Range(0, 360), 0));

            dropped++;
            yield return new WaitForSeconds(dropRate);
        }

    }
    IEnumerator kamikazeWarning(float duration, float tikInterval)
    {
        float timer = 0f;
        while (timer < duration)
        {

            rend.material.color = Color.red;

            if(agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
            }

            audPlayer.PlayOneShot(audTik[Random.Range(0, audTik.Length)], audTikVol);

            yield return new WaitForSeconds(tikInterval * 0.5f);
            rend.material.color = colorOrig;
            yield return new WaitForSeconds(tikInterval * 0.5f);
            timer += tikInterval;
        }
        rend.material.color = colorOrig;

    }

    IEnumerator kamikazeCharge()
    {
        isCharging = true;
        isExploding = true;

        agent.speed = chargeSpeed;
        agent.stoppingDistance = 0;

        yield return StartCoroutine(kamikazeWarning(explodeDelay, tikInterval));

        explode();
    }

    void explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        isCharging = false;
        isExploding = false;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosiveRadius);

        if (audExplosion.Length > 0)
        {
            AudioSource.PlayClipAtPoint(audExplosion[Random.Range(0, audExplosion.Length)], transform.position, audExplosionVol);
        }

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<IDamage>()?.takeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

}
