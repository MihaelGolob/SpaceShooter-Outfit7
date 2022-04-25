using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_turret : Powerup {
    [SerializeField] private GameEvent _event;
    
    private void OnEnable() {
        // kind of a quick fix, so the enemies don't drop 
        // more turrets than the player can equip
        var player = FindObjectOfType<Player>();
        if (player.BothTurretsActive)
            Destroy(gameObject);
    }

    public override void Activate(Player player) {
        if (player.BothTurretsActive) {
            Destroy(gameObject);
            return;
        } 
        _event.Invoke();
        player.AddTurret();
    }
}
