using UnityEngine;

public class powerUps : MonoBehaviour
{

    [SerializeField] int faceTargetSpeed;
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
            rotateToTarget();

        if (playerInTrigger)
        {
            //Power Up Code here
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

    void rotateToTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
