using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class Bullet : MonoBehaviour {
    // Serializable
    [SerializeField] private float _damage;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _timeToLive = 10f;

    // Internal variables
    private Vector3 _direction;
    private float _timeAlive;
    
    // public properties
    public Vector3 Direction {
        get => _direction;
        set => _direction = value.normalized;
    }

    private void Update() {
        _timeAlive += Time.deltaTime;
        if (_timeAlive > _timeToLive)
            Destroy(gameObject);
        
        Move();
    }

    private void Move() {
        transform.position += _direction * _speed;
    }

    private void OnTriggerEnter(Collider other) {
        if (!(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player")))
            return;

        Debug.Log("i've hit " + other.gameObject.name);
        
        // damage the enemy
        var damageable = other.gameObject.GetComponent<IDamagable>();
        damageable.TakeDamage(_damage);
        
        // destroy the bullet
        Destroy(gameObject);
    }
}
