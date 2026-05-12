using UnityEngine;

public class risingLava : MonoBehaviour
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
