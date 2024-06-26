using System.Collections.Generic;
using UnityEngine;


public static class WallGenerator
{
    // ReSharper disable Unity.PerformanceAnalysis
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> specialWallPositions,
        TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = 
            FindWallsInDirections(floorPositions, Direction2D.CardinalDirectionsList);
        var cornerWallPositions = 
            FindWallsInDirections(floorPositions, Direction2D.DiagonalDirectionsList);
        
       
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateSpecialWalls(tilemapVisualizer, specialWallPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
        
        
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, 
        HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.EightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                    neighboursBinaryType += "1";
                else
                    neighboursBinaryType += "0";
            }

            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
        }
        
    }

    private static void CreateBasicWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions,
        HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.CardinalDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                    neighboursBinaryType += "1";
                else
                    neighboursBinaryType += "0";
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType);
        }
    }

    private static void CreateSpecialWalls(TilemapVisualizer tilemapVisualizer ,HashSet<Vector2Int> specialWallPositions)
    {
        foreach (var position in specialWallPositions)
        {
            tilemapVisualizer.PaintSpecialWalls(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections
        (HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (!floorPositions.Contains(neighbourPosition))
                    wallPositions.Add(neighbourPosition);
            }
        }

        return wallPositions;
    }
    
}