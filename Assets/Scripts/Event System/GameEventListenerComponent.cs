using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerComponent : MonoBehaviour, IGameEventListener {
    [SerializeField] private GameEvent _gameEvent;
    [SerializeField] private UnityEvent _unityEvent;
    
    public void Awake() {
        _gameEvent.Register(this);
    }

    public void OnDestroy() {
        _gameEvent.Deregister(this);
    }

    public void RaiseEvent(GameObject go) {
        _unityEvent.Invoke();
    }
}
