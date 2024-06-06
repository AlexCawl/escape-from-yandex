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
            var mouse = FowUtils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = FowUtils.ReduceDimension(transform.position);
            _angle = FowUtils.GetAngleBetweenVectors(character, mouse);
        }

        private void AwareEnemies()
        {
            var position = FowUtils.ReduceDimension(transform.position);
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
            return FloatRange(0f, 360f, stepSize)
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
}
