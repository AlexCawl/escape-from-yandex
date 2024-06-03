using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject parentObject; // Префаб объекта, который будет создан
    

    public void ClearAllChildren()
    {
        foreach (Transform child in parentObject.transform)
        {
            #if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
            #else
            Destroy(child.gameObject);
            #endif
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectGenerator))]
public class ObjectGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectGenerator script = (ObjectGenerator)target;
        if (GUILayout.Button("Clear All Children"))
        {
            script.ClearAllChildren();
        }
    }
}
#endif