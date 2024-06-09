using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCharacter.FieldOfView
{
    public static class Utils
    {
        private const int Circle = 360;
            
        public static Vector2 CalculatePropTouchPoint(Vector2 from, float angle, float radius, LayerMask obstacleMask)
        {
            var dir = GetVectorFromAngle(angle);
            var hit = Physics2D.Raycast(from, dir, radius, obstacleMask.value);
            if (hit.collider is not null)
            {
                return hit.point;
            }

            var boundPoint = ConstructRay(from, angle, radius);
            return boundPoint;
        }
        
        public static GameObject CalculateEnemyTouchPoint(Vector2 from, float angle, float radius, LayerMask obstacleMask, LayerMask enemyMask)
        {
            var dir = GetVectorFromAngle(angle);
            var propHit = Physics2D.Raycast(from, dir, radius, obstacleMask.value);
            var enemyHit = Physics2D.Raycast(from, dir, radius, enemyMask.value);
            if (enemyHit.collider is null) return null;
            if (propHit.collider is not null)
            {
                return enemyHit.distance < propHit.distance ? enemyHit.collider.gameObject : null;
            }
            return enemyHit.collider.gameObject;
        }
        
        public static float GetAngleBetweenVectors(Vector2 center, Vector2 point)
        {
            var relativeVector = (point - center).normalized;
            return GetAngleFromVector(relativeVector);
        }

        public static float GetAngleFromVector(Vector2 vector)
        {
            var ox = Vector2.right;
            var oy = Vector2.up;
            var ax = Vector2.Angle(vector, ox);
            var ay = Vector2.Angle(vector, oy);
            return ax switch
            {
                >= 0 and <= 90 when ay is >= 0 and <= 90 => ax,
                >= 90 and <= 180 when ay is >= 0 and <= 90 => ax,
                >= 90 and <= 180 when ay is >= 90 and <= 180 => 90 + ay,
                >= 0 and <= 90 when ay is >= 90 and <= 180 => 360 - ax,
                _ => throw new Exception()
            };
        }

        public static Vector2 GetVectorFromAngle(float angle)
        {
            return new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ).normalized;
        }

        public static Vector2 ReduceDimension(Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector3 IncreaseDimension(Vector2 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector2 ConstructRay(Vector2 center, float angle, float radius) =>
            center + GetVectorFromAngle(angle) * radius;
        
        public static IEnumerable<float> FloatRange(float min, float max, float step)
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
        
        public static bool IsAngleInFov(float directionOfViewAngle, float viewAngle, float angle) =>
            Math.Min(
                360 - Math.Abs(directionOfViewAngle - angle),
                Math.Abs(directionOfViewAngle - angle)
            ) <= viewAngle / 2;
        
        public static List<AngleData> ProduceAngles(float directionOfViewAngle, float viewAngle, int density, bool isFovActive)
        {
            var steps = Mathf.RoundToInt(Circle * density);
            var stepSize = Circle / steps;
            return FloatRange(0f, 360f, stepSize)
                .Select(stepAngle => new AngleData(stepAngle, IsAngleInFov(directionOfViewAngle, viewAngle, stepAngle) && isFovActive))
                .ToList();
        }
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
    
    public struct AngleData
    {
        public readonly float Angle;
        public readonly bool IsInFieldOfView;

        public AngleData(float angle, bool isInFieldOfView)
        {
            Angle = angle;
            IsInFieldOfView = isInFieldOfView;
        }
    }
    
    public delegate Vector2 FromGlobalToLocalSpace(Vector2 globalPosition);
}