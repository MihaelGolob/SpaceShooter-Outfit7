using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave", fileName = "New Wave")]
public class Wave : ScriptableObject {
    public int numEnemy;
    public int numAsteroids;
    public int interval;
}
