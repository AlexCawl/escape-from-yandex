using System.Linq;
using GameMaster;
using UnityEngine;

namespace FieldOfView
{
    public class CharacterFieldOfView : MonoBehaviour
    {
        [Range(0.5f, 1.5f)] public float passiveViewRadius;
        [Range(1, 8)] public float activeViewRadius;
        [Range(10, 30)] public float darknessRadius;
        [Range(0, 360)] public float viewAngle;
        [Range(1, 4)] public int density;
        public LayerMask obstacleMask;
        public MeshFilter viewMeshFilter;

        private float _angle;
        private Mesh _viewMesh;
        private Camera _camera;
        private IMeshProducer _meshProducer;
        
        private void Start()
        {
            _viewMesh = new Mesh
            {
                name = "View Mesh"
            };
            viewMeshFilter.mesh = _viewMesh;
            _camera = Camera.main;
            _meshProducer = new DarknessMeshProducer(
                darknessRadius: darknessRadius,
                minimumRadius: passiveViewRadius,
                maximumRadius: activeViewRadius,
                density: density,
                obstacleMask: obstacleMask,
                transformer: position => transform.InverseTransformPoint(position),
                ServiceLocator.Get.Locate<State>("flashLightState")
            );
        }

        private void Update() => SetView();

        private void FixedUpdate()
        {
            var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = Utils.ReduceDimension(transform.position);
            _angle = Utils.GetAngleBetweenVectors(character, mouse);
        }

        private void SetView()
        {
            var position = Utils.ReduceDimension(transform.position);
            var meshData = _meshProducer.Render(_angle, viewAngle, position);
            _viewMesh.Clear();
            _viewMesh.vertices = meshData.Vertices
                .Select(vector2 => Utils.IncreaseDimension(vector2, transform.position.z))
                .ToArray();
            _viewMesh.triangles = meshData.Triangles;
            _viewMesh.RecalculateNormals();
        }
    }
}
