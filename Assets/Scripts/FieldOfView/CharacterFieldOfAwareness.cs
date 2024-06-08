using System.Linq;
using GameMaster;
using UnityEngine;

namespace FieldOfView
{
    public class CharacterFieldOfAwareness : MonoBehaviour
    {
        [Range(0.5f, 1.5f)] public float passiveViewRadius;
        [Range(1, 8)] public float activeViewRadius;
        [Range(0, 360)] public float viewAngle;
        [Range(1, 4)] public int density;
        public LayerMask obstacleMask;
        public LayerMask enemyMask;

        private Camera _camera;
        private float _angle;
        private State _flashLightState;
        
        private void Start()
        {
            _camera = Camera.main;
            _flashLightState = ServiceLocator.Get.Locate<State>("flashLightState");
        }

        private void Update() => SetAware();

        private void FixedUpdate()
        {
            var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = Utils.ReduceDimension(transform.position);
            _angle = Utils.GetAngleBetweenVectors(character, mouse);
        }

        private void SetAware()
        {
            var position = Utils.ReduceDimension(transform.position);
            Utils.ProduceAngles(_angle, viewAngle, density, _flashLightState.Get)
                .Select(data => Utils.CalculateEnemyTouchPoint(position, data.Angle,
                    data.IsInFieldOfView ? activeViewRadius : passiveViewRadius, obstacleMask, enemyMask))
                .Where(someGameObject => someGameObject is not null)
                .ToList()
                .ForEach(someGameObject => someGameObject.GetComponent<EnemyBehaviour>().FlashEnemyWithLight());
        }
    }
}