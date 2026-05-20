using UnityEngine;

public class lavaScreen : MonoBehaviour
{
    [SerializeField] GameObject lavaOverlay;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.showLavaOverlay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.hideLavaOverlay();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
