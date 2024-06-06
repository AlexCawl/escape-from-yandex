using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable All

public class RoomMapGenerator : AbstractMapGenerator
{
    private MapData _mapData;
    [SerializeField] private RoomDataExtractor roomDataExtractor;
    [SerializeField] private EnemyAndPropPlacementManager enemyAndPropPlacementManager;
    [SerializeField] private int minRoomSize = 4;
    [SerializeField] private int maxRoomSize = 4;
    [SerializeField] private int mapWidth = 20, mapHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private int roomSpacing = 1;
    [SerializeField] private bool useBSPAlgorithm = false;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CreateRooms()
    {
        
        _mapData = new MapData();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        if (useBSPAlgorithm)
        {
            roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
                new BoundsInt((Vector3Int)startPosition, new Vector3Int(mapWidth, mapHeight, 0)),
                minRoomSize, maxRoomSize);
        }
        else
        {
            roomsList = ProceduralGenerationAlgorithms.CreateVariableSizeRooms(
                new BoundsInt((Vector3Int)startPosition, new Vector3Int(mapWidth, mapHeight, 0)), 
                minRoomSize, maxRoomSize, roomSpacing); // Передаем зазор в метод
        }
        
        

        HashSet<Vector2Int> floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        _mapData.Path.UnionWith(corridors);
        floor.UnionWith(corridors);

        var expandedFloor = ExpandFloor(floor);
        var specialWallPositions = new HashSet<Vector2Int>(expandedFloor);
        specialWallPositions.ExceptWith(floor);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(expandedFloor, specialWallPositions, tilemapVisualizer);
        tilemapVisualizer.AddWallColliders();
        tilemapVisualizer.DefineLayers();
        
        roomDataExtractor.ProcessRooms(_mapData, tilemapVisualizer);
        DefineSpecialRooms();
        enemyAndPropPlacementManager.ProcessRooms(_mapData, furnitureContainer, enemyContainer);
        
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
            _mapData.Rooms.Add(new Room((Vector2Int)Vector3Int.RoundToInt(room.center), roomFloor));
        }

        return floor;
    }

    private void DefineSpecialRooms()
    {
        int startRoomIndex = 0;
        _mapData.startRooom = _mapData.Rooms[startRoomIndex];
        _mapData.Rooms.RemoveAt(startRoomIndex);
        
        int endRoomIndex = _mapData.Rooms.Count - 1;
        _mapData.endRoom = _mapData.Rooms[endRoomIndex];
        _mapData.Rooms.RemoveAt(endRoomIndex);

        
        int techRoomStartIndex = Convert.ToInt32(_mapData.Rooms.Count / 2);
        int techRoomEndIndex = _mapData.Rooms.Count - 2;
        int techRoomIndex = UnityEngine.Random.Range(techRoomStartIndex, techRoomEndIndex);
        _mapData.techRoom = _mapData.Rooms[techRoomIndex];
        _mapData.Rooms.RemoveAt(techRoomIndex);
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


