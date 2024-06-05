using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//Purpose of this class is to provide movement capabilities for the cursors on the grid
public class PlayerGridMovement : MonoBehaviour
{
    [SerializeField] int x; //cursor position
    [SerializeField] int z;

    private int curX; //poistion that the player wants to move the unit to
    private int curZ;

    private int orgX; //Position the unit was originally at
    private int orgZ;

    public Transform moveCursor;
    public Transform moveCursorCopy;
    [SerializeField] float speed = 20f;
    [SerializeField] LayerMask obstacleLayer;
    public static float cursorSen = .35f;
    private GenerateGrid gridControl;
    private FindPath pathFinder;
    private bool oneAction;
    private bool inMenu;
    public bool isAttacking;
    private IMaps _currentMap;

    private bool charSelected;
    public CollideWithPlayerUnit playerCollide;
    private GameObject currUnit;
    private int attackRangeStat;
    private PlayerAttack attackPath;
    private TurnManager manageTurn;   

    private GameObject attackButton;
    private GameObject itemButton;
    private GameObject waitButton;

    public bool[,] attackGrid;

    private ExpectedBattleMenu expectedMenu;
    private HoverUnitMenuManager hoverMenu;
    private CombatMenuManager combatMenu;




    void Start()
    {
        //creates an object reference
        gridControl = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        playerCollide = GameObject.Find("PlayerMove").GetComponent<CollideWithPlayerUnit>();
        attackPath = GameObject.Find("Player").GetComponent<PlayerAttack>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        expectedMenu = GameObject.Find("Canvas").GetComponent<ExpectedBattleMenu>();
        hoverMenu = GameObject.Find("Canvas").GetComponent<HoverUnitMenuManager>();
        combatMenu = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();

        attackButton = GameObject.Find("Canvas/AttackButton");
        itemButton = GameObject.Find("Canvas/WaitButton");
        waitButton = GameObject.Find("Canvas/ItemButton");

        deactivateFirstMenu();
    }

    // Update is called once per frame
    void Update()
    {
        

        if(inMenu) {
            transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);
        }
        oneAction = true;
        
        

        if (isAttacking) {
            return;
        }

        // Debug.Log("Update working");

     

