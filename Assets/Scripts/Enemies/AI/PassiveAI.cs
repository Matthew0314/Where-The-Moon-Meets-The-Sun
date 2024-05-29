using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAI : MonoBehaviour, IEnemyAI
{
    // Start is called before the first frame update
    public void enemyAttack(GameObject enemy) {
        Debug.Log("Passive");
    }
}
