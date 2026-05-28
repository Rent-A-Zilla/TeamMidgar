using UnityEngine;

public class lavaTrigger : MonoBehaviour
{
    [SerializeField] lavaRise lava;
    [SerializeField] bool startLava;
    [SerializeField] bool stopLava;
    [SerializeField] bool resetTimer;
    [SerializeField] bool useOnce = true;

    Collider triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (resetTimer)
        {
            lava.resetLavaTimer();
        }

        if (startLava)
        {
            lava.startLava();
        }

        if (stopLava)
        {
            lava.stopLava();
        }

        if (useOnce && triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }
}