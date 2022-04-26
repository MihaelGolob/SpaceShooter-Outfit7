using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour {
    [SerializeField] private Transform _followTarget;

    private void Update() {
        transform.position = _followTarget.position;
    }
}
