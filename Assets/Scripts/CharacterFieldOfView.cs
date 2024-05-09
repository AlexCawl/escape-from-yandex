using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    public float meshResolution;
    public LayerMask obstacleMask;

    private Mesh _viewMesh;
    public MeshFilter viewMeshFilter;

    private Camera _camera;
    private float _angle;

    private void Awake()
    {
        _viewMesh = new Mesh
        {
            name = "View Mesh"
        };
        viewMeshFilter.mesh = _viewMesh;
        _camera = Camera.main;
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
        var points = GetFieldOfViewPoints();
        points.ForEach(vector3 => Debug.DrawLine(transform.position, vector3, Color.magenta));
        var vertexCount = points.Count + 1;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (var i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(points[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    private List<Vector3> GetFieldOfViewPoints()
    {
        var characterPosition = FowUtils.ReduceDimension(transform.position);
        var steps = Mathf.RoundToInt(viewAngle * meshResolution);
        var stepSize = viewAngle / steps;

        var points = new List<Vector3>();
        for (var i = 0; i <= steps; i++)
        {
            var stepAngle = _angle + viewAngle / 2 - stepSize * i;
            var hitRay = MakeRayCast(characterPosition, stepAngle);
            var maxRay = FowUtils.ConstructRay(characterPosition, stepAngle, viewRadius);
            switch (hitRay.Hit)
            {
                case true:
                    points.Add(FowUtils.IncreaseDimension(hitRay.Point, transform.position.z));
                    break;
                case false:
                    points.Add(FowUtils.IncreaseDimension(maxRay, transform.position.z));
                    break;
            }
        }

        return points;
    }

    private RayCast MakeRayCast(Vector2 center, float angle)
    {
        var dir = FowUtils.GetVectorFromAngle(angle);
        var hit = Physics2D.Raycast(center, dir, viewRadius, obstacleMask.value);
        return new RayCast(hit.point, hit.collider is not null);
    }


    public Tuple<Vector3, Vector3, Vector3> DebugSightVectors()
    {
        var height = transform.position.z;
        var sight = FowUtils.IncreaseDimension(FowUtils.GetVectorFromAngle(_angle) * viewRadius, height);
        var left = FowUtils.IncreaseDimension(FowUtils.GetVectorFromAngle(_angle + viewAngle / 2) * viewRadius, height);
        var right = FowUtils.IncreaseDimension(FowUtils.GetVectorFromAngle(_angle - viewAngle / 2) * viewRadius, height);
        return new Tuple<Vector3, Vector3, Vector3>(left, sight, right);
    }
}

internal struct RayCast
{
    public readonly bool Hit;
    public Vector2 Point;

    public RayCast(Vector2 point, bool hit)
    {
        Point = point;
        Hit = hit;
    }
}