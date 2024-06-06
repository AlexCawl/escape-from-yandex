using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float chaseSpeed = 0.5f;
    public float stoppingDistance = 1.0f; // Расстояние, на котором враг останавливается
    private Rigidbody2D rb;
    

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = PlayerTracker.Instance?.PlayerTransform;
    }

    public void FixedUpdate()
    {

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > stoppingDistance)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * chaseSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}