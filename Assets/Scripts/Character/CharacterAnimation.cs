using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    private GameInput _gameInput;
    private Vector2 _moveDirection = Vector2.zero;

    private const string _horizontal = "Horizontal";
    private const string _vertical = "Vertical";
    private const string _speed = "Speed";
    private const string _lastHorizontal = "LastHorizontal";
    private const string _lastVertical = "LastVertical";
    
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
        animator.SetFloat(_horizontal, _moveDirection.x);
        animator.SetFloat(_vertical, _moveDirection.y);
        animator.SetFloat(_speed, _moveDirection.sqrMagnitude);

        if (_moveDirection != Vector2.zero)
        {
            animator.SetFloat(_lastHorizontal, _moveDirection.x);
            animator.SetFloat(_lastVertical, _moveDirection.y);
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        _moveDirection = value.ReadValue<Vector2>().normalized;
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        _moveDirection = Vector2.zero;
    }
}
