using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NonTargetingEnemy : Enemy {
    // Inspector assigned
    [Header("General non targeting enemy properties")]
    [SerializeField] private bool _randomScale = true;
    [SerializeField] [Range(0.01f, 1f)] private float _maxSpeed = 2f;
    [SerializeField] [Range(0f, 0.5f)] private float _directionOffset = 0.4f;
    [SerializeField] [Range(0.01f, 1f)] private float _rotationSpeed = 2f;
    
    // Internal variables
    private Vector3 _movementDirection;
    private float _actualSpeed;
    
    // Random rotating
    private Quaternion _originalRotation;
    private Quaternion _targetRotation;
    private float _rotationProgress;
    
    protected override void Start() {
        base.Start();

        _actualSpeed = Random.Range(0.005f, _maxSpeed);
        var oldPlayerPos = _player.transform.position;
        _movementDirection = (oldPlayerPos - transform.position).normalized;
        
        // calculate the offset vector
        var offset = Vector3.Cross(_movementDirection, Vector3.forward).normalized;
        var randomSign = Random.Range(0f,1f) >= 0.5f ? 1f : -1f;
        _movementDirection += offset * randomSign * _directionOffset;
        
        // scale the 3d model for more diversity
        var randScale = Random.Range(0.4f, 1.5f);
        transform.localScale *= randScale;

        _rotationSpeed += _actualSpeed;
        _originalRotation = transform.rotation;
        _targetRotation = Random.rotation;
    }
    
    protected override void Move() {
        transform.position += _movementDirection * _actualSpeed;
        RandomRotate();
    }

    private void RandomRotate() {
        _rotationProgress += Time.deltaTime * _rotationSpeed;
        if (_rotationProgress > 1.0f) {
            _originalRotation = transform.rotation;
            _targetRotation = Random.rotation;
            _rotationProgress = 0f;
        }
        transform.rotation = Quaternion.Slerp(_originalRotation, _targetRotation, _rotationProgress);
    }
}
