using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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