using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
#endif

public abstract class AbstractMapGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected GameObject furnitureContainer;
    [SerializeField] protected GameObject enemyContainer;

    public void GenerateMap()
    {
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
    }
    
    public void DeleteAllProps()
    {
        foreach (Transform furniture in furnitureContainer.transform)
        {
            #if UNITY_EDITOR
            DestroyImmediate(furniture.gameObject);
            #else
            Destroy(child.gameObject);
            #endif
        }

        foreach (Transform enemy in enemyContainer.transform)
        {
            #if UNITY_EDITOR
            DestroyImmediate(enemy.gameObject);
            #else
            Destroy(child.gameObject);
            #endif
        }
    }

    protected abstract void RunProceduralGeneration();
}