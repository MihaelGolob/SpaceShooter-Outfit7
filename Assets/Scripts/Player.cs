using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public interface IDamagable {
    public void TakeDamage(float amount);
}

public class Player : MonoBehaviour, IDamagable {
    // Inspector assigned
    [SerializeField] private float _health = 100f;
    [Header("Movement")]
    [SerializeField] [Range(5f, 20f)] private float _movementSpeed = 1.0f;
    [SerializeField] [Range(5f, 20f)] private float _rotationSpeed = 2f;
    [SerializeField] private float _cursorGap = 2f;
    [Header("Shooting")] 
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Transform _bulletsParent;
    [SerializeField] [Range(0.01f, 1f)] private float _coolDownPeriod = 0.4f;

    // internal variables
    private Vector3 _pos;
    private Rigidbody _rb;
    private Camera _cam;
    private CharacterController _controller;
    
    // shooting
    private float _shootTimer;
    
    // movement
    private float _timeMoving;
    
    // input
    private Vector3 _mousePosWorldSpace;
    
    private void Start() {
        _pos = transform.position;
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        _controller = GetComponent<CharacterController>();

        _shootTimer = 0;
    }

    private void Update() {
        // update shoot timer
        _shootTimer -= Time.deltaTime;
        
        GetInput();
        Move();
    }

    private void GetInput() {
        // get mouse position
        Vector3 mousePosPixels = Input.mousePosition;
        mousePosPixels.z = transform.position.z;
        _mousePosWorldSpace = _cam.ScreenToWorldPoint(new Vector3(mousePosPixels.x, mousePosPixels.y, 10.0f));
        
        // shoot input
        if (Input.GetMouseButton(0)) {
            Shoot();
        }
    }

    private void Shoot() {
        if (_shootTimer > 0) return;
        // spawn bullet
        var go = Instantiate(_bulletPrefab, _bulletSpawnPoint);
        go.transform.parent = _bulletsParent;
        // bullet setup
        var bu = go.GetComponent<Bullet>();
        bu.Direction = transform.forward;

        _shootTimer = _coolDownPeriod;
    }

    private void Move() {
        // get direction to mouse pos
        var direction = (_mousePosWorldSpace - transform.position).normalized;
        // move towards the mouse
        transform.position = Vector3.Lerp(transform.position, _mousePosWorldSpace - _cursorGap * direction, Time.deltaTime * _movementSpeed);
        // rotate towards the mouse
        // with euler angles:
        direction.z = 0f;
        var angle = Vector3.SignedAngle(transform.forward, direction, Vector3.back);
        var rot = new Vector3(0f, angle, 0f);
        transform.Rotate(Time.deltaTime * _rotationSpeed * rot, Space.Self);
    }

    public void TakeDamage(float amount) {
        _health = Mathf.Clamp(_health - amount, 0f, 100f);

        // TODO: invoke event and end the game
        if (_health <= 0.001f)
            Debug.Log("Player died");
    }
}
