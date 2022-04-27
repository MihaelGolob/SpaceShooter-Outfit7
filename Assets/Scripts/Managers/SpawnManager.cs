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
    
    // public properties
    public int EnemiesLeft => _enemiesToDestroy;
    public int CurrentWave => _currWave;
    
    // Inspector assigned
    [SerializeField] private bool _enabled = false;
    [Header("Waves")]
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private Wave _tutorialWave;
    [Header("Prefabs")]
    [SerializeField] private List<GameObject> _enemyPrefabRefs = new List<GameObject>();
    [SerializeField] private GameObject _asteroidPrefabRef;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private Transform _bulletParent;
    [Header("Game events")] 
    [SerializeField] private GameEvent _onEnemyDied;
    [SerializeField] private GameEvent _onWaveCleared;
    
    // Internal variables
    private List<Enemy> _enemies = new List<Enemy>();
    private List<Enemy> _asteroids = new List<Enemy>();

    private GameEventListener _onEnemyDiedListener;

    private bool _waveStarted;
    private int _enemiesToDestroy;

    private int _currWave;
    
    private void Awake() {
        _onEnemyDiedListener = new GameEventListener {GameEvent = _onEnemyDied};
        _onEnemyDiedListener.Register(OnEnemyDied);
    }

    private void Update() {
        if (!_waveStarted) return;
        if (_enemies.Count != 0 || _enemiesToDestroy != 0) return;
        
        _currWave++;
        _onWaveCleared.Invoke();
        _waveStarted = false;
    }

    private void OnDestroy() {
        _onEnemyDiedListener.Deregister(OnEnemyDied);
    }

    private void StartNewWaveInternal(Wave wave) {
        if (!_enabled) return;
        
        _waveStarted = true;
        _enemiesToDestroy = wave.numEnemy;
        StartCoroutine(SpawnEnemies(wave));
        StartCoroutine(SpawnAsteroids(wave));
    }

    private Wave GenerateWave() {
        if (_currWave - 1 < _waves.Count)
            return _waves[_currWave - 1];
        
        // we need to ensure that there are infinite waves
        double intFunc = 1 / (0.015 * (_currWave + 10));
        double enemyFunc = Mathf.Sqrt(5 * _currWave);
        double asterFunc = Mathf.Sqrt(4 * _currWave);
        // logic for spawning enemies
        Wave wave = ScriptableObject.CreateInstance<Wave>();
        wave.numAsteroids = 10 + (int) asterFunc;
        wave.numEnemy = 5 + (int) enemyFunc;
        wave.interval = 5 + (int) intFunc;

        return wave;
    }

    private IEnumerator SpawnEnemies(Wave wave) {
        for (var i = 0; i < wave.numEnemy; i++) {
            // spawn enemy
            var r = Random.Range(0, _enemyPrefabRefs.Count);
            var go = Instantiate(_enemyPrefabRefs[r], transform);
            go.transform.position = _spawnPoints[0].position;
            var en = go.GetComponentInChildren<Enemy>();
            en.BulletParent = _bulletParent;
            _enemies.Add(en);

            yield return new WaitForSeconds(wave.interval);
        }
    }

    private IEnumerator SpawnAsteroids(Wave wave) {
        for (var i = 0; i < wave.numAsteroids; i++) {
            // spawn asteroid
            var rr = Random.Range(0, _spawnPoints.Count);
            var go = Instantiate(_asteroidPrefabRef, transform);
            go.transform.position = _spawnPoints[rr].position;
            _asteroids.Add(go.GetComponentInChildren<Enemy>());

            yield return new WaitForSeconds(wave.interval);
        }
    }
    
    // public methods / callbacks
    public void SpawnTutorialWave() {
        StartNewWaveInternal(_tutorialWave);
    }

    public void StartNewWave() {
        if (_currWave == 0) _currWave = 1;
        Wave wave = GenerateWave();
        StartNewWaveInternal(wave);
    }
    
    public void OnEnemyDied(GameObject enemy) {
        // remove from enemies if there, otherwise in _asteroids
        if (_enemies.Remove(enemy.GetComponent<Enemy>()))
            _enemiesToDestroy--;
        _asteroids.Remove(enemy.GetComponent<Enemy>());
        //Debug.Log($"Removed {enemy.name} from list");
    }

    public void DestroyAllEnemies() {
        for (int i = 0; i < _enemies.Count; i++) {
            _enemies[i].TakeDamage(10000f);
            // because the enemy game object will be destroyed
            // it will be removed from the list..
            i--;
        }
        for (int i = 0; i < _asteroids.Count; i++) {
            _asteroids[i].TakeDamage(10000f);
            // because the enemy game object will be destroyed
            // it will be removed from the list..
            i--;
        }
    }
}
