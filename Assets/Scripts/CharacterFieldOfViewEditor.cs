using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterFieldOfView))]
public class CharacterFieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        var fow = (CharacterFieldOfView) target;
        var vectors = fow.DebugSightVectors();
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.up, 360, fow.viewRadius);
        
        Handles.color = Color.red;
        Handles.DrawLine(fow.transform.position, fow.transform.position + vectors.Item2);
        
        Handles.color = Color.blue;
        Handles.DrawLine(fow.transform.position, fow.transform.position + vectors.Item1);
        
        Handles.color = Color.green;
        Handles.DrawLine(fow.transform.position, fow.transform.position + vectors.Item3);
    }
}