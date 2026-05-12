using UnityEngine;

public class RisingLava : MonoBehaviour
{
    public float riseSpeed = 0.5f;
    public float maxHeight = 30f;

    void Update()
    {
        if (transform.position.y < maxHeight)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }
}