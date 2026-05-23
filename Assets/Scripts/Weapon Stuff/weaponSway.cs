using UnityEngine;

public class weaponSway : MonoBehaviour
{
    [SerializeField] float swayAmount = 0.02f;
    [SerializeField] float maxSwayAmount = 0.06f;
    [SerializeField] float smoothAmount = 6f;

    Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        Vector3 finalPosition = new Vector3(
            initialPosition.x - mouseX,
            initialPosition.y - mouseY,
            initialPosition.z
        );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            finalPosition,
            Time.deltaTime * smoothAmount
        );
    }
}