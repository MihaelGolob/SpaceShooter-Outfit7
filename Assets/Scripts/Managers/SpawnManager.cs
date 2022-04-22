using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private List<GameObject> _asteroidPrefabRefs = new List<GameObject>();
    [SerializeField] private List<Transform> _bezierCurves = new List<Transform>();
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private Transform _bulletParent;
    
    // Internal variables
    private List<Enemy> _enemies = new List<Enemy>();

    public void StartNewWave(Wave wave) {
        if (!_enabled) return;
        
        StartCoroutine(SpawnEnemies(wave));
        StartCoroutine(SpawnAsteroids(wave));
    }

    private IEnumerator SpawnEnemies(Wave wave) {
        for (var i = 0; i < wave.numEnemy; i++) {
            // spawn enemy
            var r = Random.Range(0, _enemyPrefabRefs.Count);
            var go = Instantiate(_enemyPrefabRefs[r], transform);
            go.transform.position = _spawnPoints[0].position;
            var en = go.GetComponent<Enemy>();
            en.BulletParent = _bulletParent;
            _enemies.Add(en);
            
            yield return new WaitForSeconds(wave.interval);
        }
    }

    private IEnumerator SpawnAsteroids(Wave wave) {
        for (var i = 0; i < wave.numAsteroids; i++) {
            // spawn asteroid
            var r = Random.Range(0, _enemyPrefabRefs.Count);
            var rr = Random.Range(0, _spawnPoints.Count);
            var go = Instantiate(_asteroidPrefabRefs[r], transform);
            go.transform.position = _spawnPoints[rr].position;
            _enemies.Add(go.GetComponent<Enemy>());

            yield return new WaitForSeconds(wave.interval);
        }
    }
    
    // public methods / callbacks
    public void OnEnemyDied(GameObject enemy) {
        // TODO: somehow call this method to remove enemy
        _enemies.Remove(enemy.GetComponent<Enemy>());
    }
}
