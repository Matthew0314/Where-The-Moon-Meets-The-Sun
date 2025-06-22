using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackerAI : MonoBehaviour, IEnemyAI
{
    
    private FindPath findPath;
    private GenerateGrid generateGrid;
    private PlayerGridMovement playerGridMovement;
    private IMaps _currentMap;
    private ExecuteAction executeAction;

    public bool DidAction { get; set; }
    

    void Start() {
        findPath = GameObject.Find("Player").GetComponent<FindPath>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        executeAction = GameObject.Find("Player").GetComponent<ExecuteAction>();

    }

    public IEnumerator enemyAttack(GameObject enemy) {
        DidAction = false;

        //Get the enemy's UnitManager and its weapon list
        List<Weapon> weaponList = enemy.GetComponent<UnitManager>().stats.weapons;
        UnitManager enemyUnit = enemy.GetComponent<UnitManager>();

        //Sets up lists for Units to attack and units in range
        List<UnitsToAttack> unitAttackList = new List<UnitsToAttack>();
        List<UnitManager> unitsInRange = new List<UnitManager>();;

        // For each weapon, get the enemies movement and attack range and adds it to the list
        for (int k = 0; k < weaponList.Count; k++) {
            findPath.calculateMovement(enemyUnit.XPos, enemyUnit.ZPos, enemyUnit.getMove(), enemyUnit);
            unitsInRange = new List<UnitManager>();
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

            //If there are no units in range, continue to next weapon
            if(unitsInRange.Count == 0) {continue;}

            //For each Unit in the list, 
            for (int l = 0; l < unitsInRange.Count; l++) {
                
                UnitManager tempUnit = unitsInRange[l];

                int enemyDamage = enemyUnit.getCurrentHealth();
                int defDamage = tempUnit.getCurrentHealth();

                enemyUnit.GetPrimaryWeapon().InitiateQueues(enemyUnit, tempUnit, enemyUnit.XPos, enemyUnit.ZPos, tempUnit.XPos, tempUnit.ZPos);
                Queue<UnitManager> AttackingQueue = enemyUnit.GetPrimaryWeapon().AttackingQueue;
                Queue<UnitManager> DefendingQueue = enemyUnit.GetPrimaryWeapon().DefendingQueue;

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

        if (unitAttackList.Count == 0) {
            
            //Sets up lists that will be used later
            List<List<PathTile>> shortestPaths = new List<List<PathTile>>();
            List<UnitManager> tempUnits = _currentMap.GetMapUnits();
            List<PathTile> minPath = new List<PathTile>();
            int movX = 0;
            int movZ = 0;
            int min = 999;
 
    
            int pathCou = 0;
            for (int i = 0; i < generateGrid.GetWidth(); i++)
            {
                for (int j = 0; j < generateGrid.GetLength(); j++)
                {
                    if (generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType == "Player")
                    {
                        shortestPaths.Add(findPath.FindShortestPath(enemyUnit.XPos, enemyUnit.ZPos, i, j));
                        // Debug.Log("AHHHHHHHHHHHHHHHHHHHH" + tempUnits[i].XPos + " " + tempUnits[i].ZPos + " " + min);
                        if (shortestPaths[pathCou].Count < min) {
                            minPath = shortestPaths[pathCou];
                            min = minPath.Count;
                            
                        }  
                        pathCou++;
                    }
                }
            }
            // Debug.Log(movX + " " + movZ + " " + min);

            //Bruteforce way of preventing unit from going on tile with another unit
            int curInd = minPath.Count - 1;
            while (curInd >= 0 && generateGrid.GetGridTile(minPath[curInd].x, minPath[curInd].z).UnitOnTile != null) {
                minPath.Remove(minPath[curInd]);
                curInd--;
            }

            
            yield return StartCoroutine(playerGridMovement.MoveCursor(enemyUnit.XPos, enemyUnit.ZPos, 200f));
            Vector3 currentPosition = enemy.transform.position;
                // enemy.transform.position = new Vector3(generateGrid.GetGridTile(moveX, moveZ).GetXPos(), currentPosition.y, generateGrid.GetGridTile(moveX, moveZ).GetZPos());

            // enemyUnit.primaryWeapon = UnitToAtk.weaponUsed;

            
            Transform objectTrans = playerGridMovement.transform;
            Transform enemyTrans = enemy.transform;
            float step;

            for (int i = 0; i < enemyUnit.getMove(); i++) {
                Vector3 targetPosition = new Vector3(generateGrid.GetGridTile(minPath[i].x, minPath[i].z).GetXPos(), generateGrid.GetGridTile(minPath[i].x, minPath[i].z).GetYPos() + 0.30f, generateGrid.GetGridTile(minPath[i].x, minPath[i].z).GetZPos());
                float speed = 25f; // Speed of movement
                playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(minPath[i].x, minPath[i].z).GetXPos(), generateGrid.GetGridTile(minPath[i].x, minPath[i].z).GetYPos() + 0.30f, generateGrid.GetGridTile(minPath[i].x, minPath[i].z).GetZPos());

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
                movX = minPath[i].x;
                movZ = minPath[i].z;
                yield return null;
            }

            generateGrid.MoveUnit(enemyUnit, enemyUnit.XPos, enemyUnit.ZPos, movX, movZ);
            
        } else {
           
            bool foundSpace = false;
            bool breakFromLoop = false;

            int moveX = 0;
            int moveZ = 0;
            UnitsToAttack UnitToAtk = new UnitsToAttack();

            
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


        

            if (foundSpace) {
                Vector3 currentPosition = enemy.transform.position;
                // enemy.transform.position = new Vector3(generateGrid.GetGridTile(moveX, moveZ).GetXPos(), currentPosition.y, generateGrid.GetGridTile(moveX, moveZ).GetZPos());

                enemyUnit.SetPrimaryWeapon(UnitToAtk.weaponUsed);

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
                
                yield return StartCoroutine(executeAction.ExecuteAttack(enemyUnit, UnitToAtk.unit));
                DidAction = true;       
            }
        }   
    }
}
