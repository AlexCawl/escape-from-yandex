using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float chaseSpeed = 0.5f;
    public float damageDistance = 1.5f;
    public float stoppingDistance = 1.0f;
    public float damageCooldown = 1.0f;
    public int damage = 5;
    
    private Rigidbody2D _enemyBody;
    private SpriteRenderer _enemyRenderer;
    private Transform _player;
    private bool _isInChaseMode;
    private bool _isVisible;
    private CharacterHealthHolder _playerHealth;
    
    private void Awake()
    {
        _playerHealth = CharacterHealthHolder.GetInstance();
    }

    public void Start()
    {
        _enemyBody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _enemyRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Chase());
        StartCoroutine(Visibility());
        StartCoroutine(Damage());
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

    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
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
    
    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    private IEnumerator Visibility()
    {
        while (true)
        {
            _enemyRenderer.enabled = _isVisible;
            yield return null;
        }
    }

    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    private IEnumerator Damage()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, _player.position) <= damageDistance)
            {
                _playerHealth.Decrease(damage);
            }
            yield return new WaitForSeconds(damageCooldown);
        }
    }
}