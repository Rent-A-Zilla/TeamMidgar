using Unity.VisualScripting;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [Range(-60, 0)][SerializeField] int lockVertMin;
    [Range(0, 90)][SerializeField] int lockVertMax;
    [SerializeField] Transform player;

    float camRotx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

            camRotx -= mouseY;

            camRotx = Mathf.Clamp(camRotx, lockVertMin, lockVertMax);
            transform.localRotation = Quaternion.Euler(camRotx, 0, 0);

            player.transform.Rotate(Vector3.up * mouseX);
        }
    }
}
