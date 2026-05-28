using System;
using UnityEngine;

public class PlayerWindReceiver : MonoBehaviour
{
    [SerializeField] CharacterController playerControler;

    Vector3 windVel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerControler == null)
        {
            playerControler = GetComponent<CharacterController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (windVel.magnitude > 0.01f)
        {
            playerControler.Move(windVel * Time.deltaTime);
        }

        windVel = Vector3.Lerp(windVel, Vector3.zero, Time.deltaTime * 3f);
    }

    public void AddWind(Vector3 vel, float strength)
    {
        windVel += vel.normalized * strength;
    }
}
