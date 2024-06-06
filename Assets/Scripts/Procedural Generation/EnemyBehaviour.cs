using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float chaseSpeed = 0.5f;
    public float stoppingDistance = 1.0f; // Расстояние, на котором враг останавливается
    
    private Rigidbody2D _enemyBody;
    private SpriteRenderer _enemyRenderer;
    private Transform _player;
    private bool _isInChaseMode;
    private bool _isVisible;

    public void Start()
    {
        _enemyBody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _enemyRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Chase());
        StartCoroutine(Visibility());
    }

    private void FixedUpdate()
    {
        _isVisible = false;
    }
    
    public void FlashEnemyWithLight()
    {
        _isInChaseMode = true;
        _isVisible = true;
    }

    // ReSharper disable once IteratorNeverReturns
    private IEnumerator Chase()
    {
        while (true)
        {
            if (_isInChaseMode)
            {
                var distanceToTarget = Vector2.Distance(transform.position, _player.position);
                if (distanceToTarget > stoppingDistance)
                {
                    Vector2 direction = (_player.position - transform.position).normalized;
                    _enemyBody.velocity = direction * chaseSpeed;
                }
                else
                {
                    _enemyBody.velocity = Vector2.zero;
                }
            }
            yield return null;
        }
    }
    
    // ReSharper disable once IteratorNeverReturns
    private IEnumerator Visibility()
    {
        while (true)
        {
            _enemyRenderer.enabled = _isVisible;
            yield return null;
        }
    }
}