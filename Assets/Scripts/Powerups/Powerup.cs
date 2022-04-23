using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class Powerup : MonoBehaviour {
    // Inspector assigned
    [SerializeField] private float _rotationSpeed = 0.14f;
    [Header("Effects")] 
    [SerializeField] private ParticleSystem _pickUpParticleEffect;
    [FormerlySerializedAs("_pickUpSoundEffect")] 
    [SerializeField] private AudioClip _activateSound;
    [FormerlySerializedAs("_pickUpSoundVolume")] 
    [SerializeField] [Range(0f, 1f)] private float _activateSoundVolume = 0.1f;
    
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
        // disable meshes
        GetComponent<MeshRenderer>().enabled = false;
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        
        // play particle effect
        if (_pickUpParticleEffect)
            _pickUpParticleEffect.Play();
        // TODO: set up a pick up effect
        //yield return new WaitForSeconds(0.5f);
        // play sound
        if (_activateSound)
            AudioManager.instance.PlaySound(_activateSound, _activateSoundVolume);
        // activate powerup
        Activate(player);

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
