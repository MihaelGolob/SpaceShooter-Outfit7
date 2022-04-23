using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IDamagable {
    public void TakeDamage(float amount);
    public bool Dead();
}

public class Player : MonoBehaviour, IDamagable {
    // Inspector assigned
    [SerializeField] [Range(0f, 100f)] private float _health = 100f;
    [Header("Movement")]
    [SerializeField] [Range(5f, 20f)] private float _movementSpeed = 1.0f;
    [SerializeField] [Range(5f, 20f)] private float _rotationSpeed = 2f;
    [SerializeField] private float _cursorGap = 2f;
    [Header("Shooting")] 
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Transform _bulletsParent;
    [SerializeField] [Range(0.01f, 1f)] private float _coolDownPeriod = 0.4f;
    [Header("Extra Turrets")] 
    [SerializeField] private GameObject _leftTurret;
    [SerializeField] private GameObject _rightTurret;
    [SerializeField] private Transform _leftTurretPoint;
    [SerializeField] private Transform _rightTurretPoint;
    [Header("Effects")] 
    [SerializeField] private List<ParticleSystem> _lowHealthSmoke = new List<ParticleSystem>();
    [SerializeField] private List<AudioClip> _bulletFireSounds = new List<AudioClip>();
    [SerializeField] [Range(0f, 1f)] private float _bulletVolume = 0.1f;
    [Header("Power ups")] 
    [SerializeField] private GameObject _shield;

    // internal variables
    private Vector3 _pos;
    private Rigidbody _rb;
    private Camera _cam;
    private CharacterController _controller;

    private int _numOfSmokes;
    private int _activeSmokes;
    
    // shooting
    private float _shootTimer;
    private bool _shieldActivated;
    private bool _leftTurretActivated;
    private bool _rightTurretActivated;
    
    // movement
    private float _timeMoving;
    // input
    private Vector3 _mousePosWorldSpace;
    // public members
    public bool BothTurretsActive => _leftTurretActivated && _rightTurretActivated;

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
        UpdateSmoke();
    }

    private void UpdateSmoke() {
        _numOfSmokes = 0;
        if (_health <= 60) _numOfSmokes++;
        if (_health <= 40) _numOfSmokes++;
        if (_health <= 20) _numOfSmokes++;

        if (_numOfSmokes == _activeSmokes)
            return;

        if (_activeSmokes > _numOfSmokes) {
            for (var i = 0; i < _activeSmokes - _numOfSmokes; i++) {
                var r = Random.Range(0, _lowHealthSmoke.Count);
                if (!_lowHealthSmoke[r].isPlaying) {
                    i--;
                    continue;
                }
                _lowHealthSmoke[r].Stop();
                _activeSmokes--;
            }
            return;
        }
        
        for (var i = 0; i < _numOfSmokes - _activeSmokes; i++) {
            var r = Random.Range(0, _lowHealthSmoke.Count);
            if (_lowHealthSmoke[r].isPlaying) {
                i--;
                continue;
            }
            _lowHealthSmoke[r].Play();
            _activeSmokes++;
        }
    }

    private void GetInput() {
        // get mouse position
        Vector3 mousePosPixels = Input.mousePosition;
        mousePosPixels.z = transform.position.z;
        _mousePosWorldSpace = _cam.ScreenToWorldPoint(new Vector3(mousePosPixels.x, mousePosPixels.y, 10.0f));
        
        // shoot input
        if (Input.GetMouseButton(0) && _shootTimer <= 0) {
            Shoot(_bulletSpawnPoint);
            if (_leftTurretActivated) Shoot(_leftTurretPoint);
            if (_rightTurretActivated) Shoot(_rightTurretPoint);
            _shootTimer = _coolDownPeriod;
        }
    }

    private void Shoot(Transform from) {
        // spawn bullet
        var go = Instantiate(_bulletPrefab, _bulletSpawnPoint);
        go.transform.parent = _bulletsParent;
        go.transform.position = from.position;
        // bullet setup
        var bu = go.GetComponent<Bullet>();
        bu.Direction = transform.forward;
        bu.PlayerShooter = true;
        PlayBulletSound();
    }

    private void PlayBulletSound() {
        var r = Random.Range(0, _bulletFireSounds.Count);
        AudioManager.instance.PlaySound(_bulletFireSounds[r], _bulletVolume);
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

    private IEnumerator ActivateShieldInternal(int amount) {
        _shieldActivated = true;
        _shield.gameObject.SetActive(true);
        yield return new WaitForSeconds(amount);
        _shieldActivated = false;
        _shield.gameObject.SetActive(false);
    } 
    private IEnumerator MachineGunInternal(float mult, float duration) {
        var oldCoolDown = _coolDownPeriod;
        _coolDownPeriod /= mult;
        yield return new WaitForSeconds(duration);
        _coolDownPeriod = oldCoolDown;
    }

    // public methods
    public void TakeDamage(float amount) {
        if (_shieldActivated) return;
        
        _health = Mathf.Clamp(_health - amount, 0f, 100f);

        // TODO: invoke event and end the game
        if (_health <= 0.001f)
            Debug.Log("Player died");
    }

    public bool Dead() {
        return _health <= 0;
    }

    public void ActivateShield(int amount) {
        StartCoroutine(ActivateShieldInternal(amount));
    }

    public void MachineGun(float mult, float duration) {
        StartCoroutine(MachineGunInternal(mult, duration));
    }

    public void AddHealth(float amount) {
        _health += amount;
    }

    public void AddTurret() {
        if (!_leftTurretActivated) {
            _leftTurretActivated = true;
            _leftTurret.SetActive(true);
            return;
        }
        if (!_rightTurretActivated) {
            _rightTurretActivated = true;
            _rightTurret.SetActive(true);
        }
    }
}
