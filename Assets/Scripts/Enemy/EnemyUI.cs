using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Enemy _me;
    
    private void Update() {
        _healthSlider.value = _me.Health / _me.MaxHealth;
        if (_me.Health <= 0)
            Destroy(gameObject);
    }
}
