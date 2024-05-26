using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class SimpleRandomWalkMapGenerator : AbstractMapGenerator
{
    [SerializeField] private SimpleRandomWalkData _randomWalkParameters;


    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk()
    {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < _randomWalkParameters.iterations; i++)
        {
            var path = 
                ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, _randomWalkParameters.walkLength);
            floorPositions.UnionWith(path);

            if (_randomWalkParameters.startRandomlyEachIteration)
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }

        return floorPositions;
    }
}