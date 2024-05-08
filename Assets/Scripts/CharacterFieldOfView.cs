using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterFieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    private Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    private Camera _camera;
    private Vector3 _direction;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    private void FixedUpdate()
    {
        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        _direction = (mousePosition - transform.position).normalized * viewRadius;
    }

    private void DrawFieldOfView()
    {
        var stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        var stepAngleSize = viewAngle / stepCount;
        var startAngle = VectorUtil.CalculateSightAngle(_direction.normalized);

        for (var i = 0; i < stepCount; i++)
        {
            var angle = startAngle + viewAngle / 2 - stepAngleSize * i;
            var point = VectorUtil.CalculateVectorFromAngle(transform.position, angle) * viewRadius;
            Debug.DrawLine(transform.position, transform.position + point);
        }
    }


    public Tuple<Vector3, Vector3, Vector3> CalculateSightVectors()
    {
        var sightVector = _direction.normalized;
        var angle = VectorUtil.CalculateSightAngle(sightVector);
        var left = VectorUtil.CalculateVectorFromAngle(transform.position, angle + viewAngle / 2);
        var right = VectorUtil.CalculateVectorFromAngle(transform.position, angle - viewAngle / 2);
        return new Tuple<Vector3, Vector3, Vector3>(left * viewRadius, sightVector * viewRadius, right * viewRadius);
    }
}