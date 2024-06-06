using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float chaseSpeed = 0.5f;
    public float stoppingDistance = 1.0f; // Расстояние, на котором враг останавливается
    
    private Rigidbody2D _rb;
    private Transform _target;
    private int _health = 3;
    private int _bulletLayer;

    public void Start()
    {
        _bulletLayer = LayerMask.NameToLayer("Bullet");
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == _bulletLayer)
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
            Destroy(gameObject);
    }

    public void Die()
    {
        // Логика смерти врага
        Debug.Log(gameObject.name + " died!");
    }
}