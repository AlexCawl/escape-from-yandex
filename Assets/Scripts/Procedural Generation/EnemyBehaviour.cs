using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float chaseSpeed = 0.5f;
    public float stoppingDistance = 1.0f; // Расстояние, на котором враг останавливается
    
    private Rigidbody2D _rb;
    private Transform _target;

    public void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void FixedUpdate()
    {

        var distanceToTarget = Vector2.Distance(transform.position, _target.position);
        if (distanceToTarget > stoppingDistance)
        {
            Vector2 direction = (_target.position - transform.position).normalized;
            _rb.velocity = direction * chaseSpeed;
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }
}