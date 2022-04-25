using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_health : Powerup {
    [SerializeField] private float _amount = 60f;
    [SerializeField] private GameEvent _event;
    
    public override void Activate(Player player) {
        _event.Invoke();
        player.AddHealth(_amount);
    }

}
