using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Behaviour;
using GameMaster;
using GameMaster.State;
using UnityEngine;

namespace GameCharacter.FieldOfView
{
    public class CharacterSight : MonoBehaviour
    {
        [Range(0.5f, 1.5f)] public float passiveViewRadius;
        [Range(1, 8)] public float activeViewRadius;
        [Range(0, 360)] public float viewAngle;
        [Range(1, 4)] public int density;
        public LayerMask obstacleMask;
        public LayerMask enemyMask;

        private float _angle;
        private Camera _camera;
        private BooleanState _flashLightState;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            _flashLightState = ServiceLocator.Get.Locate<BooleanState>("flashLightState");
        }

        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        private void Update()
        {
            var position = Utils.ReduceDimension(transform.position);
            Utils.ProduceAngles(_angle, viewAngle, density, _flashLightState.Get)
                .Select(data => Utils.CalculateEnemyTouchPoint(position, data.Angle,
                    data.IsInFieldOfView ? activeViewRadius : passiveViewRadius, obstacleMask, enemyMask))
                .Where(someGameObject => someGameObject is not null)
                .ToList()
                .ForEach(someGameObject =>
                {
                    try
                    {
                        someGameObject.GetComponent<VisibleOnlyInLightBehaviour>().Highlight();
                    }
                    catch (Exception)
                    {
                    }
                });
        }

        private void FixedUpdate()
        {
            var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = Utils.ReduceDimension(transform.position);
            _angle = Utils.GetAngleBetweenVectors(character, mouse);
        }
    }
}