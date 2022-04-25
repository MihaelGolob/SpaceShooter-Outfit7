using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event", fileName = "New Game Event")]
public class GameEvent : ScriptableObject {
    private HashSet<IGameEventListener> _listeners = new HashSet<IGameEventListener>();

    public void Invoke(GameObject go = null) {
        foreach (var l in _listeners)
            l.RaiseEvent(go);
    }

    public void Register(IGameEventListener gme) => _listeners.Add(gme);
    public void Deregister(IGameEventListener gme) => _listeners.Remove(gme);
}
