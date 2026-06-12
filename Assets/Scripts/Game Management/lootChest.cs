using UnityEngine;

public class lootChest : MonoBehaviour
{
    [Header("----- Chest Parts -----")]
    [SerializeField] Transform lid;
    [SerializeField] float openAngle = -90f;
    [SerializeField] float openSpeed = 5f;

    [Header("----- Loot -----")]
    [SerializeField] GameObject[] lootPrefabs;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float throwForce = 6f;
    [SerializeField] float upwardForce = 4f;

    [Header("----- Interaction -----")]
    [SerializeField] GameObject interactText;

    bool playerInRange;
    bool isOpen;
    bool hasDroppedLoot;

    Quaternion lidClosedRot;
    Quaternion lidOpenRot;

    void Start()
    {
        lidClosedRot = lid.localRotation;
        lidOpenRot = Quaternion.Euler(openAngle, 0, 0) * lidClosedRot;

        if (interactText != null)
        {
            interactText.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !isOpen && Input.GetButtonDown("Interact"))
        {
            openChest();
        }

        if (isOpen)
        {
            lid.localRotation = Quaternion.Lerp(lid.localRotation, lidOpenRot, Time.deltaTime * openSpeed);
        }
    }

    void openChest()
    {
        isOpen = true;

        if (interactText != null)
        {
            interactText.SetActive(false);
        }

        if (!hasDroppedLoot)
        {
            dropRandomLoot();
            hasDroppedLoot = true;
        }
    }

    void dropRandomLoot()
    {
        if (lootPrefabs.Length <= 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, lootPrefabs.Length);

        GameObject loot = Instantiate(
            lootPrefabs[randomIndex],
            spawnPoint.position,
            spawnPoint.rotation
        );

        Rigidbody rb = loot.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 throwDir = transform.forward + Vector3.up;
            rb.AddForce(throwDir.normalized * throwForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerInRange = true;

            if (interactText != null)
            {
                interactText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactText != null)
            {
                interactText.SetActive(false);
            }
        }
    }
}