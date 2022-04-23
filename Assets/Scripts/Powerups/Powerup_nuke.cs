using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_nuke : Powerup {
    public override void Activate(Player player) {
        SpawnManager.instance.DestroyAllEnemies();
    }
}
