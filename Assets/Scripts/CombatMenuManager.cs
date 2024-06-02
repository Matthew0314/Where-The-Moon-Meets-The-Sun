using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatMenuManager : MonoBehaviour
{
    private IMaps _currentMap;
    private TurnManager manageTurn;    
    private PlayerGridMovement moveGrid;
    private List<GridTile> UnitsInRange;
    private PlayerAttack playerAttack;
    private GenerateGrid generateGrid;
    private UnitManager DefendingEnemy;
    private UnitManager AttackingUnit;
    public Transform moveCursor;
    private int attackerX;
    private int attackerZ;
    private int defenderX;
    private int defenderZ;

    // Start is called before the first frame update
    void Start()
    {
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        moveGrid = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        UnitsInRange = new List<GridTile>();
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
    }

 


    public void playerWait() {
        
        manageTurn.RemovePlayer(moveGrid.playerCollide.GetPlayer().stats);
        moveGrid.unitWait();
        manageTurn.CheckPhase();
        _currentMap.CheckClearCondition();
        
        
    }

    public void PlayerAttack() {

        Debug.Log("Attack");
        moveGrid.isAttacking = true;
        UnitsInRange = new List<GridTile>();
        
        for (int i = 0; i < generateGrid.GetWidth(); i++) {
            for (int j = 0; j < generateGrid.GetLength(); j++) {
                if (moveGrid.attackGrid[i,j]) {
                    Debug.Log("Attack at " + i + " " + j);
                }
                
                if (moveGrid.attackGrid[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                    Debug.Log("Hit");
                    UnitsInRange.Add(generateGrid.GetGridTile(i,j));
                    Debug.Log("Hit");
                }
            }
        }

        if (UnitsInRange.Count == 0) {
            moveGrid.isAttacking = false;
            return;
        }

        moveGrid.deactivateFirstMenu();

        StartCoroutine(moveGrid.CycleAttackList(UnitsInRange));



        // for (int i = 0; i < UnitsInRange.Count; i++) {
        //     Debug.Log(UnitsInRange[i].stats.UnitName);
        // }


    }

    /*public IEnumerator CycleAttackList() {
        bool isAttacking = false;
        int currentIndex = 0;
        AttackingUnit = moveGrid.playerCollide.GetPlayer();
        attackerX = moveGrid.GetCurX();
        attackerZ = moveGrid.GetCurZ();

        while(true) {

            Vector3 currentPosition = moveGrid.moveCursor.transform.position;
            moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            if (Input.GetKeyDown(KeyCode.Space)) {
                DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
                
                isAttacking = true;
                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                Debug.Log("Hello");
                //Go to another IEnumerator to show attacking stats
                break;
            }

            if (Input.GetKeyDown(KeyCode.B)) {
                Debug.Log("Hello");
                break;
            }

            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= .15) {
                float rawHorizontalInput = Input.GetAxis("Horizontal");

                // Determine the sign of the input
                float horizontalSign = Mathf.Sign(rawHorizontalInput);

                // Round down to -1 if negative, round up to 1 if positive
                int horizontalInput = (int)Mathf.Ceil(horizontalSign);

                // Move through the list based on the horizontal input
                if (horizontalInput > 0)
                {
                    // Move up in the list
                    currentIndex++;
                
                    if (currentIndex >= UnitsInRange.Count)
                    {
                        currentIndex = 0; // Wrap around to the start
                        
                    }
                    
                }
                else if (horizontalInput < 0)
                {
                    // Move down in the list
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = UnitsInRange.Count - 1; // Wrap around to the end
                    }
                    
                
                }

                Debug.Log("Index Changed");

                currentPosition = moveGrid.moveCursor.transform.position;
                moveGrid.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());


                yield return new WaitForSeconds(0.5f);
            }
            
           
            // Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            // moveCursor.transform.position = Vector3.MoveTowards(moveCursor.transform.position, targetPosition, 30.0f * Time.deltaTime);

            // Move the cursor towards the target position using interpolation
            // moveGrid.moveCursor.position = Vector3.Lerp(moveGrid.moveCursor.position, targetPosition, 20.0f * Time.deltaTime);
            
            // currentPosition = moveGrid.moveCursor.transform.position;
            // moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            

            yield return null;
        }

        Debug.Log("Broke Free");

        if (isAttacking) {
            //Start Attacking based on primary weapons
            Debug.Log(DefendingEnemy.primaryWeapon.WeaponName);
            Debug.Log(AttackingUnit.primaryWeapon.WeaponName);
            AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, attackerX, attackerZ, defenderX, defenderZ);
            Debug.Log(AttackingUnit.stats.UnitName);
            moveGrid.moveCursor.position = new Vector3(generateGrid.GetGridTile(attackerX, attackerZ).GetXPos(), generateGrid.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, generateGrid.GetGridTile(attackerX, attackerZ).GetZPos());
            manageTurn.RemovePlayer(AttackingUnit.stats);
            moveGrid.ResetAfterAction(AttackingUnit);
            manageTurn.CheckPhase();
            _currentMap.CheckClearCondition();
        }

        moveGrid.isAttacking = false;

        yield return null;
    }*/

    public void unitAttack(Queue<UnitManager> attacking, Queue<UnitManager> defending) {
        int queueSize = attacking.Count;
        for (int i = 0; i < queueSize; i++) {
            UnitManager atk = attacking.Dequeue();
            UnitManager def = defending.Dequeue();

            int damage = atk.stats.Attack + atk.primaryWeapon.Attack - def.stats.Defense;

            float multiplier = 1;

            if (def.stats.Mounted) {
                multiplier += atk.primaryWeapon.MultMounted - 1; 
            }
            if (def.stats.AirBorn) {
                multiplier += atk.primaryWeapon.MultAirBorn - 1; 
            }
            if (def.stats.Armored) {
                multiplier += atk.primaryWeapon.MultArmored - 1; 
            }
            if (def.stats.Whisper) {
                multiplier += atk.primaryWeapon.MultWhisper - 1; 
            }

            Debug.Log("defender current health " + def.currentHealth + " " + def.stats.Health);

            damage = (int)(damage * multiplier);

            Debug.Log(atk.stats.UnitName + " Did" + damage + " damage to " + def.stats.UnitName);

            def.currentHealth -= damage;

            Debug.Log("defender current health " + def.currentHealth + " " + def.stats.Health);

            if (def.currentHealth <= 0) {
                Debug.Log(def.stats.UnitName + "Has died");
                break;
            }
        }

    }


}
