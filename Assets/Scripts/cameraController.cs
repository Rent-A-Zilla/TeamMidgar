using Unity.VisualScripting;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;

    [Header("Third Person Look Limits")]
    [Range(-60, 0)][SerializeField] int tpsLockVertMin;
    [Range(0, 90)][SerializeField] int tpsLockVertMax;

    [Header("First Person Look Limits")]
    [SerializeField] int fpsLockVertMin;
    [SerializeField] int fpsLockVertMax;

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

            if (gameManager.instance.isFirstPerson)
            {
                camRotx = Mathf.Clamp(camRotx, fpsLockVertMin, fpsLockVertMax);
            }
            else
            {
                camRotx = Mathf.Clamp(camRotx, tpsLockVertMin, tpsLockVertMax);
            }

            transform.localRotation = Quaternion.Euler(camRotx, 0, 0);

            player.transform.Rotate(Vector3.up * mouseX);
        }
    }
}
