using UnityEngine;

public class powerUps : MonoBehaviour
{

    enum powerUpType { healthUP, jumpPlus, speedUP }
    [SerializeField] int healAmount;
    [SerializeField] int jumpAmount;
    [SerializeField] float speedUp;
    [SerializeField] float duration;
    [SerializeField] powerUpType type;
    [SerializeField] Transform textPivot;
    [SerializeField] int textRotateSpeed;

    float angleToPlayer;

    bool playerInTrigger;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;

        rotateText();

        if (playerInTrigger)
        {
            //Power Up Code here
            if (type == powerUpType.healthUP)
            {
                gameManager.instance.playerScript.healthUP(healAmount);
            }
            if(type == powerUpType.jumpPlus)
            {
                //jumpUP();

            }
            else
            {
                //speedUP();
            }

            Destroy(gameObject);
        }

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
        }
    }

    void rotateText()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        textPivot.rotation = Quaternion.Lerp(textPivot.rotation, rot, Time.deltaTime * textRotateSpeed);
    }
}
