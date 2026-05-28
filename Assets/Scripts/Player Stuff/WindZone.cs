using System;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    public enum WindType
    {
        Backward,
        Forward
    }

    [SerializeField] WindType windType;
    [SerializeField] float windStrength;
    [SerializeField] ParticleSystem windVisuals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (windVisuals != null)
        {
            if (windVisuals != null)
            {
                windVisuals.Play();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerWindReceiver receiver = other.GetComponent<PlayerWindReceiver>();
        if (receiver == null)
        {
            return;
        }

        Vector3 dire = GetWindDirection();

        receiver.AddWind(dire, windStrength * Time.deltaTime);
    }

    Vector3 GetWindDirection()
    {
        switch (windType)
        {
            case WindType.Backward:
                return transform.forward;
            case WindType.Forward:
                return -transform.forward;

            default: 
                return Vector3.zero;
        }
    }
}
