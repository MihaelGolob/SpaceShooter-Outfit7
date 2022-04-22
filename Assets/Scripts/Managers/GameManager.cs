using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Wave {
    private int waveNumber;
    private int numEnemy;
    private int numMeteors;
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
    
}
