using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public int levelWidth;
    public int levelLength;
    public int widthMin;
    public int lengthMin;
    public int maxIterations;
    public int corridorwidth; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createLevel()
    {
        levelGenerator generator = new levelGenerator(levelWidth, levelLength); 
        var listOfRooms = generator.CalculateRooms(maxIterations, widthMin, lengthMin);
    }
}
