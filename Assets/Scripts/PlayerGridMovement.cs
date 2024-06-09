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

    private TurnManager manageTurn;   

    private GameObject attackButton;
    private GameObject itemButton;
    private GameObject waitButton;

    public bool[,] attackGrid;

    // private ExpectedBattleMenu expectedMenu;

    private CombatMenuManager combatMenu;
    




    void Start()
    {
        //creates an object reference
        gridControl = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        playerCollide = GameObject.Find("PlayerMove").GetComponent<CollideWithPlayerUnit>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();

        combatMenu = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();

        attackButton = GameObject.Find("Canvas/AttackButton");
        itemButton = GameObject.Find("Canvas/WaitButton");
        waitButton = GameObject.Find("Canvas/ItemButton");

        
    }

    // Update is called once per frame
    void Update()
    {

        if(inMenu) {
            transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);
        }
        
        
        if (isAttacking) {
            return;
        }

        oneAction = true;


     
        if (Input.GetKeyDown(KeyCode.Space) && !charSelected && playerCollide.collPlayer && oneAction && manageTurn.IsActive(playerCollide.GetPlayer().stats)) {
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

        if (Input.GetKeyDown(KeyCode.Space) && charSelected && oneAction && !inMenu && !playerCollide.cantPlace)
        {
            
            combatMenu.ActivateActionMenu();
            pathFinder.DestroyArea();
            currUnit = playerCollide.GetPlayerObject();
            attackRangeStat = playerCollide.GetPlayerAttack();

            UnitManager temp = playerCollide.GetPlayer();
            

            attackGrid = pathFinder.CalculateAttack(x, z, temp.primaryWeapon.Range, temp.primaryWeapon.Range1, temp.primaryWeapon.Range2, temp.primaryWeapon.Range3);
            pathFinder.HighlightAttack(attackGrid);
            
            curX = x;
            curZ = z;
            MoveUnit(curX, curZ);
 
 
            inMenu = true;
            oneAction = false;

            
        }

        if (Input.GetKeyDown(KeyCode.B) && oneAction && inMenu) {
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
       
        if (Input.GetKeyDown(KeyCode.B) && oneAction )
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


    public void ActivateFirstMenu() {
        attackButton.SetActive(true);
        waitButton.SetActive(true);
        itemButton.SetActive(true);
        //  EventSystem.current.SetSelectedGameObject(attackButton.gameObject);
    }

    public void DeactivateFirstMenu() {
        attackButton.SetActive(false);
        waitButton.SetActive(false);
        itemButton.SetActive(false);
    }
    
    public void unitWait() {
        combatMenu.DeactivateActionMenu();
        
        pathFinder.DestroyRange();
        
        // gridControl.GetGridTile(curX, curZ).UnitOnTile = playerCollide.GetPlayer();
        // gridControl.GetGridTile(orgX, orgZ).UnitOnTile = null;

        gridControl.MoveUnit(playerCollide.GetPlayer(), orgX, orgZ, curX, curZ);

        UnitManager temp = playerCollide.GetPlayer();
        temp.XPos = curX;
        temp.ZPos = curZ;
        

        pathFinder.DestroyArea();
        charSelected = false;
        inMenu = false;       


           

        playerCollide.removePlayer();
            
    }

    public void ResetAfterAction(UnitManager playerUn) {
        pathFinder.DestroyRange();
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
        bool weaponChange = false;

        int weaponIndex = 0;
        Weapon orgPrimWeapon = AttackingUnit.primaryWeapon;
        List<Weapon> playerWeapons = AttackingUnit.stats.weapons;
        List<GridTile> newEnemies = new List<GridTile>();
    
        

        combatMenu.DeactivateHoverMenu();
        CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

        Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
        float fspeed = 40f; // Speed of movement
        moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

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

        while(true) {

            Vector3 currentPosition = moveCursor.transform.position;
            moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            
            if (Input.GetKeyDown(KeyCode.E) && playerWeapons.Count > 1) {
                newEnemies = new List<GridTile>();
                weaponIndex++;
                
                if (weaponIndex >= playerWeapons.Count)
                {
                    weaponIndex = 0;
                    
                }

                weaponChange = true;

            }

            if (Input.GetKeyDown(KeyCode.Q) && playerWeapons.Count > 1) {
                newEnemies = new List<GridTile>();
                weaponIndex--;
                
                if (weaponIndex < 0)
                {
                    weaponIndex = playerWeapons.Count - 1;
                    
                }

                weaponChange = true;

            }

            if (weaponChange) {


                bool [,] tempAttack = pathFinder.CalculateAttack(curX, curZ, playerWeapons[weaponIndex].Range, playerWeapons[weaponIndex].Range1, playerWeapons[weaponIndex].Range2, playerWeapons[weaponIndex].Range3);
                for (int i = 0; i < gridControl.GetWidth(); i++) {
                    for (int j = 0; j < gridControl.GetLength(); j++) {
                        if (tempAttack[i,j] && gridControl.GetGridTile(i,j).UnitOnTile != null && gridControl.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                            
                            newEnemies.Add(gridControl.GetGridTile(i,j));
                            
                        }
                    }
                }

                if (newEnemies.Count == 0) {
                    weaponChange = false;
                    continue;
                } else {
                    pathFinder.DestroyRange();
                    AttackingUnit.primaryWeapon = playerWeapons[weaponIndex];
                    UnitsInRange = newEnemies;
                    attackGrid = tempAttack;
                    pathFinder.HighlightAttack(attackGrid);
                    currentIndex = 0;
                    defenderX = UnitsInRange[currentIndex].GetGridX();
                    defenderZ = UnitsInRange[currentIndex].GetGridZ();
                    CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);
                    weaponChange = false;
                    
                    
                    
                }

                
            }
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
                combatMenu.DeactivateExpectedMenu();
                moveCursor.position = new Vector3(gridControl.GetGridTile(curX, curZ).GetXPos(), gridControl.GetGridTile(curX, curZ).GetYPos(), gridControl.GetGridTile(curX, curZ).GetZPos());
                AttackingUnit.primaryWeapon = orgPrimWeapon;
                combatMenu.ActivateActionMenu();

                
                
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

                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

                
                targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
                
                moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

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
                // currentPosition = moveCursor.transform.position;
                // moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

                


                yield return new WaitForSeconds(0.25f);
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
            combatMenu.DeactivateExpectedMenu();
            AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            Debug.Log(AttackingUnit.stats.UnitName);
            moveCursor.position = new Vector3(gridControl.GetGridTile(attackerX, attackerZ).GetXPos(), gridControl.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, gridControl.GetGridTile(attackerX, attackerZ).GetZPos());
            UnitManager temp = playerCollide.GetPlayer();
            temp.XPos = curX;
            temp.ZPos = curZ;
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

    //Calculates the expected attack and prints it out to the menu
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

        combatMenu.SetUpExpectedMenu(player, enemy, AttackerExpectHealth, DefendExpectHealth, PDamage, EDamage, numPHits, numEHits);

        
    }

    

    

    public int getX() { return x;}
    public int getZ() { return z;}
    public int GetCurX() { return curX;}
    public int GetCurZ() { return curZ;}
    public bool isCharSelected() { return charSelected; }
    
}
