using UnityEngine;
using System.Collections;

public struct UnitsToAttack {
    public UnitManager unit;
    public Weapon weaponUsed;
    public int score;

    

    public UnitsToAttack(UnitManager unitToAdd, Weapon weapon, int sco) {
        unit = unitToAdd;
        weaponUsed = weapon;
        score = sco;
    }
}

public interface IEnemyAI
{

    bool DidAction { get; set; }
    //interface for how enemies will behave during enemy phase
    //will assign a point system based on how we want the enemies to react
    IEnumerator enemyAttack(GameObject enemy);
    

}
