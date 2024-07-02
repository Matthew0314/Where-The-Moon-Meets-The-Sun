using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayersToAttack {
    public UnitManager player;
    public Weapon weaponUsed;
    public int score;

    // public PlayersToAttack() {

    // }
}

public class AttackInRangeAI : MonoBehaviour, IEnemyAI
{
    public IEnumerator enemyAttack(GameObject enemy) {
        List<Weapon> weaponList = enemy.GetComponent<UnitManager>().stats.weapons;


        yield return new WaitForSeconds(2);


    }
}
