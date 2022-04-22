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
    [Header("Particle effects")] 
    [SerializeField] private ParticleSystem _sparks;

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

        // damage the enemy
        var damageable = other.gameObject.GetComponent<IDamagable>();
        if (damageable.Dead())
            return;
        
        damageable.TakeDamage(_damage);

        // hide the bullet and play effect
        if (_sparks)
            _sparks.Play();
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(DelayDestroy(0.6f));
    }

    private IEnumerator DelayDestroy(float delay) {
        yield return new WaitForSeconds(delay);
        // destroy the bullet
        Destroy(gameObject);
    }
}
