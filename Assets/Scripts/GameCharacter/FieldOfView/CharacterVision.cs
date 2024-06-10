using System.Linq;
using GameMaster;
using GameMaster.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCharacter.FieldOfView
{
    public class CharacterVision : MonoBehaviour
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
        private BooleanState _flashLightState;
        private GameInput _gameInput;

        private void Awake()
        {
            _gameInput = new GameInput();
            _camera = Camera.main;
            _viewMesh = new Mesh
            {
                name = "View Mesh"
            };
            viewMeshFilter.mesh = _viewMesh;
        }

        private void Start()
        {
            _flashLightState = ServiceLocator.Get.Locate<BooleanState>("flashLightState");
            _meshProducer = new DarknessMeshProducer(
                darknessRadius: darknessRadius,
                minimumRadius: passiveViewRadius,
                maximumRadius: activeViewRadius,
                density: density,
                obstacleMask: obstacleMask,
                transformer: position => transform.InverseTransformPoint(position),
                flashLightState: _flashLightState
            );
        }

        private void Update()
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

        private void FixedUpdate()
        {
            var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = Utils.ReduceDimension(transform.position);
            _angle = Utils.GetAngleBetweenVectors(character, mouse);
        }
        
        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Flashlight.performed += HandleFlashLightClick;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Flashlight.performed -= HandleFlashLightClick;
        }

        private void HandleFlashLightClick(InputAction.CallbackContext value) => _flashLightState.Toggle();
    }
}
