using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public int levelWidth;
    public int levelLength;
    public int widthMin;
    public int lengthMin;
    public int maxIterations;
    public int corridorwidth;
    public Material material;
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifer;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;
    [Range(0, 2)]
    public int roomOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createLevel();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createLevel()
    {
        levelGenerator generator = new levelGenerator(levelWidth, levelLength);
        var listOfRooms = generator.CalculateLevel(maxIterations, widthMin, lengthMin, roomBottomCornerModifer, roomTopCornerModifier, roomOffset, corridorwidth);
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            createMesh(listOfRooms[i].BottemLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
            Debug.Log("Room: " + listOfRooms[i].BottemLeftAreaCorner + " -> " + listOfRooms[i].TopRightAreaCorner);
            Debug.Log("Rooms Returned: " + listOfRooms.Count);
        }

    }

    private void createMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV,
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3,
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject levelFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));   

        levelFloor.transform.position = Vector3.zero;
        levelFloor.transform.localScale = Vector3.one;
        levelFloor.GetComponent<MeshFilter>().mesh = mesh;
        levelFloor.GetComponent<MeshRenderer>().material = material;
    }
}
