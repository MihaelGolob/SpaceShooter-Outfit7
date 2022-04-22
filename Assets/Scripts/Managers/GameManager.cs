using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Wave {
    public Wave(int waveNumber, int numEnemy, int numAsteroids, int interval) {
        this.waveNumber = waveNumber;
        this.numEnemy = numEnemy;
        this.numAsteroids = numAsteroids;
        this.interval = interval;
    }
    
    public int waveNumber;
    public int numEnemy;
    public int numAsteroids;
    public int interval;
}

public class GameManager : MonoBehaviour { 
    // singleton
    private static GameManager _instance;
    public static GameManager instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }
    
    // Inspector assigned
    
    // Internal variables
    private bool _tutorialCompleted;
    private int _currWave;
    private int _score;


    private void Start() {
        var wave = new Wave(0, 3, 5, 10);
        SpawnManager.instance.StartNewWave(wave);
    }
}
