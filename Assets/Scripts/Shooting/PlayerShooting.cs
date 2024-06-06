using System;
using FieldOfView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject flashPrefab;
    [SerializeField] private Transform firingPoint;
    [Range(0.1f, 2f)][SerializeField] private float fireRate = 1f;
    
    
    private float _nextTimeToShoot = 0f;
    private GameInput _gameInput;
    private Vector2 _angle;
    private float _shootTimer;

    private void Awake()
    {
        _gameInput = new GameInput();
    }

    private void OnEnable()
    {
        _gameInput.Enable();
        _gameInput.Player.Fire.performed += Shoot;
    }

    private void OnDisable()
    {
        _gameInput.Disable();
        _gameInput.Player.Fire.performed -= Shoot;
    }

    private void Shoot(InputAction.CallbackContext value)
    {
        if (Time.time >= _nextTimeToShoot)
        {
            _nextTimeToShoot = Time.time + fireRate;
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0; // Обнуляем координату Z

            Vector3 direction = (mousePosition - firingPoint.position).normalized;

            GameObject bullet = Instantiate(flashPrefab, firingPoint.position, Quaternion.identity);
            bullet.transform.up = direction; // Направляем пулю в сторону мыши
            
            Debug.Log("Shot fired at: " + Time.time);
        }
        else
        {
            Debug.Log("Cannot fire yet. Next shot available at: " + _nextTimeToShoot);
        }
       
        
    }
    
}
