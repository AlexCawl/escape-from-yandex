using UnityEngine;

namespace FieldOfView
{
    public interface IMeshProducer
    {
        public MeshData Render(float directionOfViewAngle, float viewAngle, Vector2 position);
    }

    public struct MeshData
    {
        public readonly Vector2[] Vertices;
        public readonly int[] Triangles;

        public MeshData(Vector2[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }

    public delegate Vector2 FromGlobalToLocalSpace(Vector2 globalPosition);

    public static class Utils
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
}