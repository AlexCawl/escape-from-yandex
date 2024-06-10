using System;
using UnityEngine;

public abstract class AbstractMapGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected GameObject furnitureContainer;
    [SerializeField] protected GameObject enemyContainer;

    public void GenerateMap()
    {
        tilemapVisualizer.Clear();
        DeleteAllProps();
        RunProceduralGeneration();
    }
    
    public void DeleteAllProps()
    {
        foreach (Transform furniture in furnitureContainer.transform)
        {
            DestroyImmediate(furniture.gameObject);
        }

        foreach (Transform enemy in enemyContainer.transform)
        {
            DestroyImmediate(enemy.gameObject);
        }

        try
        {
            var player = GameObject.FindWithTag("Player");
            DestroyImmediate(player);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    protected abstract void RunProceduralGeneration();
}