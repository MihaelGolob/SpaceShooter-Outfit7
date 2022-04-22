using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_MachineGun : Powerup {
    // Inspector assigned
    [SerializeField] private float _frequencyMultiplier = 2.0f;
    [SerializeField] private float _duration = 5f;


    public override void Activate(Player player) {
        player.MachineGun(_frequencyMultiplier, _duration);
    }
}
