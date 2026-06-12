using UnityEngine;

public class fallingPlatformTrigger : MonoBehaviour
{
    private fallingPlatform platform;

    void Start()
    {
        platform = GetComponentInParent<fallingPlatform>();

        if (platform == null)
        {
            Debug.LogError("No FallingPlatform script found on parent object!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered falling platform trigger.");

            if (platform != null)
            {
                platform.StartFalling();
            }
        }
    }
}