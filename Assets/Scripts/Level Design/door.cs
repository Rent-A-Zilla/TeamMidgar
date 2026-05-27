using UnityEngine;

public class door : MonoBehaviour
{
    [Header("----- Door Parts -----")]
    [SerializeField] Transform doorPivot;      
    [SerializeField] Transform doorHandle;      
    [SerializeField] GameObject interactText;   

    [Header("----- Door Settings -----")]
    [SerializeField] float openAngle = 90f;     
    [SerializeField] float openSpeed = 2f;      

    [Header("----- Handle Settings -----")]
    [SerializeField] float handleAngle = -25f;  
    [SerializeField] float handleSpeed = 6f;    

    bool playerInTrigger; 
    bool doorOpen;        

    Quaternion closedRotation; 
    Quaternion openRotation;   

    Quaternion handleStartRot;     
    Quaternion handleInteractRot;  

    void Start()
    {
        
        closedRotation = doorPivot.localRotation;

        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

       
        handleStartRot = doorHandle.localRotation;

     
        handleInteractRot = handleStartRot * Quaternion.Euler(handleAngle, 0, 0);

   
        interactText.SetActive(false);
    }

    void Update()
    {
        // Open/close the door when player presses interact
        if (playerInTrigger)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (doorOpen)
                {
                    doorOpen = false;
                }
                else
                {
                    doorOpen = true;
                }
            }
        }

        // Rotate the door
        if (doorOpen)
        {
            doorPivot.localRotation = Quaternion.Lerp(doorPivot.localRotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            doorPivot.localRotation = Quaternion.Lerp(doorPivot.localRotation, closedRotation, Time.deltaTime * openSpeed);
        }

        // Rotate the handle upward when player is close
        if (playerInTrigger)
        {
            doorHandle.localRotation = Quaternion.Lerp(doorHandle.localRotation, handleInteractRot,  Time.deltaTime * handleSpeed);
        }
        else
        {
            doorHandle.localRotation = Quaternion.Lerp(doorHandle.localRotation, handleStartRot, Time.deltaTime * handleSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;

            // Show the "E to Open" text
            interactText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the trigger is the player
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;

            // Hide the "E to Open" text
            interactText.SetActive(false);
        }
    }
}