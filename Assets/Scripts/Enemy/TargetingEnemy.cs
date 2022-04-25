using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingEnemy : Enemy {
    // Inspector assigned
    [Header("Rotate settings")]
    [SerializeField] protected bool _tracking = false;
    [SerializeField] [Range(0.1f, 20f)] protected float _rotateSpeed = 15f;
    [SerializeField] [Range(1f, 50f)] protected float _angleMistake = 20f;

    [Header("Movement")] 
    [SerializeField] [Range(0.05f, 1f)] protected float _speed = 1f;
    [SerializeField] protected List<Transform> _routes = new List<Transform>();
    [SerializeField] protected int _numStopPoints;
    
    // public properties
    public void AddRoute(Transform r) => _routes.Add(r);
    
    // Internal variables
    private float _angleOffset;
    private float _moveParameter;
    private int _currRoute;
    private FollowBezierCurve _bc;

    private List<float> _stopPoints;
    private int _stopPointCounter;
    private bool _shooting;

    private Vector3 _oldPlayerPos;

    protected override void Start() {
        base.Start();
        _angleOffset = Random.Range(-_angleMistake, _angleMistake);
        
        // movement
        _bc = GetComponent<FollowBezierCurve>();
        _bc.Initialize(_routes);
        
        _moveParameter = 0;
        _currRoute = Random.Range(0, _routes.Count);
        
        _shooting = false;
        _stopPoints = new List<float>(_numStopPoints);
        // choose random points to stop and shoot
        for (var i = 0; i < _numStopPoints; i++) {
            var rand = Random.Range(0.4f, 0.6f);
            _stopPoints.Add(rand);
        }
    }

    protected override void Update() {
        if (_died) return;
        
        if (_shooting) {
            Shoot();
            RotateTo(_tracking ? _player.transform.position : _oldPlayerPos);
            return;
        }
        
        if (_routes.Count > 0)
            Move();
    }

    protected override void Move() {
        // update parameter
        _moveParameter += Time.deltaTime * _speed;
        if (_moveParameter >= 1f) {
            _stopPointCounter = 0;
            _moveParameter = 0f;
            _currRoute++;
            if (_currRoute >= _routes.Count)
                _currRoute = 0;
        }
        transform.position = _bc.GetPoint(_currRoute, _moveParameter);
        
        // rotate to face velocity direction
        RotateTo(_bc.GetPoint(_currRoute, _moveParameter + Time.deltaTime*_speed));
        
        // check progress
        if (_stopPoints.Count > 0 && _stopPointCounter < _stopPoints.Count && _moveParameter >= _stopPoints[_stopPointCounter]) {
            _shooting = true;
            _shootTimer = 0;
            _stopPointCounter++;
            _oldPlayerPos = _player.transform.position;
            StartCoroutine(StopShooting());
        }
    }

    protected void RotateTo(Vector3 to) {
        var direction = (to - transform.position).normalized;
        direction.z = 0f;
        var angle = Vector3.SignedAngle(transform.forward, direction, Vector3.back);
        angle += _angleOffset;
        var rot = new Vector3(0f, angle, 0f);
        transform.Rotate(Time.deltaTime * _rotateSpeed * rot);
    }

    private IEnumerator StopShooting() {
        yield return new WaitForSeconds(_timeToShoot);
        _shooting = false;
        yield return null;
    }
}
