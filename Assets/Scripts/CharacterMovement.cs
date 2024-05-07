using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public Rigidbody2D body;
    private Vector2 _moveDirection = Vector2.zero;
    private GameInput _gameInput = null;

    private void Awake()
    {
        _gameInput = new GameInput();
    }

    private void OnEnable()
    {
        _gameInput.Enable();
        _gameInput.Player.Movement.performed += OnMovementPerformed;
        _gameInput.Player.Movement.canceled += OnMovementCancelled;
    }

    private void OnDisable()
    {
        _gameInput.Disable();
        _gameInput.Player.Movement.performed -= OnMovementPerformed;
        _gameInput.Player.Movement.canceled -= OnMovementCancelled;
    }
    
    private void FixedUpdate()
    {
        body.MovePosition(body.position + _moveDirection * (speed * Time.fixedDeltaTime));
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        _moveDirection = value.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        _moveDirection = Vector2.zero;
    }
}
