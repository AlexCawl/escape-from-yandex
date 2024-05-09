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
    public LayerMask obstacleMask;

    private Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    private Camera _camera;
    private float _angle;

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
        var mouse = FowUtils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
        var character = FowUtils.ReduceDimension(transform.position);
        _angle = FowUtils.GetAngleBetweenVectors(character, mouse);
    }

    private void DrawFieldOfView()
    {
        var characterPosition = FowUtils.ReduceDimension(transform.position);
        var stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        var stepAngleSize = viewAngle / stepCount;
    
        for (var i = 0; i < stepCount; i++)
        {
            var angle = _angle + viewAngle / 2 - stepAngleSize * i;
            var ray = FowUtils.ConstructRay(characterPosition, angle, viewRadius);
            Debug.DrawLine(transform.position, FowUtils.IncreaseDimension(ray, transform.position.z));
        }
    }

    private RayCast LightWithRay(Vector2 center, float angle)
    {
        var dir = FowUtils.GetVectorFromAngle(angle);
        var hit = Physics2D.Raycast(center, dir, viewRadius, obstacleMask.value);
        return new RayCast(hit.point, hit.distance);
    }


    public Tuple<Vector3, Vector3, Vector3> DebugSightVectors()
    {
        var height = transform.position.z;
        var sight = FowUtils.IncreaseDimension(FowUtils.GetVectorFromAngle(_angle) * viewRadius, height);
        var left = FowUtils.IncreaseDimension(FowUtils.GetVectorFromAngle(_angle + viewAngle / 2) * viewRadius, height);
        var right = FowUtils.IncreaseDimension(FowUtils.GetVectorFromAngle(_angle - viewAngle / 2) * viewRadius, height);
        return new Tuple<Vector3, Vector3, Vector3>(left, sight, right);
    }
}

internal struct RayCast {
    public Vector2 point;
    public float distance;

    public RayCast(Vector2 _point, float _distance) {
        point = _point;
        distance = _distance;
    }
}