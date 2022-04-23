using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour {
    // Inspector assigned
    [SerializeField] private float _explosionDamage = 10f;

    private void OnTriggerEnter(Collider other) {
        var dam = other.GetComponent<IDamagable>();
        dam?.TakeDamage(_explosionDamage);
    }
}
