using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour {
    // Inspector assigned
    [Header("Particle effects")] 
    [SerializeField] private ParticleSystem _pickUpEffect;
    
    // abstract
    public abstract void Activate(Player player);

    // public methods
    

    // private methods
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        // pick up and activate
        StartCoroutine(PickUpInternal(other.GetComponent<Player>()));
    }

    private IEnumerator PickUpInternal(Player player) {
        GetComponent<MeshRenderer>().enabled = false;
        _pickUpEffect.Play();
        yield return new WaitForSeconds(0.5f);
        Activate(player);
    }
}
