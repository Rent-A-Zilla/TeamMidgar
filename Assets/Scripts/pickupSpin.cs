using UnityEngine;

public class pickupSpin : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = new Vector3 (0, 0, 0);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed *  Time.deltaTime);
    }
}
