using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class meleeEnemyAI : MonoBehaviour, IDamage, IGrenade
{
    [Header("----- Components -----")]
    [SerializeField] Renderer rend;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject healthBarCanvas;
    [SerializeField] Image enemyHPBarFill;

    [Header("----- Stats -----")]
    [Range(1, 1000)][SerializeField] int HP =30;
    [Range(1, 50)][SerializeField] int faceTargetSpeed =10;
    [Range(5, 180)][SerializeField] int FOV =45;

    [Header("----- Roam Stats -----")]
    [Range(5, 500)][SerializeField] int roamDist =5;
    [Range(0, 10)][SerializeField] int roamPauseTimer =0;

    [Header("----- Weapons -----")]
    [SerializeField] Collider weaponCollider;
    [SerializeField] float attackRate;
    [SerializeField] float attackRange = 2f;

    [Header("----- Loot -----")]
    [SerializeField] GameObject[] objectsToDrop;
    [SerializeField] int amountToDrop;
    [SerializeField] int dropRate;
    [SerializeField] int dropDist =1;
    [SerializeField] int cashPerHit =5;
    [SerializeField] int cashOnDeath =25;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSearch;
    [SerializeField] float audSearchVol;

    [Header("----- Animator -----")]
    [SerializeField] Animator anim;

    Color colorOrig;

    int HPOrig;

    float attackTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    float roamTimer;

    private float lastHurtSoundTime = 0f;
    private float hurtSoundCooldown = .2f;
    bool playerInTrigger;
    bool wasChasing;
    bool isSearching;
    bool isAttacking;
    bool inAttackRange;
    bool isDead = false;

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
        weaponCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        inAttackRange = distanceToPlayer <= attackRange;
        if (HP > 0)
        {
            if(agent.enabled && agent.isOnNavMesh)
            {
                if (!isAttacking)
                {
                    anim.SetFloat("Speed", agent.velocity.magnitude);
                }
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
            HandleAttack();
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

                float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
                if(distanceToPlayer <= attackRange)
                {
                    inAttackRange = distanceToPlayer <= attackRange;
                }

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

            StartCoroutine(DeathRoutine());
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

    IEnumerator DeathRoutine()
    {
        anim.SetBool("Dead", true);
        agent.enabled = false;
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    public void EnableHitbox()
    {
        weaponCollider.enabled = false;
    }

    public void DisableHitbox()
    {
        weaponCollider.enabled = true;
    }

    void HandleAttack()
    {
        if (isDead) return;
        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        bool canAttack = distanceToPlayer <= attackRange;
        if(canAttack)
        {
            attackTimer += Time.deltaTime;

            if(attackTimer >= attackRate)
            {
                attackTimer = 0;
                anim.SetTrigger("Attack");
            }
            else
            {
                attackTimer = 0f;
            }
        }
    }

    public void OnAttackStart()
    {
        isAttacking = true;
    }
    public void OnAttackEnd()
    {
        isAttacking = false;
    }
}
