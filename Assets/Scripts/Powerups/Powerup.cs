using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Powerup : MonoBehaviour {
    // Inspector assigned
    [SerializeField] private float _rotationSpeed;
    [Header("Particle effects")] 
    [SerializeField] private ParticleSystem _pickUpEffect;
    
    // internal variables
    private bool _hasBeenPickedUp;
    
    // Random rotating
    private Quaternion _originalRotation;
    private Quaternion _targetRotation;
    private float _rotationProgress;
    
    // abstract
    public abstract void Activate(Player player);

    // private methods
    protected void Start() {
        _originalRotation = transform.rotation;
        _targetRotation = Random.rotation;
    }

    protected virtual void Update() {
        RandomRotate();
    }

    private void RandomRotate() {
        _rotationProgress += Time.deltaTime * _rotationSpeed;
        if (_rotationProgress > 1.0f) {
            _rotationProgress = 0;
            _originalRotation = transform.rotation;
            _targetRotation = Random.rotation;
        }
        transform.rotation = Quaternion.Slerp(_originalRotation, _targetRotation, _rotationProgress);
    }

    private void OnTriggerEnter(Collider other) {
        if (_hasBeenPickedUp) return;
        if (!other.CompareTag("Player")) return;
        // pick up and activate
        _hasBeenPickedUp = true;
        StartCoroutine(PickUpInternal(other.GetComponent<Player>()));
    }

    private IEnumerator PickUpInternal(Player player) {
        GetComponent<MeshRenderer>().enabled = false;
        if (_pickUpEffect)
            _pickUpEffect.Play();
        // TODO: set up a pick up effect
        yield return new WaitForSeconds(0.5f);
        Activate(player);
    }
}
