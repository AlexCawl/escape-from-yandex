using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Rigidbody2D body;
    private Vector2 _moveDirection = Vector2.zero;
    
    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + _moveDirection * (speed * Time.fixedDeltaTime));
    }
}
