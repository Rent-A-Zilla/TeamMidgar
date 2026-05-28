using System.Collections;
using UnityEngine;

public class disappearingPlatform : MonoBehaviour
{
    [SerializeField] float disappearDelay;
    [SerializeField] float reappearDelay;

    MeshRenderer meshRenderer;
    Collider platformCollider;
    bool isTriggered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTriggered)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Disappear());
        }
    }

    IEnumerator Disappear()
    {
        isTriggered = true;

        yield return new WaitForSeconds(disappearDelay);

        meshRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(reappearDelay);

        meshRenderer.enabled = true;
        platformCollider.enabled = true;

        isTriggered = false;
    }
}
