using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamagable {
    // Serializable
    [SerializeField] [Range(0f, 100f)] protected float _health = 100f;
    [SerializeField] [Range(1, 2)] protected int _size = 1;
    [Header("Shooting")]
    [SerializeField] protected bool _shoots = false;
    [SerializeField] [Range(1f, 10f)] protected float _timeToShoot = 5f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Transform _bulletsParent;
    [SerializeField] [Range(0.3f, 2f)] protected float _minShootInterval = 0.2f;
    [SerializeField] [Range(0.3f, 5f)] protected float _maxShootInterval = 1f;
    [Header("Particle effects")] 
    [SerializeField] private ParticleSystem _explosionParticleSystem;
    [SerializeField] private List<ParticleSystem> _particleSystemsToStop;
    [Header("Game events")] 
    [SerializeField] protected GameEvent _onDiedEvent;
    
    // Internal variables
    protected float _shootTimer;
    protected Player _player;
    protected bool _died;

    // abstract
    protected abstract void Move();
    
    protected virtual void Start() {
        _shootTimer = Random.Range(_minShootInterval, _maxShootInterval);
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    
    protected virtual void Update() {
        if (_died) return;
        
        if (_shoots) {
            // update shoot interval
            Shoot();
        }
        Move();
    }

    protected void Shoot() {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer > 0) return;
        
        // spawn bullet
        var go = Instantiate(_bulletPrefab, _bulletSpawnPoint);
        go.transform.parent = _bulletsParent;
        // bullet setup
        var bu = go.GetComponent<Bullet>();
        bu.Direction = transform.forward;

        _shootTimer = Random.Range(_minShootInterval, _maxShootInterval);
    }

    public virtual void TakeDamage(float amount) {
        _health = Mathf.Clamp(_health - amount, 0f, 100f);
        
        // check if dead and raise event
        if (_health <= 0.01f && !_died)
            StartCoroutine(Died());
    }

    public bool Dead() {
        return _health <= 0;
    }

    protected virtual IEnumerator Died() {
        _explosionParticleSystem.Play();
        // clean up
        var mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
        foreach (var ps in _particleSystemsToStop)
            ps.Stop();

        _died = true;
        // invoke event
        _onDiedEvent.Invoke(gameObject);

        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
