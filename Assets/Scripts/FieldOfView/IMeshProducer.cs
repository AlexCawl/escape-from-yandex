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
}