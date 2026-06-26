
using System.Collections.Generic;
using System.Collections; 
using UnityEngine;
using System;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] GameObject[] enemySpawner;
    [SerializeField] GameObject[] weapons;
    [SerializeField] GameObject flag; 

    [SerializeField] int enemyCount = 5;
    [SerializeField] int weaponCount = 5;

    [SerializeField] NavMeshSurface NavMeshSurface;
    public int levelWidth;
    public int levelLength;
    public int widthMin;
    public int lengthMin;
    public int maxIterations;
    public int corridorwidth = 3;
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

        StartCoroutine(SpawnAfterGeneration(generator.GeneratedRooms));
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
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
        for (int i = 0; i < uvs.Length; i++)
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
        levelFloor.transform.parent = transform;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPostion(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalposition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPostion(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalposition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
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
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }

    private IEnumerator SpawnAfterGeneration(List<RoomNode> rooms)
    {
        NavMeshSurface.BuildNavMesh();

        yield return null;

        MovePlayerToSpawn(rooms);

        SpawnEnemies(rooms);

        SpawnWeapons(rooms);

        SpawnFlag(rooms);
    }

    private void MovePlayerToSpawn(List<RoomNode> rooms)
    {
        if (player == null || rooms.Count == 0)
        {
            return;
        }

        RoomNode startRoom = rooms.OrderByDescending(r => r.Width * r.Length).First();

        player.position = GetRoomCenter(startRoom);
    }

    private Vector3 GetRoomCenter(RoomNode room)
    {
        float x = (room.BottemLeftAreaCorner.x + room.TopRightAreaCorner.x) / 2f;

        float z = (room.BottemLeftAreaCorner.y + room.TopRightAreaCorner.y) / 2f;

        return new Vector3(x, 1f, z);
    }

    private Vector3 GetRandomPointInRoom(RoomNode room)
    {
        const float margin = 2f;

        float x = Random.Range(room.BottemLeftAreaCorner.x + margin,
                               room.TopRightAreaCorner.x - margin);

        float z = Random.Range(room.BottemLeftAreaCorner.y + margin,
                               room.TopRightAreaCorner.y - margin);

        return new Vector3(x, 1f, z);
    }

    private void SpawnEnemies(List<RoomNode> rooms)
    {
        if (enemySpawner == null || enemySpawner.Length == 0)
            return;

        RoomNode playerRoom = rooms.OrderByDescending(r => r.Width * r.Length).First();

        List<RoomNode> enemyRooms = rooms.Where(r => r != playerRoom).ToList();

        foreach (RoomNode room in enemyRooms)
        {
            Vector3 spawnPos = GetRoomCenter(room);

            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                GameObject spawner = enemySpawner[Random.Range(0, enemySpawner.Length)];

                Instantiate(spawner, hit.position, Quaternion.identity);
            }
        }
    }



    private void SpawnWeapons(List<RoomNode> rooms)
    {
        if (weapons == null || weapons.Length == 0)
            return;

        List<RoomNode> validRooms = rooms.Where(r => r.Width >= 6 && r.Length >= 6).ToList();

        for (int i = 0; i < weaponCount; i++)
        {
            RoomNode room = validRooms[Random.Range(0, rooms.Count)];

            Vector3 pos = GetRandomPointInRoom(room);

            GameObject weapon = weapons[Random.Range(0, weapons.Length)];

            Instantiate(weapon, pos, Quaternion.identity);
        }
        
    }

    private void SpawnFlag(List<RoomNode> rooms)
    {
        if(flag == null || rooms.Count < 2)
        {
            return;
        }

        RoomNode playerRoom = rooms.OrderByDescending(r => r.Width * r.Length).First();

        RoomNode furthestRoom = rooms.Where(r => r != playerRoom).OrderByDescending(r => Vector2.Distance(playerRoom.BottemLeftAreaCorner, r.BottemLeftAreaCorner)).First();

        Vector3 flagPos = GetRoomCenter(furthestRoom); 

        Instantiate(flag, flagPos, Quaternion.identity);
    }

    private bool TryGetSpawnPoint(RoomNode room, out Vector3 spawnPos)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = GetRandomPointInRoom(room);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                    spawnPos = hit.position;
                    return true;
            }
        }

        spawnPos = Vector3.zero;
        return false;
    }
}
