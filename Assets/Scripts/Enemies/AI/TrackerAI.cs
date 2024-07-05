using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerAI : MonoBehaviour, IEnemyAI
{
    
    private FindPath findPath;
    private GenerateGrid generateGrid;
    private PlayerGridMovement playerGridMovement;
    private IMaps _currentMap;
    

    void Start() {
        findPath = GameObject.Find("Player").GetComponent<FindPath>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();

    }

    public IEnumerator enemyAttack(GameObject enemy) {
    //     for (int k = 0; k < weaponList.Count; k++) {
    //         findPath.calculateMovement(enemyUnit.XPos, enemyUnit.ZPos, enemyUnit.getMove(), enemyUnit);
    //         List<UnitManager> unitsInRange = new List<UnitManager>();
    //         for (int i = 0; i < generateGrid.GetWidth(); i++)
    //         {
    //             for (int j = 0; j < generateGrid.GetLength(); j++)
    //             {
    //                 if (generateGrid.IsValid(i, j) && findPath.canAttack[i, j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && !generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy"))
    //                 {
    //                     unitsInRange.Add(generateGrid.GetGridTile(i,j).UnitOnTile);
    //                 }
    //             }
    //         }

    //         if(unitsInRange.Count == 0) {continue;}

    //         for (int l = 0; l < unitsInRange.Count; l++) {
                
    //             UnitManager tempUnit = unitsInRange[l];

    //             int enemyDamage = enemyUnit.currentHealth;
    //             int defDamage = tempUnit.currentHealth;

    //             enemyUnit.primaryWeapon.InitiateQueues(enemyUnit, tempUnit, enemyUnit.XPos, enemyUnit.ZPos, tempUnit.XPos, tempUnit.ZPos);
    //             Queue<UnitManager> AttackingQueue = enemyUnit.primaryWeapon.AttackingQueue;
    //             Queue<UnitManager> DefendingQueue = enemyUnit.primaryWeapon.DefendingQueue;

    //             int coun = AttackingQueue.Count;

    //             for (int i = 0; i < coun; i++) {
    //                 UnitManager atk = AttackingQueue.Dequeue();
    //                 UnitManager def = DefendingQueue.Dequeue();

    //                 if (atk.stats.UnitType == "Enemy") {
    //                     defDamage += weaponList[k].UnitAttack(atk, def, true);
                        
    //                 } else {
    //                     enemyDamage += weaponList[k].UnitAttack(atk, def, true);
    //                 }
                    
    //             }

    //             UnitsToAttack atkUnit = new UnitsToAttack(tempUnit, weaponList[k], enemyDamage - defDamage);
    //             unitAttackList.Add(atkUnit);
                
    //         }
    //     }

    //     if (unitsInRange.Count == 0) {
    //         List<List<PathTile>> shortestPaths = new List<List<PathTile>>();
    //         for (int i = 0; i < _currentMap.mapUnits.Count; i++) {
    //             // List<PathTile> newPath = findPath.FindShortestPath(enemyUnit.XPos, enemyUnit.ZPos, _currentMap.mapUnits[i].XPos, moveZ);
    //         }
            
    //     }
    // }

    yield return null;
    }
}
