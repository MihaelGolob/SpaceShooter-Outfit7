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
    [Header("Death")] 
    [SerializeField] private Collider _explosionCollider;
    [Header("Sound effects")]
    [SerializeField] private List<AudioClip> _bulletSounds = new List<AudioClip>();
    [SerializeField] [Range(0f, 1f)] private float _bulletVolume = 0.01f;
    [SerializeField] private AudioClip _explosion;
    [SerializeField] [Range(0f, 1f)] private float _explosionVolume = 0.2f;
    [Header("Particle effects")] 
    [SerializeField] private ParticleSystem _explosionParticleSystem;
    [SerializeField] private List<ParticleSystem> _particleSystemsToStop;
    [Header("Powerups")] 
    [SerializeField] [Range(0f, 1f)] private float _dropChance = 0.3f;
    [SerializeField] private List<GameObject> _powerupsToDrop = new List<GameObject>();
    [Header("Game events")] 
    [SerializeField] protected GameEvent _onDiedEvent;
    
    // public properties
    public Transform BulletParent {
        set => _bulletsParent = value;
    }
    
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
        PlayBulletSound();
        
        _shootTimer = Random.Range(_minShootInterval, _maxShootInterval);
    }

    private void PlayBulletSound() {
        int r = Random.Range(0, _bulletSounds.Count);
        AudioManager.instance.PlaySound(_bulletSounds[r], _bulletVolume);
    }


    protected virtual IEnumerator Died() {
        // play sound and particle effect
        AudioManager.instance.PlaySound(_explosion, _explosionVolume);
        _explosionParticleSystem.Play();
        StartCoroutine(ExplosionDamage());
        // clean up
        var mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
        // stop particle effects
        foreach (var ps in _particleSystemsToStop)
            ps.Stop();
        _died = true;
        // invoke event
        _onDiedEvent.Invoke(gameObject);
        // drop power up
        DropPowerup();
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private IEnumerator ExplosionDamage() {
        _explosionCollider.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _explosionCollider.gameObject.SetActive(false);
    }

    private void DropPowerup() {
        if (_powerupsToDrop.Count == 0) return;

        var rand = Random.Range(0f, 1f);
        if (rand >= _dropChance) return;
        
        // drop a random power up
        var r = Random.Range(0, _powerupsToDrop.Count - 1);
        // instantiate
        var pu = Instantiate(_powerupsToDrop[r], _bulletsParent);
        pu.transform.position = transform.position;
    }
    
    // public methods
    public virtual void TakeDamage(float amount) {
        _health = Mathf.Clamp(_health - amount, 0f, 100f);
        
        // check if dead and raise event
        if (_health <= 0.01f && !_died)
            StartCoroutine(Died());
    }

    public bool Dead() {
        return _health <= 0;
    }
}
