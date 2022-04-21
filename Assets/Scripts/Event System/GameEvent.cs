using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event", fileName = "New Game Event")]
public class GameEvent : ScriptableObject {
    [SerializeField] private HashSet<GameEventListener> _listeners = new HashSet<GameEventListener>();

    public void Invoke(GameObject go = null) {
        foreach (var l in _listeners)
            l.RaiseEvent(go);
    }

    public void Register(GameEventListener gme) => _listeners.Add(gme);
    public void Deregister(GameEventListener gme) => _listeners.Remove(gme);
}
