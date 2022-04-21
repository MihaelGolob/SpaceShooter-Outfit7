using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingEnemy : Enemy {
    // Inspector assigned
    [Header("Rotate settings")]
    [SerializeField] [Range(0.1f, 20f)] protected float _rotateSpeed = 15f;
    [SerializeField] [Range(1f, 50f)] protected float _angleMistake = 20f;
    
    // Internal variables
    private float _angleOffset;

    protected override void Start() {
        base.Start();
        _angleOffset = Random.Range(-_angleMistake, _angleMistake);
    }

    protected override void Move() {
        RotateToPlayer();
    }

    protected void RotateToPlayer() {
        var direction = (_player.transform.position - transform.position).normalized;
        direction.z = 0f;
        var angle = Vector3.SignedAngle(transform.forward, direction, Vector3.back);
        angle += _angleOffset;
        var rot = new Vector3(0f, angle, 0f);
        transform.Rotate(Time.deltaTime * _rotateSpeed * rot);
    }
}
