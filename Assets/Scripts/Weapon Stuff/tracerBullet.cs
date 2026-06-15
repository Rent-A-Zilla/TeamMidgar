using UnityEngine;

public class tracerBullet : MonoBehaviour
{
    [SerializeField] float speed = 120f;
    [SerializeField] float lifeTime = 0.15f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}