using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private bool _playerShooter;
    
    // public properties
    public Vector3 Direction {
        get => _direction;
        set => _direction = value.normalized;
    }

    public bool PlayerShooter {
        get => _playerShooter;
        set => _playerShooter = value;
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
        if ((other.CompareTag("Shield") || other.CompareTag("Player")) && _playerShooter) return;
        // bounce the bullet back
        if (other.gameObject.CompareTag("Shield")) {
            _direction = -_direction + new Vector3(Random.Range(0.0f, 0.2f), Random.Range(0.0f, 0.2f));
            return;
        }
        
        if (!(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player")))
            return;

        // do damage
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
