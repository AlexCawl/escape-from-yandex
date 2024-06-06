using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [Range(1, 10)] [SerializeField] private float speed = 10f;
    [Range(1, 10)] [SerializeField] private float lifetime = 5f;

    private Rigidbody2D _rb;
    private int _obstacleLayer;

    private void Start()
    {
        _obstacleLayer = LayerMask.NameToLayer("Obstacle");
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == _obstacleLayer)
            Destroy(gameObject);
    }
}
