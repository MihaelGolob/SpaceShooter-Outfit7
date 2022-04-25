using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_shield : Powerup {
    // Inspector assigned
    [Header("Shield properties")]
    [SerializeField] private int _shieldDuration;
    [SerializeField] private GameEvent _event;
    
    public override void Activate(Player player) {
        _event.Invoke();
        player.ActivateShield(_shieldDuration);
    }
}
