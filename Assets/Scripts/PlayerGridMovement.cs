using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cinemachine;


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
    public bool inMenu;
    public bool isAttacking;
    public bool enemyRangeActive = false;
    private IMaps _currentMap;

    public bool charSelected;
    public CollideWithPlayerUnit playerCollide;
    private GameObject currUnit;
    private int attackRangeStat;

    private TurnManager manageTurn;   

    // private GameObject attackButton;
    // private GameObject itemButton;
    // private GameObject waitButton;

    private CombatMenuManager combatMenu;

    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera combatCam;
    private GameObject playerCurs;

    private ExecuteAction executeAction;

    public bool startGame = false;

    Gamepad gamepad;
    




    void Start()
    {
        playerCurs = GameObject.Find("Player");
        
        gridControl = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        playerCollide = GameObject.Find("PlayerMove").GetComponent<CollideWithPlayerUnit>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        executeAction = GameObject.Find("Player").GetComponent<ExecuteAction>();

        combatMenu = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();

        // attackButton = GameObject.Find("Canvas/AttackButton");
        // itemButton = GameObject.Find("Canvas/WaitButton");
        // waitButton = GameObject.Find("Canvas/ItemButton");  
        
    }


    void Update()
    {
        gamepad = Gamepad.current;

        if(!startGame) {
            return;
        }

        if(inMenu) {
            transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);
            return;
        }
        oneAction = true;
        
        if (isAttacking || manageTurn.IsEnemyTurn()) {
            oneAction = false;
            // return;
        }

        


        if((Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)) && oneAction && gridControl.GetGridTile(x,z).UnitOnTile != null && gridControl.GetGridTile(x,z).UnitOnTile.UnitType == "Enemy" && !pathFinder.selectedEnemies.Contains(gridControl.GetGridTile(x,z).UnitOnTile)) {
            pathFinder.SpecificEnemyRange(gridControl.GetGridTile(x,z).UnitOnTile);
            oneAction = false;
        }

        if((Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)) && oneAction && gridControl.GetGridTile(x,z).UnitOnTile != null && gridControl.GetGridTile(x,z).UnitOnTile.UnitType == "Enemy" && pathFinder.selectedEnemies.Contains(gridControl.GetGridTile(x,z).UnitOnTile)) {
            pathFinder.UnSelectEnemies(gridControl.GetGridTile(x,z).UnitOnTile);
            oneAction = false;
        }

        //Toggles enemy ranges for all enemies
        if ((Input.GetKeyDown(KeyCode.X) || (gamepad != null && gamepad.buttonNorth.wasPressedThisFrame)) && !enemyRangeActive && !inMenu && oneAction && !manageTurn.IsEnemyTurn()) {
            pathFinder.EnemyRange();
            enemyRangeActive = true;
            oneAction = false;
        }
        if ((Input.GetKeyDown(KeyCode.X) || (gamepad != null &&  gamepad.buttonNorth.wasPressedThisFrame)) && enemyRangeActive && !inMenu && oneAction && !manageTurn.IsEnemyTurn()) {
            pathFinder.DestroyEnemyRange();
            enemyRangeActive = false;
            oneAction = false;
        }
     
     
        if ((Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)) && !charSelected && playerCollide.collPlayer && oneAction && manageTurn.IsActive(playerCollide.GetPlayer().stats)) {
            pathFinder.ResetArea();
            currUnit = playerCollide.GetPlayerObject();

            attackRangeStat = playerCollide.GetPlayerAttack();
       
            // pathFinder.CalcAttack(x, z, attackRangeStat , playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
            pathFinder.calculateMovement(x, z, playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
       
            pathFinder.PrintArea();
 
            
            orgX = x;
            orgZ = z;
            charSelected = true;
            oneAction = false;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)) && charSelected && oneAction && !inMenu && !playerCollide.cantPlace)
        {
            
            StartCoroutine(combatMenu.ActivateActionMenu());
            pathFinder.DestroyArea();
            // currUnit = playerCollide.GetPlayerObject();
            // attackRangeStat = playerCollide.GetPlayerAttack();

            // UnitManager temp = playerCollide.GetPlayer();
            

            // executeAction.attackGrid = pathFinder.CalculateAttack(x, z, temp.primaryWeapon.Range, temp.primaryWeapon.Range1, temp.primaryWeapon.Range2, temp.primaryWeapon.Range3);
            // pathFinder.HighlightAttack(executeAction.attackGrid);
            
            curX = x;
            curZ = z;
            MoveUnit(curX, curZ);

            
 
 
            inMenu = true;
            oneAction = false;
        }

        if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)) && oneAction && inMenu) {
            // combatMenu.DeactivateActionMenu();
            // pathFinder.DestroyArea();
            // pathFinder.DestroyRange();
            // pathFinder.ResetArea();
            // MoveUnit(orgX, orgZ);
            // // pathFinder.CalcAttack(orgX, orgZ, attackRangeStat , playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
            // pathFinder.calculateMovement(orgX, orgZ, playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
            // pathFinder.PrintArea();
            // charSelected = true;
            // inMenu = false;
            // oneAction = false;

          
        }
       
        if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)) && oneAction )
        {
            pathFinder.DestroyArea();
            charSelected = false;
            oneAction = false;
            
            if (orgX != x || orgZ != z) {
                playerCollide.removePlayer();
            }  
        }
        
        
        

        
        if (!inMenu && !manageTurn.IsEnemyTurn()) {
            cursorMovement();
        }
        
    }

    //Moves the cursor based on the user input
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
                }
                //If Character is selected, check to see if next tile is whithin the characters range
                else if (gridControl.IsValid(x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z) && charSelected && pathFinder.canMove[x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z])
                {
                    moveCursor.position += new Vector3(Mathf.Sign(Input.GetAxis("Horizontal")) * gridControl.GetCellSize(), 0f, 0f);

                    x += (int)Mathf.Sign(Input.GetAxis("Horizontal"));
                }
            }

            if (Mathf.Abs(Input.GetAxis("Vertical")) >= cursorSen)
            {
                if (gridControl.IsValid(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))) && !gridControl.GetGridTile(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))).GetTallObstacle() && !charSelected)
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(Input.GetAxis("Vertical")) * gridControl.GetCellSize());

                    z += (int)Mathf.Sign(Input.GetAxis("Vertical"));
                }
                else if (gridControl.IsValid(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))) && charSelected && pathFinder.canMove[x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))])
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(Input.GetAxis("Vertical")) * gridControl.GetCellSize());

                    z += (int)Mathf.Sign(Input.GetAxis("Vertical"));
                }

            }
        }
    }

    //Moves a unit from one position to another, teleports and doesnt show movement
    private void MoveUnit(int movX, int movZ) {
        Vector3 currentPosition = currUnit.transform.position;
        currUnit.transform.position = new Vector3(gridControl.GetGridTile(movX, movZ).GetXPos(), currentPosition.y, gridControl.GetGridTile(movX, movZ).GetZPos());
    }


    // Moves cursor to a specific x and z position
    public IEnumerator MoveCursor(int moveCurX, int moveCurZ, float fspeed) {
        Vector3 targetPosition = new Vector3(gridControl.GetGridTile(moveCurX, moveCurZ).GetXPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetYPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetZPos());
        
        moveCursor.position = new Vector3(gridControl.GetGridTile(moveCurX, moveCurZ).GetXPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetYPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetZPos());

        // float fspeed = 40f;

        // // Move the enemy towards the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Calculate the step based on speed and deltaTime
            float step = fspeed * Time.deltaTime;

            // Move the enemy towards the target position gradually
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            yield return null;
        }

        transform.position = targetPosition; 
    }

    public int getX() { return x;}
    public int getZ() { return z;}
    public int GetCurX() { return curX;}
    public int GetCurZ() { return curZ;}
    public int GetOrgX() { return orgX; }
    public int GetOrgZ() { return orgZ; }
    public bool isCharSelected() { return charSelected; }
    public CollideWithPlayerUnit GetPlayerCollide() {
        return playerCollide;
    }

    public bool IsAttacking
    {
        get { return isAttacking; }
        set { isAttacking = value; }
    }

    public void OutOfMenu() {
        combatMenu.DeactivateActionMenu();
        pathFinder.DestroyArea();
        pathFinder.DestroyRange();
        pathFinder.ResetArea();
        MoveUnit(orgX, orgZ);
        // pathFinder.CalcAttack(orgX, orgZ, attackRangeStat , playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
        pathFinder.calculateMovement(orgX, orgZ, playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
        pathFinder.PrintArea();
        charSelected = true;
        inMenu = false;
        oneAction = false;
    }

    
    
    // public void unitWait() {
    //     combatMenu.DeactivateActionMenu();
        
    //     pathFinder.DestroyRange();
        
    //     // gridControl.GetGridTile(curX, curZ).UnitOnTile = playerCollide.GetPlayer();
    //     // gridControl.GetGridTile(orgX, orgZ).UnitOnTile = null;

    //     gridControl.MoveUnit(playerCollide.GetPlayer(), orgX, orgZ, curX, curZ);
    //     Debug.Log("Moved Player from " + orgX + " " + orgZ + " to " + curX + " " + curZ);

    //     UnitManager temp = playerCollide.GetPlayer();
    //     temp.XPos = curX;
    //     temp.ZPos = curZ;

    //     // List<UnitManager> tempUnits = _currentMap.GetMapUnits();

    //     // UnitManager tempUnit = tempUnits.Find()
        

    //     pathFinder.DestroyArea();
    //     charSelected = false;
    //     inMenu = false;       


           

    //     playerCollide.removePlayer();

    //     if (enemyRangeActive) {
    //             pathFinder.DestroyEnemyRange();
    //             pathFinder.EnemyRange();
    //     }
            
    // }

    // public void ResetAfterAction(UnitManager playerUn) {
    //     pathFinder.DestroyRange();
    //     pathFinder.DestroyArea();
    //     if (playerUn.currentHealth > 0) {
    //         gridControl.MoveUnit(playerUn, orgX, orgZ, curX, curZ);
    //     }
        
    //     isAttacking = false;
    //     charSelected = false;
    //     inMenu = false; 
    //     playerCollide.removePlayer();
    // }

//     public IEnumerator CycleAttackList(List<GridTile> UnitsInRange) {
//         // bool isAttacking = false;
//         int currentIndex = 0;
//         UnitManager AttackingUnit = playerCollide.GetPlayer();
//         UnitManager DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
//         int attackerX = curX;
//         int attackerZ = curZ;
//         int defenderX = UnitsInRange[currentIndex].GetGridX();
//         int defenderZ = UnitsInRange[currentIndex].GetGridZ();
//         bool weaponChange = false;

//         int weaponIndex = 0;
//         Weapon orgPrimWeapon = AttackingUnit.primaryWeapon;
//         List<Weapon> playerWeapons = AttackingUnit.stats.weapons;
//         List<GridTile> newEnemies = new List<GridTile>();
    
        

//         combatMenu.DeactivateHoverMenu();
//         CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

//         Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
//         float fspeed = 40f; // Speed of movement
//         moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

//         // // Move the enemy towards the target position
//         while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
//         {
//             // Calculate the step based on speed and deltaTime
//             float step = fspeed * Time.deltaTime;

//             // Move the enemy towards the target position gradually
//             transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

//             yield return null;
//         }

//         transform.position = targetPosition; 

//         while(true) {

//             Vector3 currentPosition = moveCursor.transform.position;
//             moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            
//             if (Input.GetKeyDown(KeyCode.E) && playerWeapons.Count > 1) {
//                 newEnemies = new List<GridTile>();
//                 weaponIndex++;
                
//                 if (weaponIndex >= playerWeapons.Count)
//                 {
//                     weaponIndex = 0;
                    
//                 }

//                 weaponChange = true;

//             }

//             if (Input.GetKeyDown(KeyCode.Q) && playerWeapons.Count > 1) {
//                 newEnemies = new List<GridTile>();
//                 weaponIndex--;
                
//                 if (weaponIndex < 0)
//                 {
//                     weaponIndex = playerWeapons.Count - 1;
                    
//                 }

//                 weaponChange = true;

//             }

//             if (weaponChange) {


//                 bool [,] tempAttack = pathFinder.CalculateAttack(curX, curZ, playerWeapons[weaponIndex].Range, playerWeapons[weaponIndex].Range1, playerWeapons[weaponIndex].Range2, playerWeapons[weaponIndex].Range3);
//                 for (int i = 0; i < gridControl.GetWidth(); i++) {
//                     for (int j = 0; j < gridControl.GetLength(); j++) {
//                         if (tempAttack[i,j] && gridControl.GetGridTile(i,j).UnitOnTile != null && gridControl.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                            
//                             newEnemies.Add(gridControl.GetGridTile(i,j));
                            
//                         }
//                     }
//                 }

//                 if (newEnemies.Count == 0) {
//                     weaponChange = false;
//                     continue;
//                 } else {
//                     pathFinder.DestroyRange();
//                     AttackingUnit.primaryWeapon = playerWeapons[weaponIndex];
//                     UnitsInRange = newEnemies;
//                     attackGrid = tempAttack;
//                     pathFinder.HighlightAttack(attackGrid);
//                     currentIndex = 0;
//                     defenderX = UnitsInRange[currentIndex].GetGridX();
//                     defenderZ = UnitsInRange[currentIndex].GetGridZ();
//                     CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);
//                     weaponChange = false;
                    
                    
                    
//                 }

                
//             }
//             if (Input.GetKeyDown(KeyCode.Space)) {
//                 DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
                
//                 isAttacking = true;
//                 defenderX = UnitsInRange[currentIndex].GetGridX();
//                 defenderZ = UnitsInRange[currentIndex].GetGridZ();
//                 Debug.Log("Hello");
//                 //Go to another IEnumerator to show attacking stats
//                 break;
//             }

//             if (Input.GetKeyDown(KeyCode.B)) {
//                 isAttacking = false;
//                 combatMenu.DeactivateExpectedMenu();
//                 moveCursor.position = new Vector3(gridControl.GetGridTile(curX, curZ).GetXPos(), gridControl.GetGridTile(curX, curZ).GetYPos(), gridControl.GetGridTile(curX, curZ).GetZPos());
//                 AttackingUnit.primaryWeapon = orgPrimWeapon;
//                 combatMenu.ActivateActionMenu();

                
                
//                 inMenu = true;
//                 break;
//             }

//             if (Mathf.Abs(Input.GetAxis("Horizontal")) >= .15) {
//                 float rawHorizontalInput = Input.GetAxis("Horizontal");

//                 // Determine the sign of the input
//                 float horizontalSign = Mathf.Sign(rawHorizontalInput);

//                 // Round down to -1 if negative, round up to 1 if positive
//                 int horizontalInput = (int)Mathf.Ceil(horizontalSign);

//                 // Move through the list based on the horizontal input
//                 if (horizontalInput > 0)
//                 {
//                     // Move up in the list
//                     currentIndex++;
                
//                     if (currentIndex >= UnitsInRange.Count)
//                     {
//                         currentIndex = 0; // Wrap around to the start
                        
//                     }

                    

                    
//                 }
//                 else if (horizontalInput < 0)
//                 {
//                     // Move down in the list
//                     currentIndex--;
//                     if (currentIndex < 0)
//                     {
//                         currentIndex = UnitsInRange.Count - 1; // Wrap around to the end
//                     }

                    
//                 }

//                 Debug.Log("Index Changed");

//                 defenderX = UnitsInRange[currentIndex].GetGridX();
//                 defenderZ = UnitsInRange[currentIndex].GetGridZ();
//                 CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

                
//                 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
                
//                 moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

//                 // // Move the enemy towards the target position
//                 while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
//                 {
//                     // Calculate the step based on speed and deltaTime
//                     float step = fspeed * Time.deltaTime;

//                     // Move the enemy towards the target position gradually
//                     transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

//                     yield return null;
//                 }

//                 transform.position = targetPosition; 
//                 // currentPosition = moveCursor.transform.position;
//                 // moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

                


//                 yield return new WaitForSeconds(0.25f);
//             }

           
            
           
//             // Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
//             // moveCursor.transform.position = Vector3.MoveTowards(moveCursor.transform.position, targetPosition, 30.0f * Time.deltaTime);

//             // Move the cursor towards the target position using interpolation
//             // moveGrid.moveCursor.position = Vector3.Lerp(moveGrid.moveCursor.position, targetPosition, 20.0f * Time.deltaTime);
            
//             // currentPosition = moveGrid.moveCursor.transform.position;
//             // moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            

//             yield return null;
//         }

//         Debug.Log("Broke Free");



//         if (isAttacking) {
//             //Start Attacking based on primary weapons
//             yield return StartCoroutine(ExecuteAttack(AttackingUnit, DefendingEnemy));

//             Debug.Log(DefendingEnemy.primaryWeapon.WeaponName);
//             Debug.Log(AttackingUnit.primaryWeapon.WeaponName);
//             combatMenu.DeactivateExpectedMenu();
//             // AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
//             // AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
//             Debug.Log(AttackingUnit.stats.UnitName);
//             moveCursor.position = new Vector3(gridControl.GetGridTile(attackerX, attackerZ).GetXPos(), gridControl.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, gridControl.GetGridTile(attackerX, attackerZ).GetZPos());
//             UnitManager temp = playerCollide.GetPlayer();
//             temp.XPos = curX;
//             temp.ZPos = curZ;
//             manageTurn.RemovePlayer(AttackingUnit.stats);
//             ResetAfterAction(AttackingUnit);
//             manageTurn.CheckPhase();
//             _currentMap.CheckClearCondition();
//             _currentMap.CheckDefeatCondition();
            

//         }
//         // Debug.Log(gridControl.GetGridTile(5, 4).UnitOnTile.stats.UnitName);

//         isAttacking = false;

//         yield return null;
//     }

//     public IEnumerator ExecuteAttack(UnitManager attackingUnit, UnitManager defendingUnit) {
//         GameObject atkObj = attackingUnit.gameObject;
//         GameObject defObj = defendingUnit.gameObject;
//         Quaternion originalAtkRotation = atkObj.transform.rotation;
//         Quaternion originalDefRotation = defObj.transform.rotation;
//         atkObj.transform.LookAt(defObj.transform);
//         defObj.transform.LookAt(atkObj.transform);
//         SwitchToCombatCamera(attackingUnit.gameObject.transform, defendingUnit.gameObject.transform);
//         // CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
//         // while (brain.ActiveVirtualCamera != combatCam)
//         // {
//         //     yield return null; // Wait until next frame
//         // }

//         GameObject atkCircle = attackingUnit.unitCircle;
//         GameObject defCircle = defendingUnit.unitCircle;
//         atkCircle.SetActive(false);
//         defCircle.SetActive(false);
//         pathFinder.DestroyRange();
//         pathFinder.DestroyArea();
//         // moveCursor.gameObject.SetActive(false);
//         playerCurs.gameObject.GetComponent<MeshRenderer>().enabled = false;
//         yield return new WaitForSeconds(3f);
//         attackingUnit.primaryWeapon.InitiateQueues(attackingUnit, defendingUnit, attackingUnit.XPos, attackingUnit.ZPos, defendingUnit.XPos, defendingUnit.ZPos);
//         Queue<UnitManager> AttackingQueue = attackingUnit.primaryWeapon.AttackingQueue;
//         Queue<UnitManager> DefendingQueue = attackingUnit.primaryWeapon.DefendingQueue;

//         UnitManager playerUnit = null;
//         EnemyUnit enemyUnit = null;

//         if (attackingUnit.stats.UnitType == "Player") {
//             playerUnit = attackingUnit;
//             enemyUnit = (EnemyUnit)defendingUnit;
//         }

//         if (defendingUnit.stats.UnitType == "Player") {
//             playerUnit = defendingUnit;
//             enemyUnit = (EnemyUnit)attackingUnit;
//         }

//         int coun = AttackingQueue.Count;
//         // Debug.Log(" COUNT " + coun);

//         for (int i = 0; i < coun; i++) {
//             UnitManager atk = AttackingQueue.Dequeue();
//             UnitManager def = DefendingQueue.Dequeue();
//             int damage = atk.primaryWeapon.UnitAttack(atk, def, false);
//             def.currentHealth -= damage;
//             // Debug.Log(atk.stats.UnitName + " hits " + def.stats.UnitName + " for " +  damage);
//             if (def.currentHealth <= 0) {
//                 // Debug.Log("Removing " + def.stats.UnitName);
//                 _currentMap.RemoveDeadUnit(def, def.XPos, def.ZPos);
//                 // Debug.Log("Removing " + def.stats.UnitName);
//                 break;
//             }
//             yield return new WaitForSeconds(1f);
//         }
//         // Debug.Log("End Execute Attack");

//         if (playerUnit != null && playerUnit.currentHealth > 0) {
//             int expObtained = 0;

//             expObtained = 30 + ((enemyUnit.stats.Level - playerUnit.stats.Level) * 5);

//             // if (enemyUnit.stats.IsBoss) {

//             // }

//             if (enemyUnit.currentHealth > 0) {
//                 expObtained /= 2;
//             }

//             Debug.Log("AHHHHHH GAINED " + expObtained + " EXP");

//             playerUnit.ExperienceGain(expObtained);
            
//         }
//         Vector3 forward = atkObj.transform.forward; // The forward direction of the attacker
//         Vector3 right = atkObj.transform.right;     // The right direction of the attacker

//         // Offset position: slightly behind and to the right of the attacker
//         Vector3 offsetPosition = atkObj.transform.position - forward * -10f + right * 2f;
//         offsetPosition.y = 4f;

//         combatCam.transform.position = offsetPosition;

//         // Calculate the target rotation by adding 180 degrees to the object's Y rotation
//         Vector3 currentRotation = atkObj.transform.eulerAngles;
//         Quaternion targetRotation = Quaternion.Euler(0f, currentRotation.y + 180f, 0f);

//         // Apply the calculated rotation to the camera
//         combatCam.transform.rotation = targetRotation;

//         // Set LookAt target to maintain focus on the object
//         combatCam.LookAt = atkObj.transform;
//         yield return new WaitForSeconds(3f);
//         atkCircle.SetActive(true);
//         // defCircle.SetActive(true);

//         if (defObj != null) {
//             defCircle.SetActive(true);
//             defObj.transform.rotation = originalDefRotation;
//         }
//         playerCurs.gameObject.GetComponent<MeshRenderer>().enabled = true;
//         atkObj.transform.rotation = originalAtkRotation;
        
//         // moveCursor.gameObject.SetActive(true);
//         SwitchToMainCamera();
//         yield return new WaitForSeconds(3f);


//         yield return null;


//     } 

//     //Calculates the expected attack and prints it out to the menu
//     void CalculateExpectedAttack(UnitManager player, UnitManager enemy, int attackerX, int attackerZ, int defenderX, int defenderZ) {
        
//         player.primaryWeapon.InitiateQueues(player, enemy, attackerX, attackerZ, defenderX, defenderZ);
//         Queue<UnitManager> AttackingQueue = player.primaryWeapon.AttackingQueue;
//         Queue<UnitManager> DefendingQueue = player.primaryWeapon.DefendingQueue;

//         int coun = AttackingQueue.Count;
        
//         int AttackerExpectHealth = player.currentHealth;
      
//         int DefendExpectHealth = enemy.currentHealth;

//         int PDamage = 0;
//         int EDamage = 0;

//         int numPHits = 0;
//         int numEHits = 0;

//         for (int i = 0; i < coun; i++) {
//             UnitManager atk = AttackingQueue.Dequeue();
//             UnitManager def = DefendingQueue.Dequeue();

//             int damage = atk.stats.Attack + atk.primaryWeapon.Attack - def.stats.Defense;

//             float multiplier = 1;

//             if (def.stats.Mounted) {
//                 multiplier += atk.primaryWeapon.MultMounted - 1; 
//             }
//             if (def.stats.AirBorn) {
//                 multiplier += atk.primaryWeapon.MultAirBorn - 1; 
//             }
//             if (def.stats.Armored) {
//                 multiplier += atk.primaryWeapon.MultArmored - 1; 
//             }
//             if (def.stats.Whisper) {
//                 multiplier += atk.primaryWeapon.MultWhisper - 1; 
//             }


//             damage = (int)(damage * multiplier);

//             if (atk.stats.UnitType == "Player") {
//                 DefendExpectHealth -= damage;
//                 PDamage = damage;
//                 numPHits++;
//             } else {
//                 AttackerExpectHealth -= damage;
//                 EDamage = damage;
//                 numEHits++;
//             }
            
//         }

//         combatMenu.SetUpExpectedMenu(player, enemy, AttackerExpectHealth, DefendExpectHealth, PDamage, EDamage, numPHits, numEHits);

        
//     }

//     public IEnumerator MoveCursor(int moveCurX, int moveCurZ) {
//         Vector3 targetPosition = new Vector3(gridControl.GetGridTile(moveCurX, moveCurZ).GetXPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetYPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetZPos());
        
//         moveCursor.position = new Vector3(gridControl.GetGridTile(moveCurX, moveCurZ).GetXPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetYPos(), gridControl.GetGridTile(moveCurX, moveCurZ).GetZPos());

//         float fspeed = 40f;

//         // // Move the enemy towards the target position
//         while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
//         {
//             // Calculate the step based on speed and deltaTime
//             float step = fspeed * Time.deltaTime;

//             // Move the enemy towards the target position gradually
//             transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

//             yield return null;
//         }

//         transform.position = targetPosition; 
//     }

// /*public void SwitchToCombatCamera(Transform attacker, Transform defender)
// {
//     CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
//     brain.enabled = false; // Disable Cinemachine Brain temporarily

//     // Temporarily remove LookAt target to prevent Cinemachine from overriding rotation
//     Transform previousLookAt = combatCam.LookAt;
//     combatCam.LookAt = null;

//     // Ensure that the attacker is on the left side, and the defender is on the right side
//     Transform leftCharacter = attacker.position.x < defender.position.x ? attacker : defender;
//     Transform rightCharacter = leftCharacter == attacker ? defender : attacker;

//     // Get the midpoint between the two characters
//     Transform midpoint = GetMidpoint(leftCharacter, rightCharacter);
//     combatCam.LookAt = midpoint; // Re-enable LookAt after rotation

//     // // Position the camera slightly above the midpoint
//     // float yOffset = 2f; // Adjust the y-offset for camera height
//     // Vector3 cameraPosition = new Vector3(
//     //     (leftCharacter.position.x + rightCharacter.position.x) / 2, // Midpoint of X
//     //     (leftCharacter.position.y + rightCharacter.position.y) / 2 + yOffset, // Midpoint of Y + offset
//     //     (leftCharacter.position.z + rightCharacter.position.z) / 2 - 10f // Keep the same Z position for now
//     // );

//     // // Move the camera to the new position
//     // combatCam.transform.position = cameraPosition;

//     // Calculate the direction to rotate the camera horizontally
//     Vector3 directionToFace = rightCharacter.position - leftCharacter.position;

//     // Calculate the angle between the two characters in degrees
//     float angle = Mathf.Atan2(directionToFace.y, directionToFace.x) * Mathf.Rad2Deg;

//     angle = (angle + 360f) % 360f;



//     Debug.Log("Calculated Angle: " + angle);

//     // Set the camera rotation to only affect the Y-axis (horizontal rotation)
//     // Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);

//     // Explicitly set the camera's global rotation to include the correct Y rotation
//     // combatCam.transform.rotation = targetRotation;

    

//     // Position the camera slightly above the midpoint
//     float yOffset = 5f; // Adjust the y-offset for camera height
//     Vector3 cameraPositionMid = new Vector3(
//         (leftCharacter.position.x + rightCharacter.position.x) / 2, // Midpoint of X
//         (leftCharacter.position.y + rightCharacter.position.y) / 2 + yOffset, // Midpoint of Y + Offset
//         (leftCharacter.position.z + rightCharacter.position.z) / 2 // Midpoint of Z
//     );

//     // Determine the rotation of the camera around the Y-axis
//     float angleInRadians = combatCam.transform.eulerAngles.y * Mathf.Deg2Rad;

//     // Calculate the X and Z offsets based on the angle
//     float offsetDistance = 10f; // Distance from the midpoint
//     float xOffset = Mathf.Sin(angleInRadians) * offsetDistance;
//     float zOffset = Mathf.Cos(angleInRadians) * offsetDistance;

//     // Adjust the camera's position using the offsets
//     Vector3 cameraPosition = new Vector3(
//         cameraPositionMid.x + xOffset,
//         cameraPositionMid.y, // Keep the same Y position
//         cameraPositionMid.z - zOffset
//     );

//     Debug.Log(xOffset + " " + zOffset + " AHHHHHHHHHHHHHHHHHHHHH");

//     // Move the camera to the new position
//     combatCam.transform.position = cameraPosition;

//     Vector3 newRotation = combatCam.transform.eulerAngles;
//     newRotation.y = angle;
//     combatCam.transform.eulerAngles = newRotation;


//     // Optionally, use a smoothing function to smooth the camera's rotation
//     // combatCam.transform.rotation = Quaternion.Slerp(combatCam.transform.rotation, targetRotation, Time.deltaTime * 5f);

//     // Adjust the camera's field of view based on the distance between characters
//     float distance = Vector3.Distance(leftCharacter.position, rightCharacter.position);
//     combatCam.m_Lens.FieldOfView = Mathf.Clamp(distance * 2, 40f, 60f);

//     combatCam.Priority = 1000; // Activate combat camera (higher priority)
    
//     // Re-enable CinemachineBrain
//     brain.enabled = true;
// }
// */
// public void SwitchToCombatCamera(Transform attacker, Transform defender)
// {
//     // Disable Cinemachine Brain temporarily to prevent override
//     CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
//     brain.enabled = false;

//     // Temporarily remove LookAt target to prevent Cinemachine from overriding rotation
//     Transform previousLookAt = combatCam.LookAt;
//     combatCam.LookAt = null;

//     // Ensure that the attacker is on the left side and the defender is on the right side
//     Transform leftCharacter = attacker.position.x < defender.position.x ? attacker : defender;
//     Transform rightCharacter = leftCharacter == attacker ? defender : attacker;

//     // Get the midpoint between the two characters
//     Transform midpoint = GetMidpoint(leftCharacter, rightCharacter);
//     // combatCam.LookAt = midpoint;
//     // combatCam.Follow = midpoint;

//     // Calculate the direction from left character to right character
//     Vector3 directionToFace = rightCharacter.position - leftCharacter.position;

//     // Calculate the angle between the two characters (in 2D plane, using X and Z axis)
//     float angle = Mathf.Atan2(directionToFace.z, directionToFace.x) * Mathf.Rad2Deg;

//     // Normalize the angle to the range [0, 360] degrees
    

//     Debug.Log("Angle " + angle);

//     if (angle == 0f || Mathf.Abs(angle) == 180f || Mathf.Abs(angle) == 270f || Mathf.Abs(angle) == 90) {
//         if (angle == 0f) {
//             if (rightCharacter == attacker) {
//                 angle = 180;
//             }
//         }
//         angle = (angle + 360f) % 360f;
//     } else if (angle > 0) {
//         if (leftCharacter == attacker) {
//             angle = (angle - 90f + 360f) % 360f;
//         } else {
//             angle = (angle + 90f + 360f) % 360f;
//         }
        
//     } else {
//        if (rightCharacter == attacker) {
//             angle = (angle - 90f + 360f) % 360f;
//         } else {
//             angle = (angle + 90f + 360f) % 360f;
//         }
//     }



    
//     // else if (angle > 0f && angle < 180f)
//     // {
//     //     angle = (angle - 90f + 360f) % 360f; // Shift by +90 for 0–180 range
//     // }
//     // else
//     // {
//     //     angle = (angle + 90f) % 360f; // Shift by -90 for 180–360 range
//     // }



//     float characterDistance = Vector3.Distance(leftCharacter.position, rightCharacter.position);

//     // Scale the offset distance based on character distance
//     // For example, offset is proportional to distance (adjust multiplier as needed)
//     float baseOffsetDistance = 7f; // Default offset distance
//     float offsetDistance = baseOffsetDistance + (characterDistance * 0.5f);
//     float angleInRadians = angle * Mathf.Deg2Rad;
//     // float xOffset = 10f;

//     // Calculate the X and Z offsets based on the angle
//     float xOffset = Mathf.Sin(angleInRadians) * offsetDistance;
//     float zOffset = Mathf.Cos(angleInRadians) * offsetDistance;

//     // float totalDistance = Mathf.Sqrt(xOffset * xOffset + zOffset * zOffset); // Calculate diagonal distance
//     // xOffset = (xOffset / totalDistance) * offsetDistance; // Scale the xOffset
//     // zOffset = (zOffset / totalDistance) * offsetDistance; // Scale the zOffset


//     // Debug.Log("Angle " + angle + " xOffset " + xOffset + " zOffset ");

//     // Set the camera's position 10 units away from the midpoint, adjusting X and Z based on angle
//     Vector3 cameraPosition = new Vector3(midpoint.position.x - xOffset, midpoint.position.y, midpoint.position.z - zOffset); // 5f is the y-offset

//     Debug.Log("Angle " + angle + " xOffset " + xOffset +  " zOffset " + zOffset + " midpoint " + midpoint.transform.position + " Camera poisiton " + cameraPosition);

//     // Move the camera to the new position
//     combatCam.transform.position = cameraPosition;

//     // Optionally, use the angle to set the rotation (only affecting Y-axis)
//     Vector3 newRotation = combatCam.transform.eulerAngles;
//     newRotation.y = angle;
//     combatCam.transform.eulerAngles = newRotation;

//     // Adjust the camera's field of view based on the distance between characters
//     float distance = Vector3.Distance(leftCharacter.position, rightCharacter.position);
//     combatCam.m_Lens.FieldOfView = Mathf.Clamp(distance * 2, 40f, 60f);

//     combatCam.Priority = 1000; // Activate combat camera (higher priority)

//     // Re-enable CinemachineBrain
//     brain.enabled = true;
// }


// private Transform GetMidpoint(Transform left, Transform right)
// {
//     Vector3 midpoint = (left.position + right.position) / 2;
//     midpoint.y += 5f;
//     GameObject midpointObject = new GameObject("Midpoint");
//     midpointObject.transform.position = midpoint;
//     return midpointObject.transform;
// }

// public void SwitchToMainCamera()
// {
//     combatCam.Priority = 10; // Deactivate the combat camera and return to the main camera
// }



    

    

    


    
}
