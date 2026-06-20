using UnityEngine;

public class lavaRise : MonoBehaviour
{
    [Header("----- Lava Settings -----")]
    [SerializeField] int lavaSpeed;
    [SerializeField] float lavaTimer;
    [SerializeField] playerController player;

    Vector3 lavaDepth;

    bool lavaActive;
    float lavaTimerOrig;

    void Start()
    {
        lavaTimerOrig = lavaTimer;
        lavaActive = false;

        if (player == null)
        {
            player = gameManager.instance.player.GetComponent<playerController>();
        }

        gameManager.instance.updateLavaTimer(lavaTimer);
    }

    void Update()
    {
        if (player.getHP() <= 0)
        {
            stopLava();
        }

        if (!lavaActive)
        {
            return;
        }

        if (lavaTimer > 0)
        {
            lavaTimer -= Time.deltaTime;

            if (lavaTimer < 0)
            {
                lavaTimer = 0;
            }

            gameManager.instance.updateLavaTimer(lavaTimer);
        }
        else
        {
            lavaDepth = new Vector3(0, lavaSpeed * Time.deltaTime, 0);

            transform.localScale += lavaDepth;
            transform.position += new Vector3(0, (lavaSpeed * Time.deltaTime) / 2, 0);
        }
    }

    public void startLava()
    {
        lavaActive = true;
        gameManager.instance.showLavaTimer();
    }

    public void stopLava()
    {
        lavaActive = false;
        gameManager.instance.hideLavaTimer();
    }

    public void resetLavaTimer()
    {
        lavaTimer = lavaTimerOrig;
        gameManager.instance.updateLavaTimer(lavaTimer);
    }

    public void restartLava()
    {
        resetLavaTimer();
        startLava();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            weaponPickup weapon = other.GetComponent<weaponPickup>();

            if (weapon != null)
            {
                Destroy(weapon.gameObject);
                return;
            }

            grenadePickup grenade = other.GetComponent<grenadePickup>();

            if (grenade != null)
            {
                Destroy(grenade.gameObject);
                return;
            }

            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            gameManager.instance.SetDeathCause(gameManager.DeathCause.Lava);

            dmg.takeDamage(999);
        }
    }
}