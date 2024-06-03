using System.Collections.Generic;
using UnityEngine;

public static class Direction2D
{
    public static readonly List<Vector2Int> CardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // UP
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, 0) // LEFT
    };

    public static readonly List<Vector2Int> DiagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, -1), // RIGHT-DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 1) // LEFT-UP
    };

    public static List<Vector2Int> EightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // UP
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(1, -1), // RIGHT-DOWN
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0), // LEFT
        new Vector2Int(-1, 1) // LEFT-UP
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return CardinalDirectionsList[Random.Range(0, CardinalDirectionsList.Count)];
    }
}