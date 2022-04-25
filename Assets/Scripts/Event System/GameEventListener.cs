using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public delegate void RaiseEvent(GameObject gameObject);

public class GameEventListener : IGameEventListener {
    // internal
    private event RaiseEvent _raiseMethod;
    // public property
    public GameEvent GameEvent { get; set; }

    public void Register(RaiseEvent e) {
        if (GameEvent == null) throw new Exception("game event is null");
        GameEvent.Register(this);
        _raiseMethod += e;
    }

    public void Deregister(RaiseEvent e) {
        if (GameEvent == null) throw new Exception("game event is null");
        GameEvent.Deregister(this);
        _raiseMethod -= e;
    }

    public void RaiseEvent(GameObject go) {
        if (_raiseMethod == null) throw new Exception("delegate is null");
        _raiseMethod(go);
    }
}
