using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstronautMovement : MonoBehaviour {
    // inspector assigned
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private float _moveSpeed;

    // internal variables
    private Vector3 _moveTo;
    
    private void Start() {
        _moveTo = _point1.position;
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, _moveTo, Time.deltaTime * _moveSpeed);
        
        if (Vector3.Distance(transform.position, _moveTo) < 0.1f) {
            _moveTo = _moveTo == _point1.position ? _point2.position : _point1.position;
        }
    }
}
