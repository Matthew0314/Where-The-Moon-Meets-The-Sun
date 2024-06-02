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
        
    }

    public void PlayerAttack() {

        Debug.Log("Attack");
        moveGrid.isAttacking = true;
        
        for (int i = 0; i < generateGrid.GetWidth(); i++) {
            for (int j = 0; j < generateGrid.GetLength(); j++) {
                if (playerAttack.canAttack[i,j]) {
                    Debug.Log("Attack at " + i + " " + j);
                }
                
                if (playerAttack.canAttack[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
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

        StartCoroutine(CycleAttackList());



        // for (int i = 0; i < UnitsInRange.Count; i++) {
        //     Debug.Log(UnitsInRange[i].stats.UnitName);
        // }


    }

    public IEnumerator CycleAttackList() {
        bool isAttacking = false;
        int currentIndex = 0;
        AttackingUnit = moveGrid.playerCollide.GetPlayer();

        while(true) {

            Vector3 currentPosition = moveGrid.moveCursor.transform.position;
            moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

            if (Input.GetKeyDown(KeyCode.Space)) {
                DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
                isAttacking = true;
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
                moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());


                yield return new WaitForSeconds(0.3f);
            }
            
           
            // Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

            // // Move the cursor towards the target position using interpolation
            // moveGrid.moveCursor.transform.position = Vector3.Lerp(moveGrid.moveCursor.transform.position, targetPosition, Time.deltaTime * 20000f);
           
            

            yield return null;
        }

        Debug.Log("Broke Free");

        if (isAttacking) {
            //Start Attacking based on primary weapons
            Debug.Log(AttackingUnit.stats.UnitName);
        }

        moveGrid.isAttacking = false;

        yield return null;
    }


}
