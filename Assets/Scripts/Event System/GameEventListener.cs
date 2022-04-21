using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour {
    [SerializeField] protected GameEvent _gameEvent;
    [SerializeField] protected UnityEvent _unityEvent;

    private void Awake() => _gameEvent.Register(this);
    private void OnDestroy() => _gameEvent.Deregister(this);
    public virtual void RaiseEvent(GameObject go) => _unityEvent.Invoke();
}
