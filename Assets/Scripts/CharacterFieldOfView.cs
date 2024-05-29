using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterFieldOfView : MonoBehaviour
{
    [Range(1, 8)] public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    public float meshResolution;
    public LayerMask obstacleMask;

    private Mesh _viewMesh;
    public MeshFilter viewMeshFilter;

    private Camera _camera;
    private float _angle;
    private IMeshProducer _meshProducer;

    private void Awake()
    {
        _viewMesh = new Mesh
        {
            name = "View Mesh"
        };
        viewMeshFilter.mesh = _viewMesh;
        _camera = Camera.main;
        _meshProducer = new DarknessEffectMesh(
            darknessRadius: 10f,
            minimumRadius: 0.5f,
            maximumRadius: 5f,
            density: meshResolution,
            obstacleMask: obstacleMask,
            transformer: position => transform.InverseTransformPoint(position)
        );
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    private void FixedUpdate()
    {
        var mouse = FowUtils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
        var character = FowUtils.ReduceDimension(transform.position);
        _angle = FowUtils.GetAngleBetweenVectors(character, mouse);
    }

    private void DrawFieldOfView()
    {
        var position = FowUtils.ReduceDimension(transform.position);
        var meshData = _meshProducer.Render(_angle, viewAngle, position);
        var vertices = meshData.Vertices
            .Select(vector2 => FowUtils.IncreaseDimension(vector2, transform.position.z))
            .ToArray();
        var triangles = meshData.Triangles;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i % 2 == 1)
            {
                var color = Color.magenta;
                Debug.DrawLine(
                    transform.TransformDirection(vertices[(i + 0) % vertices.Length]), 
                    transform.TransformDirection(vertices[(i + 1) % vertices.Length]), color);
                Debug.DrawLine(
                    transform.TransformDirection(vertices[(i + 1) % vertices.Length]), 
                    transform.TransformDirection(vertices[(i + 2) % vertices.Length]), color);
                Debug.DrawLine(
                    transform.TransformDirection(vertices[(i + 2) % vertices.Length]), 
                    transform.TransformDirection(vertices[(i + 0) % vertices.Length]), color);
            }
            
        }
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
}
