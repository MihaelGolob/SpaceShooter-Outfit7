using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private SharedVariable_String _tutorialScript;
    [SerializeField] private bool _tutorial;
    [Header("Game events")] 
    [SerializeField] private GameEvent _waveCleared;
    [SerializeField] private GameEvent _startTutorial;
    [SerializeField] private GameEvent _endTutorial;

    // Internal variables
    private bool _tutorialCompleted;
    private int _score;
    private bool _paused;

    private GameEventListener _waveClearedListener;

    private Player _player;
    
    // public members
    public bool Paused => _paused;

    private void Awake() {
        // dont destroy this game object
        DontDestroyOnLoad(gameObject);
        if (instance != this)
            Destroy(gameObject);
        // subscribe to events
        SceneManager.sceneLoaded += OnSceneLoaded;
        _waveClearedListener = new GameEventListener();
        _waveClearedListener.GameEvent = _waveCleared;
        _waveClearedListener.Register(WaveCleared);

        if (!_tutorial)
            _tutorialCompleted = true;
    }

    private void OnDestroy() {
        // unsubscribe from all events
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _waveClearedListener.Deregister(WaveCleared);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
        if (!scene.name.Equals("Game")) return;
        if (_player == null)
            _player = FindObjectOfType<Player>();

        if (!_tutorialCompleted) {
            StartCoroutine(StartTutorial());
        }
        else {
            StartCoroutine(StartNewWave()); 
        }
    }

    private void Start() {
        _player = FindObjectOfType<Player>();
        
        //_tutorial = PlayerPrefs.GetInt("TutorialCompleted", 0) > 0;
    }

    private IEnumerator StartNewWave() {
        yield return new WaitForSeconds(3);
        SpawnManager.instance.StartNewWave();
    }

    private IEnumerator StartTutorial() {
        _tutorialScript.value = "Move the ship by dragging your finger.";
        _startTutorial.Invoke();
        yield return new WaitForSeconds(5f);
        
        _tutorialScript.value = "The ship automatically shoots while you hold down your finger.";
        yield return new WaitForSeconds(5f);
        
        _tutorialScript.value = "Your objective is to destroy all enemy ships.";
        SpawnManager.instance.SpawnTutorialWave();
        yield return new WaitForSeconds(5f);
        
        _tutorialScript.value = "Also watch out for asteroids.";
        yield return new WaitForSeconds(20f);
        _endTutorial.Invoke();
    }

    private void WaveCleared(GameObject go) {
        if (!_tutorialCompleted) {
            _player.AddHealth(100);
            _tutorialCompleted = true;
            // save for further sessions
            //PlayerPrefs.SetInt("TutorialCompleted", 1);
            //PlayerPrefs.Save();
        }
        StartCoroutine(StartNewWave());
    }

    // public methods
    public void PauseGame() {
        Time.timeScale = 0f;
        _paused = true;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        _paused = false;
    }

    public void ToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
