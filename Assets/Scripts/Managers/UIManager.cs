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
    [SerializeField] private Sprite _soundSprite;
    [SerializeField] private Sprite _muteSprite;
    [Header("Health and Armor")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TextMeshProUGUI _healthText;

    [Header("info Text")] 
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _powerUpText;
    [SerializeField] private GameEvent _onPickUp;
    
    // internal
    private Player _player;
    private GameEventListener _onPickUpListener;

    private void Awake() {
        _onPickUpListener = new GameEventListener();
        _onPickUpListener.GameEvent = _onPickUp;
        //_onPickUpListener.Register(UpdatePickUpText);
    }

    private void OnDestroy() {
        //_onPickUpListener.Deregister(UpdatePickUpText);
    }

    private void Start() {
        _player = FindObjectOfType<Player>();
        _powerUpText.text = "";
    }

    private void Update() {
        UpdateHealthSlider();
        _waveText.text = $"wave {GameManager.instance.Wave}";
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
    }
    
}
