using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class RoomMapGenerator : AbstractMapGenerator
{
    private DungeonData _dungeonData;
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int mapWidth = 20, mapHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private int roomSpacing = 1;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.CreateVariableSizeRooms(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(mapWidth, mapHeight, 0)), 
            minRoomWidth, minRoomHeight, roomSpacing); // Передаем зазор в метод

        HashSet<Vector2Int> floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        _dungeonData.Path.UnionWith(corridors);
        floor.UnionWith(corridors);

        var expandedFloor = ExpandFloor(floor);
        var specialWallPositions = new HashSet<Vector2Int>(expandedFloor);
        specialWallPositions.ExceptWith(floor);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(expandedFloor, specialWallPositions, tilemapVisualizer);
        tilemapVisualizer.AddWallColliders();
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        List<Edge> edges = CreateAllEdges(roomCenters);
        edges.Sort((edge1, edge2) => edge1.Weight.CompareTo(edge2.Weight));

        Dictionary<Vector2Int, Vector2Int> parent = InitializeParentDictionary(roomCenters);

        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        foreach (var edge in edges)
        {
            if (Find(parent, edge.RoomA) != Find(parent, edge.RoomB))
            {
                Union(parent, edge.RoomA, edge.RoomB);
                corridors.UnionWith(CreateCorridor(edge.RoomA, edge.RoomB));
            }
        }

        return corridors;
    }

    private List<Edge> CreateAllEdges(List<Vector2Int> roomCenters)
    {
        List<Edge> edges = new List<Edge>();

        for (int i = 0; i < roomCenters.Count; i++)
        {
            for (int j = i + 1; j < roomCenters.Count; j++)
            {
                edges.Add(new Edge(roomCenters[i], roomCenters[j]));
            }
        }

        return edges;
    }

    private Dictionary<Vector2Int, Vector2Int> InitializeParentDictionary(List<Vector2Int> roomCenters)
    {
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        foreach (var center in roomCenters)
        {
            parent[center] = center;
        }
        return parent;
    }

    private Vector2Int Find(Dictionary<Vector2Int, Vector2Int> parent, Vector2Int room)
    {
        if (parent[room] != room)
        {
            parent[room] = Find(parent, parent[room]);
        }
        return parent[room];
    }

    private void Union(Dictionary<Vector2Int, Vector2Int> parent, Vector2Int roomA, Vector2Int roomB)
    {
        Vector2Int rootA = Find(parent, roomA);
        Vector2Int rootB = Find(parent, roomB);
        if (rootA != rootB)
        {
            parent[rootB] = rootA;
        }
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        
        Vector2Int currentPosition = start;
        corridor.Add(currentPosition);
        
        while (currentPosition.x != end.x)
        {
            if (currentPosition.x < end.x)
            {
                currentPosition += Vector2Int.right;
            }
            else if (currentPosition.x > end.x)
            {
                currentPosition += Vector2Int.left;
            }
            corridor.Add(currentPosition);
        }
        
        while (currentPosition.y != end.y)
        {
            if (currentPosition.y < end.y)
            {
                currentPosition += Vector2Int.up;
            }
            else if (currentPosition.y > end.y)
            {
                currentPosition += Vector2Int.down;
            }
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    private float Distance(Vector2Int a, Vector2Int b)
    {
        return Vector2Int.Distance(a, b);
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float minDistance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    roomFloor.Add(position);
                }
            }
            floor.UnionWith(roomFloor);
            _dungeonData.Rooms.Add(new Room((Vector2Int)Vector3Int.RoundToInt(room.center), roomFloor));
        }

        return floor;
    }

    private HashSet<Vector2Int> ExpandFloor(HashSet<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>(floorPositions);
        foreach (var position in floorPositions)
        {
            var upPosition = Direction2D.CardinalDirectionsList[0];
            var neighbourPosition = position + upPosition;
            if (!floorPositions.Contains(neighbourPosition))
                wallPositions.Add(neighbourPosition);
        }

        return wallPositions;
    }
}


