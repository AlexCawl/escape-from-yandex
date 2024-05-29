using System;
using System.Collections.Generic;
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

internal struct MeshData
{
    public readonly Vector2[] Vertices;
    public readonly int[] Triangles;

    public MeshData(Vector2[] vertices, int[] triangles)
    {
        Vertices = vertices;
        Triangles = triangles;
    }
}

internal interface IMeshProducer
{
    public MeshData Render(float directionOfViewAngle, float viewAngle, Vector2 position);
}

internal delegate Vector2 FromGlobalToLocalSpace(Vector2 globalPosition);

internal class FlashlightMesh : IMeshProducer
{
    private readonly float _radius;
    private readonly float _density;
    private readonly LayerMask _obstacleMask;
    private readonly FromGlobalToLocalSpace _transformer;

    public FlashlightMesh(float radius, float density, LayerMask obstacleMask, FromGlobalToLocalSpace transformer)
    {
        _radius = radius;
        _density = density;
        _obstacleMask = obstacleMask;
        _transformer = transformer;
    }

    public MeshData Render(float directionOfViewAngle, float viewAngle, Vector2 position)
    {
        var points = CalculateMeshPoints(directionOfViewAngle, viewAngle, position);
        var vertexCount = points.Count + 1;
        var vertices = new Vector2[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];
        CalculateMeshTriangles(points, vertexCount, vertices, triangles);
        return new MeshData(vertices, triangles);
    }

    private List<Vector2> CalculateMeshPoints(float directionOfViewAngle, float viewAngle, Vector2 position)
    {
        var steps = Mathf.RoundToInt(viewAngle * _density);
        var stepSize = viewAngle / steps;
        var points = new List<Vector2>();
        for (var i = 0; i <= steps; i++)
        {
            var stepAngle = directionOfViewAngle + viewAngle / 2 - stepSize * i;
            var touchPoint = Utils.CalculateTouchPoint(position, stepAngle, _radius, _obstacleMask);
            points.Add(touchPoint);
        }

        return points;
    }

    private void CalculateMeshTriangles(List<Vector2> points, float vertexCount, Vector2[] vertices, int[] triangles)
    {
        vertices[0] = Vector2.zero;
        for (var i = 0; i < vertexCount; i++)
        {
            vertices[i + 1] = _transformer(points[i]);
            if (!(i < vertexCount - 2)) continue;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
    }
}

internal class DarknessEffectMesh : IMeshProducer
{
    private readonly float _darknessRadius;
    private readonly float _minimumRadius;
    private readonly float _maximumRadius;
    private readonly float _density;
    private readonly LayerMask _obstacleMask;
    private readonly FromGlobalToLocalSpace _transformer;

    private const float Circle = 360f;

    public DarknessEffectMesh(
        float darknessRadius, float minimumRadius, float maximumRadius, float density,
        LayerMask obstacleMask, FromGlobalToLocalSpace transformer)
    {
        _darknessRadius = darknessRadius;
        _minimumRadius = minimumRadius;
        _maximumRadius = maximumRadius;
        _density = density;
        _obstacleMask = obstacleMask;
        _transformer = transformer;
    }

    public MeshData Render(float directionOfViewAngle, float viewAngle, Vector2 position)
    {
        var points = CalculateMeshPoints(directionOfViewAngle, viewAngle, position);
        var vertexCount = points.Count;
        var vertices = new Vector2[vertexCount];
        var triangles = new int[vertexCount * 3];
        CalculateMeshTriangles(points, vertexCount, vertices, triangles);
        return new MeshData(vertices, triangles);
    }

    private List<AngleData> ProduceAngles(float directionOfViewAngle, float viewAngle)
    {
        var steps = Mathf.RoundToInt(Circle * _density);
        var stepSize = Circle / steps;
        return FloatRange(0f, 360f, stepSize)
            .Select(stepAngle => IsAngleInFov(directionOfViewAngle, viewAngle, stepAngle)
                ? new AngleData(stepAngle, true)
                : new AngleData(stepAngle, false))
            .ToList();
    }

    private static bool IsAngleInFov(float directionOfViewAngle, float viewAngle, float angle)
    {
        return 360 + directionOfViewAngle - viewAngle / 2 < 360 + angle &&
               360 + angle < 360 + directionOfViewAngle + viewAngle / 2;
    }

    private List<Vector2> CalculateMeshPoints(float directionOfViewAngle, float viewAngle, Vector2 position)
    {
        return ProduceAngles(directionOfViewAngle, viewAngle).SelectMany(
            data =>
            {
                var points = new Vector2[2];
                if (data.IsInFieldOfView)
                {
                    points[0] = Utils.CalculateTouchPoint(position, data.Angle, _maximumRadius, _obstacleMask);
                }
                else
                {
                    points[0] = FowUtils.ConstructRay(position, data.Angle, _minimumRadius);
                }

                points[1] = FowUtils.ConstructRay(position, data.Angle, _darknessRadius);
                return points;
            }
        ).ToList();
    }

    private void CalculateMeshTriangles(List<Vector2> points, int vertexCount, Vector2[] vertices, int[] triangles)
    {
        for (var i = 0; i < vertexCount; i++)
        {
            vertices[i] = _transformer(points[i]);
            triangles[i * 3] = i % vertexCount;
            triangles[i * 3 + 1] = (i + 1) % vertexCount;
            triangles[i * 3 + 2] = (i + 2) % vertexCount;
        }
    }

    private struct AngleData
    {
        public readonly float Angle;
        public readonly bool IsInFieldOfView;

        public AngleData(float angle, bool isInFieldOfView)
        {
            Angle = angle;
            IsInFieldOfView = isInFieldOfView;
        }
    }

    private static IEnumerable<float> FloatRange(float min, float max, float step)
    {
        for (var i = 0; i < int.MaxValue; i++)
        {
            var value = min + step * i;
            if (value >= max)
            {
                break;
            }

            yield return value;
        }
    }
}

internal static class Utils
{
    public static Vector2 CalculateTouchPoint(Vector2 from, float angle, float radius, LayerMask obstacleMask)
    {
        var dir = FowUtils.GetVectorFromAngle(angle);
        var hit = Physics2D.Raycast(from, dir, radius, obstacleMask.value);
        if (hit.collider is not null)
        {
            return hit.point;
        }

        var boundPoint = FowUtils.ConstructRay(from, angle, radius);
        return boundPoint;
    }
}