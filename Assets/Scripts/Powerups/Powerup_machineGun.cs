using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_machineGun : Powerup {
    // Inspector assigned
    [SerializeField] private float _frequencyMultiplier = 2.0f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private GameEvent _event;

    public override void Activate(Player player) {
        _event.Invoke();
        player.MachineGun(_frequencyMultiplier, _duration);
    }
}
