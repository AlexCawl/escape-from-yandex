using System.Collections.Generic;
using System.Linq;
using GameMaster;
using UnityEngine;

namespace FieldOfView
{
    public interface IMeshProducer
    {
        public MeshData Render(float directionOfViewAngle, float viewAngle, Vector2 position);
    }

    public class DarknessMeshProducer : IMeshProducer
    {
        private readonly float _darknessRadius;
        private readonly float _minimumRadius;
        private readonly float _maximumRadius;
        private readonly int _density;
        private readonly LayerMask _obstacleMask;
        private readonly FromGlobalToLocalSpace _transformer;
        private readonly BooleanState _flashLightState;

        public DarknessMeshProducer(
            float darknessRadius, float minimumRadius, float maximumRadius, int density,
            LayerMask obstacleMask, FromGlobalToLocalSpace transformer, BooleanState flashLightState)
        {
            _darknessRadius = darknessRadius;
            _minimumRadius = minimumRadius;
            _maximumRadius = maximumRadius;
            _density = density;
            _obstacleMask = obstacleMask;
            _transformer = transformer;
            _flashLightState = flashLightState;
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

        private List<Vector2> CalculateMeshPoints(float directionOfViewAngle, float viewAngle, Vector2 position) =>
            Utils.ProduceAngles(directionOfViewAngle, viewAngle, _density, _flashLightState.Get).SelectMany(
                data =>
                {
                    var points = new Vector2[2];
                    if (data.IsInFieldOfView)
                    {
                        points[0] = Utils.CalculatePropTouchPoint(position, data.Angle, _maximumRadius, _obstacleMask);
                    }
                    else
                    {
                        points[0] = Utils.CalculatePropTouchPoint(position, data.Angle, _minimumRadius, _obstacleMask);
                    }

                    points[1] = Utils.ConstructRay(position, data.Angle, _darknessRadius);
                    return points;
                }
            ).ToList();

        private void CalculateMeshTriangles(List<Vector2> points, int vertexCount, Vector2[] vertices, int[] triangles)
        {
            for (var i = 0; i < vertexCount; i++)
            {
                vertices[i] = _transformer(points[i]);
                if (i % 2 == 0)
                {
                    triangles[i * 3] = i % vertexCount;
                    triangles[i * 3 + 1] = (i + 2) % vertexCount;
                    triangles[i * 3 + 2] = (i + 1) % vertexCount;
                }
                else
                {
                    triangles[i * 3] = i % vertexCount;
                    triangles[i * 3 + 1] = (i + 1) % vertexCount;
                    triangles[i * 3 + 2] = (i + 2) % vertexCount;
                }
            }
        }
    }
}