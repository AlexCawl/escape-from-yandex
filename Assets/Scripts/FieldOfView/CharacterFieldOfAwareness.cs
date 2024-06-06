using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FieldOfView
{
    public class CharacterFieldOfAwareness : MonoBehaviour
    {
        [Range(0.5f, 1.5f)] public float passiveViewRadius;
        [Range(1, 8)] public float activeViewRadius;
        [Range(0, 360)] public float viewAngle;
        public LayerMask obstacleMask;
        public LayerMask enemyMask;
    
        private Camera _camera;
        private float _angle;

        private const float Circle = 360f;
        private const float Density = 1f;
    
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            AwareEnemies();
        }
    
        private void FixedUpdate()
        {
            var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = Utils.ReduceDimension(transform.position);
            _angle = Utils.GetAngleBetweenVectors(character, mouse);
        }

        private void AwareEnemies()
        {
            var position = Utils.ReduceDimension(transform.position);
            ProduceAngles(_angle, viewAngle)
                .Select(data => Utils.CalculateEnemyTouchPoint(position, data.Angle,
                    data.IsInFieldOfView ? activeViewRadius : passiveViewRadius, obstacleMask, enemyMask))
                .Where(someGameObject => someGameObject is not null)
                .ToList()
                .ForEach(someGameObject => someGameObject.GetComponent<EnemyBehaviour>().FlashEnemyWithLight());
        }
    
        private static List<AngleData> ProduceAngles(float directionOfViewAngle, float viewAngle)
        {
            var steps = Mathf.RoundToInt(Circle * Density);
            var stepSize = Circle / steps;
            return Utils.FloatRange(0f, 360f, stepSize)
                .Select(stepAngle => new AngleData(stepAngle, IsAngleInFov(directionOfViewAngle, viewAngle, stepAngle)))
                .ToList();
        }
        
        private static bool IsAngleInFov(float directionOfViewAngle, float viewAngle, float angle)
        {
            return Math.Min(
                360 - Math.Abs(directionOfViewAngle - angle),
                Math.Abs(directionOfViewAngle - angle)
            ) <= viewAngle / 2;
        }
    }
}
