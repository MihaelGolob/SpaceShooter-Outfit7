using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_health : Powerup {
    [SerializeField] private float _amount = 60f;
    
    public override void Activate(Player player) {
        player.AddHealth(_amount);
    }

}
