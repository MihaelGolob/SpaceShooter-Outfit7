using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    // singleton
    private static SpawnManager _instance;
    public static SpawnManager instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<SpawnManager>();
            return _instance;
        }
    }
    
    // Inspector assigned
    [SerializeField] private bool _enabled;
    

}
