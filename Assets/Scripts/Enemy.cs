using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable {
    // Serializable
    [SerializeField] [Range(1f, 10f)] protected float _health = 100f;
    [SerializeField] [Range(1, 2)] protected int _size = 1;
    [Header("Shooting")]
    [SerializeField] protected bool _shoots = false;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Transform _bulletsParent;
    [SerializeField] [Range(0.3f, 2f)] protected float _minShootInterval = 0.2f;
    [SerializeField] [Range(0.3f, 5f)] protected float _maxShootInterval = 1f;
    [Header("Game events")] 
    [SerializeField] protected GameEvent _onDiedEvent;
    
    // Internal variables
    private float _shootTimer;
    protected Player _player;

    protected virtual void Start() {
        _shootTimer = Random.Range(_minShootInterval, _maxShootInterval);
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    
    protected void Update() {
        if (_shoots) {
            // update shoot interval
            _shootTimer -= Time.deltaTime;
            Shoot();
        }
        Move();
    }

    protected void Shoot() {
        if (_shootTimer > 0) return;
        
        // spawn bullet
        var go = Instantiate(_bulletPrefab, _bulletSpawnPoint);
        go.transform.parent = _bulletsParent;
        // bullet setup
        var bu = go.GetComponent<Bullet>();
        bu.Direction = transform.forward;

        _shootTimer = Random.Range(_minShootInterval, _maxShootInterval);
    }
        
    protected virtual void Move() {
        
    }

    public virtual void TakeDamage(float amount) {
        _health = Mathf.Clamp(_health - amount, 0f, 100f);
        
        // check if dead and raise event
        if (_health <= 0.01f)
            _onDiedEvent.Invoke(gameObject);
    }
}
