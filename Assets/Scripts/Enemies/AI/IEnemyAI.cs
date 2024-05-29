using UnityEngine;

public interface IEnemyAI
{
    //interface for how enemies will behave during enemy phase
    //will assign a point system based on how we want the enemies to react
    void enemyAttack(GameObject enemy);
    

}
