using UnityEngine;

public class lavaRise : MonoBehaviour
{
    [SerializeField] int lavaSpeed;
    [SerializeField] float lavaTimer;
    [SerializeField] playerController player;

    Vector3 lavaDepth;

  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       if(player.getHP() <= 0)
        {
            enabled = false;
        }

        if (lavaTimer > 0)
        {
            lavaTimer -= Time.deltaTime;
        }
        else
        {
            lavaDepth = new Vector3(0, lavaSpeed * Time.deltaTime, 0);

            transform.localScale += lavaDepth;
            transform.position += new Vector3(0, (lavaSpeed * Time.deltaTime) / 2, 0);
        }


    }

}
