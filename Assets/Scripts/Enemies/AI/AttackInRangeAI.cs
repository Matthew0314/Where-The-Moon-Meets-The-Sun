using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class AttackInRangeAI : MonoBehaviour, IEnemyAI
{

    private FindPath findPath;
    private GenerateGrid generateGrid;
    private PlayerGridMovement playerGridMovement;
    private ExecuteAction executeAction;
    public bool DidAction { get; set; }
    

    void Start() {
        findPath = GameObject.Find("Player").GetComponent<FindPath>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        executeAction = GameObject.Find("Player").GetComponent<ExecuteAction>();
    }

    public IEnumerator enemyAttack(GameObject enemy) {

        DidAction = false;


        List<Weapon> weaponList = enemy.GetComponent<UnitManager>().stats.weapons;
        UnitManager enemyUnit = enemy.GetComponent<UnitManager>();
        List<UnitsToAttack> unitAttackList = new List<UnitsToAttack>();

        // yield return StartCoroutine(playerGridMovement.MoveCursor(enemyUnit.XPos, enemyUnit.ZPos, 200f));

        for (int k = 0; k < weaponList.Count; k++) {
            findPath.calculateMovement(enemyUnit.XPos, enemyUnit.ZPos, enemyUnit.getMove(), enemyUnit);
            List<UnitManager> unitsInRange = new List<UnitManager>();
            for (int i = 0; i < generateGrid.GetWidth(); i++)
            {
                for (int j = 0; j < generateGrid.GetLength(); j++)
                {
                    if (generateGrid.IsValid(i, j) && findPath.canAttack[i, j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && !generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy"))
                    {
                        unitsInRange.Add(generateGrid.GetGridTile(i,j).UnitOnTile);
                    }
                }
            }

            if(unitsInRange.Count == 0) {continue;}

            for (int l = 0; l < unitsInRange.Count; l++) {
                
                UnitManager tempUnit = unitsInRange[l];

                int enemyDamage = enemyUnit.getCurrentHealth();
                int defDamage = tempUnit.getCurrentHealth();

                enemyUnit.primaryWeapon.InitiateQueues(enemyUnit, tempUnit, enemyUnit.XPos, enemyUnit.ZPos, tempUnit.XPos, tempUnit.ZPos);
                Queue<UnitManager> AttackingQueue = enemyUnit.primaryWeapon.AttackingQueue;
                Queue<UnitManager> DefendingQueue = enemyUnit.primaryWeapon.DefendingQueue;

                int coun = AttackingQueue.Count;

                for (int i = 0; i < coun; i++) {
                    UnitManager atk = AttackingQueue.Dequeue();
                    UnitManager def = DefendingQueue.Dequeue();

                    if (atk.stats.UnitType == "Enemy") {
                        defDamage += weaponList[k].UnitAttack(atk, def, true);
                        
                    } else {
                        enemyDamage += weaponList[k].UnitAttack(atk, def, true);
                    }
                    
                }

                UnitsToAttack atkUnit = new UnitsToAttack(tempUnit, weaponList[k], enemyDamage - defDamage);
                unitAttackList.Add(atkUnit);
                
            }
        }

        bool noUnits = false;
        bool foundSpace = false;
        bool breakFromLoop = false;

        int moveX = 0;
        int moveZ = 0;
        UnitsToAttack UnitToAtk = new UnitsToAttack();

        if (unitAttackList.Count == 0) { Debug.Log("No Units");  noUnits = true;}

        // UnitsToAttack highestScoreUnit = unitAttackList[0];
        // for (int i = 0; i < unitAttackList.Count; i++) {
        //     if (unitAttackList[i].score > highestScoreUnit.score) {
        //         highestScoreUnit = unitAttackList[i];
        //     }
        // }
        if (!noUnits) {
            unitAttackList.Sort((unit1, unit2) => unit1.score.CompareTo(unit2.score));

            
            for (int k = 0; k < unitAttackList.Count; k++) {
                UnitToAtk = unitAttackList[k];
                bool[,] tempGrid = findPath.CalculateAttack(UnitToAtk.unit.XPos, UnitToAtk.unit.ZPos, UnitToAtk.weaponUsed.Range, UnitToAtk.weaponUsed.Range1, UnitToAtk.weaponUsed.Range2, UnitToAtk.weaponUsed.Range3);
                for (int i = 0; i < generateGrid.GetWidth(); i++)
                {
                    for (int j = 0; j < generateGrid.GetLength(); j++)
                    {
                        if(tempGrid[i,j] && findPath.canMove[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile == null) {
                            foundSpace = true;
                            moveX = i;
                            moveZ = j;
                            breakFromLoop = true;
                            break;
                        }
                    }
                    if (breakFromLoop) { break; }
                }
                if (breakFromLoop) { break; }
            }


        }

        if (!noUnits && foundSpace) {
            Vector3 currentPosition = enemy.transform.position;
            // enemy.transform.position = new Vector3(generateGrid.GetGridTile(moveX, moveZ).GetXPos(), currentPosition.y, generateGrid.GetGridTile(moveX, moveZ).GetZPos());

            enemyUnit.primaryWeapon = UnitToAtk.weaponUsed;

            List<PathTile> shortestPath = findPath.FindShortestPath(enemyUnit.XPos, enemyUnit.ZPos, moveX, moveZ);
            Transform objectTrans = playerGridMovement.transform;
            Transform enemyTrans = enemy.transform;
            float step;
            yield return StartCoroutine(playerGridMovement.MoveCursor(enemyUnit.XPos, enemyUnit.ZPos, 200f));
            yield return new WaitForSeconds(0.20f);
            for (int i = 0; i < shortestPath.Count; i++) {
                Vector3 targetPosition = new Vector3(generateGrid.GetGridTile(shortestPath[i].x, shortestPath[i].z).GetXPos(), generateGrid.GetGridTile(shortestPath[i].x, shortestPath[i].z).GetYPos() + 0.30f, generateGrid.GetGridTile(shortestPath[i].x, shortestPath[i].z).GetZPos());
                float speed = 25f; // Speed of movement
                playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(shortestPath[i].x, shortestPath[i].z).GetXPos(), generateGrid.GetGridTile(shortestPath[i].x, shortestPath[i].z).GetYPos() + 0.30f, generateGrid.GetGridTile(shortestPath[i].x, shortestPath[i].z).GetZPos());

                // // Move the enemy towards the target position
                while (Vector3.Distance(objectTrans.position, targetPosition) > 0.01f)
                {
                    // Calculate the step based on speed and deltaTime
                    step = speed * Time.deltaTime;

                    // Move the enemy towards the target position gradually
                    objectTrans.position = Vector3.MoveTowards(objectTrans.position, targetPosition, step);
                    enemyTrans.position = Vector3.MoveTowards(enemyTrans.position, targetPosition, step);

                    yield return null; // Wait for the next frame
                }

                objectTrans.position = targetPosition; 
                enemyTrans.position = targetPosition; 
                yield return null;
            }
            
            generateGrid.MoveUnit(enemyUnit, enemyUnit.XPos, enemyUnit.ZPos, moveX, moveZ);
            // Debug.Log("AHHHHHHHHH " + enemyUnit.stats.UnitName + " Attacks " + UnitToAtk.unit.stats.UnitName);
            // Debug.Log("AHHHHHHHHHH player primary weapon " + UnitToAtk.unit.primaryWeapon.WeaponName);
            
            yield return StartCoroutine(executeAction.ExecuteAttack(enemyUnit, UnitToAtk.unit));
            DidAction = true;
            // Debug.Log("AHHHHHHHHHH End Co Routine");
          
        
        }
        
        // List<PathTile> shortestPath = findPath.FindShortestPath(enemyUnit.XPos, enemyUnit.ZPos, highestScoreUnit.unit.XPos, highestScoreUnit.unit.ZPos);


        //Move the unit through the tiles

        // Debug.Log("ENEMY ATTACKING " + highestScoreUnit.unit.stats.UnitName);


        // Debug.Log("Howdy");
        // yield return new WaitForSeconds(2);
        // yield return null;
        // if (enemyUnit.getCurrentHealth() > 0) {
        //     yield return new WaitForSeconds(1f);
        // } else {
        //     yield return null;
        // }

    }
}
