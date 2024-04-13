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

    // Update is called once per frame
    void Update()
    {
        // Gives a value between -1 and 1
        var horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        var vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        _moveDirection = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        body.velocity = _moveDirection * speed;
    }
}
