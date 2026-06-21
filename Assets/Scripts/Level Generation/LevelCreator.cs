
using System.Collections.Generic;
using System.Collections; 
using UnityEngine;
using System;

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
    public GameObject wallVertical, wallHorizontal;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalposition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;

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

        GameObject wallParent = new GameObject("Wall Parent");
        wallParent.transform.parent = transform; 
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalposition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            createMesh(listOfRooms[i].BottemLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
          
        }
        CreateWalls(wallParent);

    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach(var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach(var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
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

        for(int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPostion(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalposition);
        }
        for(int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPostion(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalposition);
        }
        for(int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPostion(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPostion(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPostion(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if(wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }
}
