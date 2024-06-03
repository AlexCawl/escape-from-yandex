using UnityEngine;
#if UNITY_EDITOR
#endif

public abstract class AbstractMapGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected GameObject propsContainer;

    public void GenerateMap()
    {
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
    }
    
    public void DeleteAllProps()
    {
        foreach (Transform child in propsContainer.transform)
        {
            #if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
            #else
            Destroy(child.gameObject);
            #endif
        }
    }

    protected abstract void RunProceduralGeneration();
}