using UnityEngine;

public class lavaRise : MonoBehaviour
{
    [SerializeField] int lavaSpeed;
    [SerializeField] float lavaTimer;
    [SerializeField] playerController player;

    Vector3 lavaDepth;

    bool lavaExpand;
  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       if(player.getHP() <= 0)
        {
            lavaExpand = false;
        }

       if(lavaExpand)
        {
            return;
        }

        if (lavaTimer > 0)
        {
            lavaTimer -= Time.deltaTime;

            gameManager.instance.updateLavaTimer(lavaTimer);
        }
        else
        {
            lavaDepth = new Vector3(0, lavaSpeed * Time.deltaTime, 0);

            transform.localScale += lavaDepth;
            transform.position += new Vector3(0, (lavaSpeed * Time.deltaTime) / 2, 0);
        }


    }

}
