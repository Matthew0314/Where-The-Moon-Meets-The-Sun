using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveAI : MonoBehaviour, IEnemyAI
{
    public void enemyAttack(GameObject enemy) {
        Debug.Log("Agressive");
    }
}
