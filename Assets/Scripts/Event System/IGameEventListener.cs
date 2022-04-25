 using System;
using UnityEngine;
using UnityEngine.Events;
 using Object = UnityEngine.Object;

 public interface IGameEventListener {
    
    public void RaiseEvent(GameObject go);
}
