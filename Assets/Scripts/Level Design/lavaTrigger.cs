using UnityEngine;

public class lavaTrigger : MonoBehaviour
{
    [SerializeField] lavaRise lava;
    [SerializeField] bool startLava;
    [SerializeField] bool stopLava;
    [SerializeField] bool resetTimer;

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
    }
}