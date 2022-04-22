using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAxis : MonoBehaviour {
    // Inspector assigned
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 axis;

    private void Update() {
        transform.RotateAround(transform.position, axis, _speed * Time.deltaTime);
    }
}
