using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_shield : Powerup {
    // Inspector assigned
    [Header("Shield properties")]
    [SerializeField] private int _shieldDuration;
    
    public override void Activate(Player player) {
        player.ActivateShield(_shieldDuration);
    }
}
