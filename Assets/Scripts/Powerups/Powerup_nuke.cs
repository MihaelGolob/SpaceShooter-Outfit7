using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_nuke : Powerup {
    [SerializeField] private GameEvent _event;
    
    public override void Activate(Player player) {
        _event.Invoke();
        SpawnManager.instance.DestroyAllEnemies();
    }
}
