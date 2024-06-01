using System;
using UnityEngine;

public class FowUtils
{
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
}