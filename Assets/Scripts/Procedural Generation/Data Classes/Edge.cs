using UnityEngine;

public class Edge
{
    public Vector2Int RoomA { get; }
    public Vector2Int RoomB { get; }
    public float Weight { get; }

    public Edge(Vector2Int roomA, Vector2Int roomB)
    {
        RoomA = roomA;
        RoomB = roomB;
        Weight = Vector2Int.Distance(roomA, roomB);
    }
}