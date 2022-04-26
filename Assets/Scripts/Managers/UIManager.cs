using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIManager : MonoBehaviour {
    // inspector assigned
    [SerializeField] private float _notificationDuration = 5f;
    [SerializeField] private SharedVariable_String _tutorialScriptText;
    [Header("Mute button")]
    [SerializeField] private Image _muteButton;
    [SerializeField] private Sprite _soundSprite;
    [SerializeField] private Sprite _muteSprite;
    [Header("Health and Armor")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TextMeshProUGUI _healthText;

    [Header("info Text")] 
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _enemiesLeftText;
    [SerializeField] private TextMeshProUGUI _powerUpText;
    [SerializeField] private TextMeshProUGUI _tutorialScript;
    [SerializeField] private TextMeshProUGUI _waveClearedText;

    [Header("Game events")] 
    [SerializeField] private GameEvent _startTutorial;
    [SerializeField] private GameEvent _endTutorial;
    [SerializeField] private GameEvent _onWaveCleared;
    [SerializeField] private GameEvent _onPlayerDied;
    
    // internal
    private Player _player;

    private GameEventListener _startTutorialListener;
    private GameEventListener _endTutorialListener;
    private GameEventListener _onWaveClearedListener;
    private GameEventListener _onPlayerDiedListener;

    private void Awake() {
        // disable unnecessary ui
        _tutorialScript.gameObject.SetActive(false);
        _powerUpText.text = "";
        _waveClearedText.text = "";

        // register to events
        _startTutorialListener = new GameEventListener {GameEvent = _startTutorial};
        _endTutorialListener = new GameEventListener {GameEvent = _endTutorial};
        _onWaveClearedListener = new GameEventListener {GameEvent = _onWaveCleared};
        _onPlayerDiedListener = new GameEventListener() {GameEvent = _onPlayerDied};

        _startTutorialListener.Register(ShowTutorialScript);
        _endTutorialListener.Register(HideTutorialScript);
        _onWaveClearedListener.Register(ShowWaveClearedText);
        _onPlayerDiedListener.Register(ShowEndGameText);
    }
    
    private void OnDestroy() {
        _startTutorialListener.Deregister(ShowTutorialScript);
        _endTutorialListener.Deregister(HideTutorialScript);
        _onWaveClearedListener.Deregister(ShowWaveClearedText);
        _onPlayerDiedListener.Deregister(ShowEndGameText);
    }
    
    // game event callbacks
    private void ShowTutorialScript(GameObject go) {
        _tutorialScript.gameObject.SetActive(true);
    }
    
    private void HideTutorialScript(GameObject go) {
        _tutorialScript.gameObject.SetActive(false);
    }
    private void ShowWaveClearedText(GameObject go) {
        StartCoroutine(ShowWaveClearedTextCou());
    }
    
    private void ShowEndGameText(GameObject go) {
        _waveClearedText.text = "Your ship has collapsed!";
    }

    private IEnumerator ShowWaveClearedTextCou() {
        _waveClearedText.text = "wave cleared!";
        yield return new WaitForSeconds(5f);
        _waveClearedText.text = "";
    }

    private void Start() {
        _player = FindObjectOfType<Player>();
        _powerUpText.text = "";
    }

    private void Update() {
        UpdateHealthSlider();
        _waveText.text = $"wave {SpawnManager.instance.CurrentWave}";
        _tutorialScript.text = _tutorialScriptText.value;

        var numEnemies = SpawnManager.instance.EnemiesLeft;
        Color c = new Color();
        if (numEnemies > 0) ColorUtility.TryParseHtmlString("#D63131", out c);
        else c = Color.white;
        _enemiesLeftText.color = c;
        _enemiesLeftText.text = numEnemies.ToString();
    }

    private void UpdateHealthSlider() {
        var h = _player.Health;
        _healthSlider.value = h / 100f;
        _healthText.text = $"{Mathf.Round(h)}%";
    }

    private IEnumerator SetPowerUpText(String text) {
        _powerUpText.text = text;
        yield return new WaitForSeconds(_notificationDuration);
        _powerUpText.text = "";
    }

    public void PowerUpPickedUp(String powerUp) {
        String txt = "";
        switch (powerUp) {
            case "health":
                txt = "Ship patched up!";
                StartCoroutine(SetPowerUpText(txt));
                break;
            case "nuke":
                txt = "Nuke activated!";
                StartCoroutine(SetPowerUpText(txt));
                break;
            case "shield":
                txt = "Shield activated!";
                StartCoroutine(SetPowerUpText(txt));
                break;
            case "turret":
                txt = "Extra turret!";
                StartCoroutine(SetPowerUpText(txt));
                break;
            case "upgrade":
                txt = "Faster shooting!";
                StartCoroutine(SetPowerUpText(txt));
                break;
        }
    }

    public void ChangeSoundSprite() {
        AudioManager.instance.Mute();
        if (AudioManager.instance.IsMuted)
            _muteButton.sprite = _muteSprite;
        else
            _muteButton.sprite = _soundSprite;
    }
    
}
