using System.Collections.Generic;
using UnityEngine;

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