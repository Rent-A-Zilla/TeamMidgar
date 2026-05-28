using UnityEngine;
using System.Collections;

public class checkPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerStartPos.transform.position != transform.position)
        {
            gameManager.instance.playerStartPos.transform.position = transform.position;
            StartCoroutine(displayCheckPointUI());
        }
    }

    IEnumerator displayCheckPointUI()
    {
        gameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        gameManager.instance.checkpointPopup.SetActive(false);
    }
}
