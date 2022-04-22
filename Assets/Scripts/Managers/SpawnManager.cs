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
    [SerializeField] private bool _enabled = false;
    [SerializeField] private List<GameObject> _enemyPrefabRefs = new List<GameObject>();
    [SerializeField] private List<Transform> _bezierCurves = new List<Transform>();
    
    // Internal variables
    private List<Enemy> _enemies;

    public void StartNewWave(Wave wave) {
        // TODO spawn new enemies and assign them paths
    }

}
