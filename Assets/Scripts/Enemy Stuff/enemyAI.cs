using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer rend;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject healthBarCanvas;
    [SerializeField] Image enemyHPBarFill;

    [Header("----- Stats -----")]
    [Range(1, 100)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int faceTargetSpeed;
    [Range(5, 180)][SerializeField] int FOV;

    [Header("----- Roam Stats -----")]
    [Range(5, 500)][SerializeField] int roamDist;
    [Range(0, 10)][SerializeField] int roamPauseTimer;

    [Header("----- Weapons -----")]
    [SerializeField] GameObject bullet;
    [Range(0.1f, 2)][SerializeField] float shootRate;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform shootPos;
    [Range(1, 25)][SerializeField] int gunRotateSpeed;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;

    Color colorOrig;

    int HPOrig;

    float shootTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    float roamTimer;

    bool playerInTrigger;

    Vector3 playerDir;
    Vector3 startingPos;

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
        if (HP > 0)
        {
            if (playerInTrigger && !canSeePlayer())
            {
                checkRoam();
            }
            else if (!playerInTrigger)
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

    void checkRoam()
    {
        if (agent.remainingDistance < 0.01f)
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

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                rotateGun();
                rotateToTarget();

                shootTimer += Time.deltaTime;

                if (shootTimer > shootRate)
                {
                    shoot();
                }
                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
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
        if (HP <= 0)
        {
            return;
        }

        audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        HP -= amount;

        updateHealthBar();

        gameManager.instance.addCurrency(10);
        agent.SetDestination(gameManager.instance.player.transform.position);


        if (HP <= 0)
        {
            gameManager.instance.addCurrency(100);
            gameManager.instance.updateGameGoal(-1);
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

    void rotateGun()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, rot, Time.deltaTime * gunRotateSpeed);
    }

    void rotateToTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, shootPos.position, gunPivot.rotation);
    }

    void updateHealthBar()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(true);

        if (enemyHPBarFill != null)
            enemyHPBarFill.fillAmount = (float)HP / HPOrig;
    }
}