        if (Input.GetKeyDown(KeyCode.Space) && !charSelected && playerCollide.collPlayer && oneAction && manageTurn.isActive(playerCollide.GetPlayer().stats)) {
            pathFinder.ResetArea();
            currUnit = playerCollide.GetPlayerObject();
            Debug.Log("Hello");
            attackRangeStat = playerCollide.GetPlayerAttack();
            Debug.Log("No");
            pathFinder.CalcAttack(x, z, attackRangeStat , playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
            Debug.Log("No");
            pathFinder.PrintArea();
            Debug.Log("No");
            
            orgX = x;
            orgZ = z;
            charSelected = true;
            oneAction = false;
        }
        if (Input.GetKeyDown(KeyCode.B) && oneAction && inMenu) {
            deactivateFirstMenu();
            attackPath.DestroyRange();
            MoveUnit(orgX, orgZ);
            pathFinder.CalcAttack(orgX, orgZ, attackRangeStat , playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
            pathFinder.PrintArea();
            charSelected = true;
            inMenu = false;
            oneAction = false;
        }
        // if (Input.GetKeyDown(KeyCode.B) && oneAction && isAttacking)
        // {
        //     attackPath.DestroyRange();
        //     pathFinder.CalcAttack(x, z, attackRangeStat , playerCollide.GetPlayerMove());
        //     pathFinder.PrintArea();
        //     charSelected = true;
        //     isAttacking = false;
        //     oneAction = false;
        //     playerCollide.removePlayer();
        // }
        if (Input.GetKeyDown(KeyCode.B) && oneAction )
        {
            pathFinder.DestroyArea();
            charSelected = false;
            oneAction = false;
            
            if (orgX != x || orgZ != z) {
                playerCollide.removePlayer();
            }
            
          
            
        }
        if (Input.GetKeyDown(KeyCode.Space) && charSelected && oneAction && !inMenu && !playerCollide.cantPlace)
        {
            
            activateFirstMenu();
            pathFinder.DestroyArea();
            currUnit = playerCollide.GetPlayerObject();
            attackRangeStat = playerCollide.GetPlayerAttack();

            UnitManager temp = playerCollide.GetPlayer();
            

            attackGrid = attackPath.CalculateAttack(x, z, temp.primaryWeapon.Range, temp.primaryWeapon.Range1, temp.primaryWeapon.Range2, temp.primaryWeapon.Range3);
            attackPath.HighlightAttack(attackGrid);
            
            curX = x;
            curZ = z;
            MoveUnit(curX, curZ);
 
            Debug.Log("Move Unit");
            inMenu = true;
            oneAction = false;

            
        }
        
        

        if (Input.GetKeyDown(KeyCode.G)) {
            UnitManager temp = playerCollide.GetPlayer();
            Debug.Log("Hello player Mov: " + temp.getMove());
        }
        if (!inMenu && !manageTurn.isEnemyTurn()) {
            cursorMovement();
        }
        
    }

    void cursorMovement()
    {
        //updates "player" position to where "PlayerMove" is on the grid
        transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);


        if (Vector3.Distance(transform.position, moveCursor.position) <= cursorSen)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= cursorSen)
            {
                //Checks if the next tile is not a TallObstacle or if a chracter is not selected
                if (gridControl.IsValid(x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z) && !gridControl.GetGridTile(x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z).GetTallObstacle() && !charSelected)
                {
                    moveCursor.position += new Vector3(Mathf.Sign(Input.GetAxis("Horizontal")) * gridControl.GetCellSize(), 0f, 0f);

                    x += (int)Mathf.Sign(Input.GetAxis("Horizontal"));

                    Debug.Log(x + " " + z);
                }
                //If Character is selected, check to see if next tile is whithin the characters range
                else if (gridControl.IsValid(x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z) && charSelected && pathFinder.canMove[x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z])
                {
                    moveCursor.position += new Vector3(Mathf.Sign(Input.GetAxis("Horizontal")) * gridControl.GetCellSize(), 0f, 0f);

                    x += (int)Mathf.Sign(Input.GetAxis("Horizontal"));

                    Debug.Log(x + " " + z);
                }
            }

