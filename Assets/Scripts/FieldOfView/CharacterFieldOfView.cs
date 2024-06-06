using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace FieldOfView
{
    public class CharacterFieldOfView : MonoBehaviour
    {
        [Range(0.5f, 1.5f)] public float passiveViewRadius;
        [Range(1, 8)] public float activeViewRadius;
        [Range(10, 30)] public float darknessRadius;
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
            var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = Utils.ReduceDimension(transform.position);
            _angle = Utils.GetAngleBetweenVectors(character, mouse);
        }

        private void DrawFieldOfView()
        {
            var position = Utils.ReduceDimension(transform.position);
            var meshData = _meshProducer.Render(_angle, viewAngle, position);
            var vertices = meshData.Vertices
                .Select(vector2 => Utils.IncreaseDimension(vector2, transform.position.z))
                .ToArray();
            var triangles = meshData.Triangles;
            _viewMesh.Clear();
            _viewMesh.vertices = vertices;
            _viewMesh.triangles = triangles;
            _viewMesh.RecalculateNormals();
        }
    }
}
