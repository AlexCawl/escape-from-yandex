using System.Linq;
using UnityEngine;

public class CharacterFieldOfView : MonoBehaviour
{
    [Range(0.5f, 1.5f)] public float passiveViewRadius;
    [Range(1, 8)] public float activeViewRadius;
    [Range(10, 15)] public float darknessRadius;
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
            darknessRadius: darknessRadius,
            minimumRadius: passiveViewRadius,
            maximumRadius: activeViewRadius,
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
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
}
