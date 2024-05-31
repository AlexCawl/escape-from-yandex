using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomMapGenerator : AbstractMapGenerator
{
    [SerializeField] private int minRoomSize = 4;
    [SerializeField] private int maxRoomSize = 4;
    [SerializeField] private int mapWidth = 20, mapHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] [Range(0, 20)] private int roomOffset = 2;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        // var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
        //     new Vector3Int(mapWidth, mapHeight, 0)), minRoomWidth, minRoomHeight);
        
        var roomsList = ProceduralGenerationAlgorithms.CreateVariableSizeRooms
        (new BoundsInt((Vector3Int)startPosition, 
            new Vector3Int(mapWidth, mapHeight, 0)), 
            minRoomSize, maxRoomSize, roomOffset);

        HashSet<Vector2Int> floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        var expandedFloor = ExpandFloor(floor);
        var specialWallPositions = new HashSet<Vector2Int>(expandedFloor);
        specialWallPositions.ExceptWith(floor);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(expandedFloor, specialWallPositions, tilemapVisualizer);
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int currentPosition = start;

        while (currentPosition != end)
        {
            if (currentPosition.x != end.x)
            {
                currentPosition.x += (end.x > currentPosition.x) ? 1 : -1;
            }
            else if (currentPosition.y != end.y)
            {
                currentPosition.y += (end.y > currentPosition.y) ? 1 : -1;
            }
            corridor.Add(currentPosition);
        }

        return corridor;
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
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
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
