using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    private Camera _camera;
    private Vector3 _direction;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        _direction = (mousePos - transform.position).normalized * viewRadius;
    }


    public Tuple<Vector3, Vector3, Vector3> CalculateSightVectors()
    {
        var sightVector = _direction.normalized;
        var angle = CalculateSightAngle(sightVector);
        var left = CalculateVectorFromAngle(angle + viewAngle / 2);
        var right = CalculateVectorFromAngle(angle - viewAngle / 2);
        return new Tuple<Vector3, Vector3, Vector3>(left * viewRadius, sightVector * viewRadius, right * viewRadius);
    }

    private float CalculateSightAngle(Vector3 sightVector)
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

    private Vector3 CalculateVectorFromAngle(float angle)
    {
        return new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            transform.position.z
        ).normalized;
    }
}