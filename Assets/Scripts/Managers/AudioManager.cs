using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPoolItem {
    public GameObject GameObject;
    public Transform Transform;
    public AudioSource AudioSource;
    public bool Playing;
    public IEnumerator Coroutine;
    public ulong ID;
}

public class AudioManager : MonoBehaviour {
    // singleton
    private static AudioManager _instance;

    public static AudioManager instance {
        get {
            if (_instance == null)
                _instance = FindObjectOfType<AudioManager>();
            return _instance;
        }
    }

    public bool IsMuted => _mute;

    // inspector assigned
    [SerializeField] private int _maxSounds = 20;
    [SerializeField] private AudioSource _backgroundMusic;

    // internal variables
    private List<AudioPoolItem> _pool = new List<AudioPoolItem>();
    private Dictionary<ulong, AudioPoolItem> _activePool = new Dictionary<ulong, AudioPoolItem>();
    private static ulong _idGiver = 0;
    private bool _mute;
    
    // private methods
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        if (instance != this)
            Destroy(gameObject);

        // Generate pool
        for (var i = 0; i < _maxSounds; i++) {
            // create game object and add audio source
            var go = new GameObject("Pool item");
            var aus = go.AddComponent<AudioSource>();
            go.transform.parent = transform;

            // create and configure pool item
            var pi = new AudioPoolItem();
            pi.GameObject = go;
            pi.AudioSource = aus;
            pi.Transform = go.transform;
            pi.Playing = false;
            go.SetActive(false);
            _pool.Add(pi);
        }
    }

    private void ConfigurePoolObject(int poolIndex, AudioClip clip, float volume) {
        if (poolIndex < 0 || poolIndex >= _pool.Count) return;

        var pi = _pool[poolIndex];
        // configure audio source
        var so = pi.AudioSource;
        so.clip = clip;
        so.volume = volume;
        
        // enable game object and start playing
        pi.Playing = true;
        pi.GameObject.SetActive(true);
        so.Play();
        _idGiver++;
        pi.ID = _idGiver;
        pi.Coroutine = StopSoundDelayed(pi.ID, so.clip.length);
        StartCoroutine(pi.Coroutine);
        
        // add sound to active pool
        _activePool[pi.ID] = pi;
    }

    private IEnumerator StopSoundDelayed(ulong id, float clipLength) {
        yield return new WaitForSeconds(clipLength + 0.4f);

        AudioPoolItem activeSound;
        if (_activePool.TryGetValue(id, out activeSound)) {
            activeSound.AudioSource.Stop();
            activeSound.AudioSource.clip = null;
            activeSound.GameObject.SetActive(false);
            _activePool.Remove(id);

            activeSound.Playing = false;
        }
    }

    // public methods
    public void PlaySound(AudioClip clip, float volume) {
        if (clip == null || volume.Equals(0.0f)) return;

        // find an available audio source to use
        for (var i = 0; i < _pool.Count; i++) {
            var pi = _pool[i];
            if (pi.Playing) continue;
            
            ConfigurePoolObject(i, clip, _mute ? 0f : volume);
            break;
        }
    }

    public void Mute() {
        if (_mute) {
            _mute = false;
            foreach (var po in _pool) {
                po.AudioSource.mute = false;
                _backgroundMusic.mute = false;
            }
        }
        else {
            _mute = true;
            foreach (var po in _pool) {
                po.AudioSource.mute = true;
                _backgroundMusic.mute = true;
            }
        }
    }
}
