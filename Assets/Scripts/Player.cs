using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour {
    // Inspector assigned
    [SerializeField] [Range(5f, 20f)] private float _movementSpeed = 1.0f;
    [SerializeField] [Range(5f, 10f)] private float _rotationSpeed = 2f;

    // internal variables
    private Vector3 _pos;
    private Rigidbody _rb;
    private Camera _cam;
    private CharacterController _controller;
    
    // movement
    private float _timeMoving;
    
    // input
    private Vector3 _mousePosWorldSpace;
    private bool _shoot;
    
    private void Start() {
        _pos = transform.position;
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        _controller = GetComponent<CharacterController>();
    }

    private void Update() {
        GetInput();
        Move();
    }

    private void GetInput() {
        // get mouse position
        Vector3 mousePosPixels = Input.mousePosition;
        mousePosPixels.z = _pos.z;
        _mousePosWorldSpace = _cam.ScreenToWorldPoint(new Vector3(mousePosPixels.x, mousePosPixels.y, 10.0f));
        
        // shoot input
        _shoot = Input.GetMouseButton(0);
    }

    private void Move() {
        // get direction to mouse pos
        var direction = (_mousePosWorldSpace - _pos).normalized;
        // move towards the mouse
        transform.position = Vector3.Lerp(transform.position, _mousePosWorldSpace - direction, Time.deltaTime * _movementSpeed);
        // rotate towards the mouse
        // with euler angles:
        direction.z = 0f;
        float angle = Vector3.Angle(transform.forward, direction);
        var rot = new Vector3(0f, angle, 0f);
        transform.Rotate(Time.deltaTime * _rotationSpeed * rot);
    }
}
