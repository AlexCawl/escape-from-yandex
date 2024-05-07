using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    private Vector2 _moveDirection = Vector2.zero;
    
    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        var vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        _moveDirection = new Vector2(horizontal, vertical).normalized;
        
        animator.SetFloat("Horizontal", _moveDirection.x);
        animator.SetFloat("Vertical", _moveDirection.y);
        animator.SetFloat("Speed", _moveDirection.sqrMagnitude);
    }
}