            if (Mathf.Abs(Input.GetAxis("Vertical")) >= cursorSen)
            {
                if (gridControl.IsValid(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))) && !gridControl.GetGridTile(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))).GetTallObstacle() && !charSelected)
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(Input.GetAxis("Vertical")) * gridControl.GetCellSize());

                    z += (int)Mathf.Sign(Input.GetAxis("Vertical"));

                    Debug.Log(x + " " + z);
                }
                else if (gridControl.IsValid(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))) && charSelected && pathFinder.canMove[x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))])
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(Input.GetAxis("Vertical")) * gridControl.GetCellSize());

                    z += (int)Mathf.Sign(Input.GetAxis("Vertical"));

                    Debug.Log(x + " " + z);
                }

            }
        }
    }


    private void MoveUnit(int movX, int movZ) {

        Vector3 currentPosition = currUnit.transform.position;
        currUnit.transform.position = new Vector3(gridControl.GetGridTile(movX, movZ).GetXPos(), currentPosition.y, gridControl.GetGridTile(movX, movZ).GetZPos());
    }


    public void activateFirstMenu() {
        attackButton.SetActive(true);
        waitButton.SetActive(true);
        itemButton.SetActive(true);
        //  EventSystem.current.SetSelectedGameObject(attackButton.gameObject);
    }

    public void deactivateFirstMenu() {
        attackButton.SetActive(false);
        waitButton.SetActive(false);
        itemButton.SetActive(false);
    }
    
    public void unitWait() {
        deactivateFirstMenu();
        
        attackPath.DestroyRange();
        
        // gridControl.GetGridTile(curX, curZ).UnitOnTile = playerCollide.GetPlayer();
        // gridControl.GetGridTile(orgX, orgZ).UnitOnTile = null;

        gridControl.MoveUnit(playerCollide.GetPlayer(), orgX, orgZ, curX, curZ);

        
        

        pathFinder.DestroyArea();
        charSelected = false;
        inMenu = false;       


           

        playerCollide.removePlayer();
            
    }

    public void ResetAfterAction(UnitManager playerUn) {
        attackPath.DestroyRange();
        pathFinder.DestroyArea();
        if (playerUn.currentHealth > 0) {
            gridControl.MoveUnit(playerUn, orgX, orgZ, curX, curZ);
        }
        
        isAttacking = false;
        charSelected = false;
        inMenu = false; 
        playerCollide.removePlayer();
    }

    public IEnumerator CycleAttackList(List<GridTile> UnitsInRange) {
        // bool isAttacking = false;
        int currentIndex = 0;
        UnitManager AttackingUnit = playerCollide.GetPlayer();
        UnitManager DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
        int attackerX = curX;
        int attackerZ = curZ;
        int defenderX = UnitsInRange[currentIndex].GetGridX();
        int defenderZ = UnitsInRange[currentIndex].GetGridZ();

        Weapon orgPrimWeapon = AttackingUnit.primaryWeapon;
        List<Weapon> playerWeapons = AttackingUnit.stats.weapons;
        int weaponIndex = 0;
        bool recalc = false;

        hoverMenu.deactivateMenu();
        CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

        while(true) {

            Vector3 currentPosition = moveCursor.transform.position;
            moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            

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
                isAttacking = false;
                expectedMenu.DeactivateMenu();
                moveCursor.position = new Vector3(gridControl.GetGridTile(curX, curZ).GetXPos(), gridControl.GetGridTile(curX, curZ).GetYPos(), gridControl.GetGridTile(curX, curZ).GetZPos());
                AttackingUnit.primaryWeapon = orgPrimWeapon;
                activateFirstMenu();
                Debug.Log("Hello");
                inMenu = true;
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

                

                currentPosition = moveCursor.transform.position;
                moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);


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
            expectedMenu.DeactivateMenu();
            AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            Debug.Log(AttackingUnit.stats.UnitName);
            moveCursor.position = new Vector3(gridControl.GetGridTile(attackerX, attackerZ).GetXPos(), gridControl.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, gridControl.GetGridTile(attackerX, attackerZ).GetZPos());
            manageTurn.RemovePlayer(AttackingUnit.stats);
            ResetAfterAction(AttackingUnit);
            manageTurn.CheckPhase();
            _currentMap.CheckClearCondition();
            _currentMap.CheckDefeatCondition();
            

        }
        // Debug.Log(gridControl.GetGridTile(5, 4).UnitOnTile.stats.UnitName);

        isAttacking = false;

        yield return null;
    }


    void CalculateExpectedAttack(UnitManager player, UnitManager enemy, int attackerX, int attackerZ, int defenderX, int defenderZ) {
        player.primaryWeapon.InitiateQueues(player, enemy, attackerX, attackerZ, defenderX, defenderZ);
        Queue<UnitManager> AttackingQueue = player.primaryWeapon.AttackingQueue;
        Queue<UnitManager> DefendingQueue = player.primaryWeapon.DefendingQueue;

        int coun = AttackingQueue.Count;
        
        int AttackerExpectHealth = player.currentHealth;
      
        int DefendExpectHealth = enemy.currentHealth;

        int PDamage = 0;
        int EDamage = 0;

        int numPHits = 0;
        int numEHits = 0;

        for (int i = 0; i < coun; i++) {
            UnitManager atk = AttackingQueue.Dequeue();
            UnitManager def = DefendingQueue.Dequeue();

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


            damage = (int)(damage * multiplier);

            if (atk.stats.UnitType == "Player") {
                DefendExpectHealth -= damage;
                PDamage = damage;
                numPHits++;
            } else {
                AttackerExpectHealth -= damage;
                EDamage = damage;
                numEHits++;
            }
            
        }

        expectedMenu.SetUpMenu(player, enemy, AttackerExpectHealth, DefendExpectHealth, PDamage, EDamage, numPHits, numEHits);

        
    }

    

    

    public int getX() { return x;}
    public int getZ() { return z;}
    public int GetCurX() { return curX;}
    public int GetCurZ() { return curZ;}
    public bool isCharSelected() { return charSelected; }
    
}
