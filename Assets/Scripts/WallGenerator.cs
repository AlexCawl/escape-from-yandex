using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        // var basicWallPositions = 
        //     FindWallsInDirections(floorPositions, Direction2D.CardinalDirectionsList);
        
        var topWallFirstPositions = FindWallsInDirections
            (floorPositions, new List<Vector2Int> {Direction2D.CardinalDirectionsList[0]});
        var rightWallPositions = FindWallsInDirections
            (floorPositions, new List<Vector2Int> {Direction2D.CardinalDirectionsList[1]});
        var bottomWallPositions = FindWallsInDirections
            (floorPositions, new List<Vector2Int> {Direction2D.CardinalDirectionsList[2]});
        var leftWallPositions = FindWallsInDirections
            (floorPositions, new List<Vector2Int> {Direction2D.CardinalDirectionsList[3]});
        var cornerWallPositions = FindWallsInDirections
            (floorPositions, Direction2D.DiagonalDirectionsList);

        var topWallSecondPositions = FindWallsInDirectionsSecondLevel
                (floorPositions, topWallFirstPositions,
                    new List<Vector2Int> {Direction2D.CardinalDirectionsList[0]});
        var sideWallsSecondLevel = FindWallsInDirectionsSecondLevel
        (floorPositions, topWallFirstPositions, new List<Vector2Int>
            {
                Direction2D.CardinalDirectionsList[1],
                Direction2D.CardinalDirectionsList[3]
            });

        var sideCornerWallsSecondLevel = FindWallsInDirectionsSecondLevel
        (floorPositions, topWallSecondPositions, new List<Vector2Int>
            {
                Direction2D.CardinalDirectionsList[1],
                Direction2D.CardinalDirectionsList[3]
            });
        
        CreateBasicWalls(tilemapVisualizer, topWallFirstPositions, floorPositions);
        CreateBasicWalls(tilemapVisualizer, leftWallPositions, floorPositions);
        CreateBasicWalls(tilemapVisualizer, rightWallPositions, floorPositions);
        CreateBasicWalls(tilemapVisualizer, bottomWallPositions , floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
        CreateTopWallsSecondLevel(tilemapVisualizer, topWallSecondPositions);
        CreateSideWallsSecondLevel(tilemapVisualizer, sideWallsSecondLevel, 
            topWallFirstPositions, false);
        CreateSideWallsSecondLevel(tilemapVisualizer, sideCornerWallsSecondLevel, 
            topWallSecondPositions, true);
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
    
    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions,
        HashSet<Vector2Int> floorPositions)
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

    private static void CreateTopWallsSecondLevel(TilemapVisualizer tilemapVisualizer, 
        HashSet<Vector2Int> wallPositions)
    {
        foreach (var position in wallPositions)
        {
            tilemapVisualizer.PaintTopWallSecondLevel(position);
        }
    }
    
    private static void CreateSideWallsSecondLevel(TilemapVisualizer tilemapVisualizer, 
        HashSet<Vector2Int> wallPositions, HashSet<Vector2Int> topWallPositions, bool cornerCheck)
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Direction2D.CardinalDirectionsList[1],
            Direction2D.CardinalDirectionsList[3]
        };
        foreach (var position in wallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in directions)
            {
                var neighbourPosition = position + direction;
                if (topWallPositions.Contains(neighbourPosition))
                    neighboursBinaryType += "1";
                else
                    neighboursBinaryType += "0";
            }
            if (cornerCheck)
                tilemapVisualizer.PaintSingleSideCornerWallsSecondLevel(position, neighboursBinaryType);
            else 
                tilemapVisualizer.PaintSingleSideWallsSecondLevel(position, neighboursBinaryType);
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
    
    private static HashSet<Vector2Int> FindWallsInDirectionsSecondLevel
        (HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> wallPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> secondWallPositions = new HashSet<Vector2Int>();
        foreach (var position in wallPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (!floorPositions.Contains(neighbourPosition) && !wallPositions.Contains(neighbourPosition))
                    secondWallPositions.Add(neighbourPosition);
            }
            
        }

        return secondWallPositions;
    }
}