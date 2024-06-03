using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    
    public static List<BoundsInt> CreateVariableSizeRooms(BoundsInt spaceToSplit, int minRoomSize, int maxRoomSize,
        int roomSpacing)
    {
        List<BoundsInt> roomsList = new List<BoundsInt>();

        int x = spaceToSplit.xMin;
        while (x <= spaceToSplit.xMax - minRoomSize)
        {
            int y = spaceToSplit.yMin;
            while (y <= spaceToSplit.yMax - minRoomSize)
            {
                int roomWidth = Random.Range(minRoomSize, maxRoomSize + 1);
                int roomHeight = Random.Range(minRoomSize, maxRoomSize + 1);
                roomWidth = Mathf.Min(roomWidth, spaceToSplit.xMax - x);
                roomHeight = Mathf.Min(roomHeight, spaceToSplit.yMax - y);

                BoundsInt newRoom = new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(roomWidth, roomHeight, 0));
                roomsList.Add(newRoom);

                y += roomHeight + roomSpacing; // Добавляем зазор между комнатами по вертикали
            }

            int maxRoomWidthInRow = roomsList
                .Where(room => room.min.x == x)
                .Max(room => room.size.x);
            x += maxRoomWidthInRow + roomSpacing; // Обеспечиваем минимальный шаг с учетом зазора
        }

        return roomsList;
    }


    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                        SplitHorizontally(minHeight, roomsQueue, room);
                    else if (room.size.x >= minWidth * 2)
                        SplitVertically(minWidth, roomsQueue, room);
                    else
                        roomsList.Add(room);
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                        SplitVertically(minWidth, roomsQueue, room);
                    else if (room.size.y >= minHeight * 2)
                        SplitHorizontally(minHeight, roomsQueue, room);
                    else
                        roomsList.Add(room);
                }
            }
        }

        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(minWidth, room.size.x - minWidth);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(minHeight, room.size.y - minHeight);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}