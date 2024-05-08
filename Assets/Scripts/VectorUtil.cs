using System;
using UnityEngine;

public class VectorUtil
{
    public static float CalculateSightAngle(Vector3 sightVector)
    {
        var destination = new Vector2(sightVector.x, sightVector.y).normalized;
        var ox = Vector2.right;
        var oy = Vector2.up;
        var ax = Vector2.Angle(destination, ox);
        var ay = Vector2.Angle(destination, oy);
        return ax switch
        {
            >= 0 and <= 90 when ay is >= 0 and <= 90 => ax,
            >= 90 and <= 180 when ay is >= 0 and <= 90 => ax,
            >= 90 and <= 180 when ay is >= 90 and <= 180 => 90 + ay,
            >= 0 and <= 90 when ay is >= 90 and <= 180 => 360 - ax,
            _ => throw new Exception()
        };
    }

    public static Vector3 CalculateVectorFromAngle(Vector3 position, float angle)
    {
        return new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            position.z
        ).normalized;
    }
}
