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

    [SerializeField] float cursorY = 2.2f;

    public Transform moveCursor;
    [SerializeField] float speed = 20f;
    public static float cursorSen = .35f;
    private GenerateGrid gridControl;
    private FindPath pathFinder;
    public bool inMenu;
    public bool isAttacking;
    public bool enemyRangeActive = false;

    public bool charSelected;
    // public CollideWithPlayerUnit playerCollide;
    private GameObject currUnit;

    private TurnManager manageTurn;   
    private CombatMenuManager combatMenu;


    public bool startGame = false;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    void Start()
    {
        gridControl = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        combatMenu = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();

        playerInput = GetComponent<PlayerInput>();

        
    }


    void Update()
    {
        // moveCursor.position = new Vector3(moveCursor.position.x, cursorY, moveCursor.position.z);

        if(!startGame) return;
        // transform.position = new Vector3(transform.position.x, cursorY, transform.position.z);
        if(inMenu) {
            transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);
            return;
        }

        bool oneAction = true;
        
        if (isAttacking || manageTurn.IsEnemyTurn()) {}

        else if(playerInput.actions["Select"].WasPressedThisFrame() && gridControl.GetGridTile(x,z).UnitOnTile != null && gridControl.GetGridTile(x,z).UnitOnTile.UnitType == "Enemy" && !pathFinder.selectedEnemies.Contains(gridControl.GetGridTile(x,z).UnitOnTile)) {

            pathFinder.SpecificEnemyRange(gridControl.GetGridTile(x,z).UnitOnTile);
        }

        else if(playerInput.actions["Select"].WasPressedThisFrame() && gridControl.GetGridTile(x,z).UnitOnTile != null && gridControl.GetGridTile(x,z).UnitOnTile.UnitType == "Enemy" && pathFinder.selectedEnemies.Contains(gridControl.GetGridTile(x,z).UnitOnTile)) {

            pathFinder.UnSelectEnemies(gridControl.GetGridTile(x,z).UnitOnTile);
        }

        //Toggles enemy ranges for all enemies
        else if (playerInput.actions["ShowRange"].WasPressedThisFrame() && !enemyRangeActive && !inMenu && !manageTurn.IsEnemyTurn()) {
            pathFinder.EnemyRange();
            enemyRangeActive = true;
        }

        else if (playerInput.actions["ShowRange"].WasPressedThisFrame() && enemyRangeActive && !inMenu && !manageTurn.IsEnemyTurn()) {
            pathFinder.DestroyEnemyRange();
            enemyRangeActive = false;
        }
     
     
        // else if (playerInput.actions["Select"].WasPressedThisFrame() && !charSelected && playerCollide.collPlayer && manageTurn.IsActive(playerCollide.GetPlayer().stats)) {
        else if (playerInput.actions["Select"].WasPressedThisFrame() && !charSelected && gridControl.GetGridTile(x, z).UnitOnTile?.UnitType == "Player" && manageTurn.IsActive((gridControl.GetGridTile(x, z).UnitOnTile as PlayerUnit)?.stats)) {
            pathFinder.ResetArea();
            // currUnit = playerCollide.GetPlayerObject();
            currUnit = gridControl.GetGridTile(x, z).UnitOnTile.gameObject;
       
            // pathFinder.calculateMovement(x, z, playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
            pathFinder.calculateMovement(x, z, gridControl.GetGridTile(x, z).UnitOnTile.getMove(), gridControl.GetGridTile(x, z).UnitOnTile as PlayerUnit);
       
            pathFinder.PrintArea();
 
            
            orgX = x;
            orgZ = z;
            charSelected = true;
        }

        // else if (playerInput.actions["Select"].WasPressedThisFrame() && charSelected && !inMenu && !playerCollide.cantPlace)
        else if (playerInput.actions["Select"].WasPressedThisFrame() && charSelected && !inMenu && CanPlace())
        {
            // gridControl.GetGridTile(x, z).UnitOnTile = gridControl.GetGridTile(orgX, orgZ).UnitOnTile;
            // gridControl.GetGridTile(orgX, orgZ).UnitOnTile = null;

            UnitManager temp = gridControl.GetGridTile(orgX, orgZ).UnitOnTile;
            gridControl.GetGridTile(orgX, orgZ).UnitOnTile = null;
            gridControl.GetGridTile(x, z).UnitOnTile = temp;

            StartCoroutine(combatMenu.ActivateActionMenu());
            pathFinder.DestroyArea();
            
            curX = x;
            curZ = z;
            MoveUnit(curX, curZ);

            inMenu = true;
        }
       
        else if (playerInput.actions["Back"].WasPressedThisFrame())
        {
            pathFinder.DestroyArea();
            charSelected = false;
            
            // if (orgX != x || orgZ != z) {
            //     playerCollide.removePlayer();
            // }  
        }

        // if (playerInput.actions["Back"].WasPressedThisFrame() && )
        
        
        

        
        if (!inMenu && !manageTurn.IsEnemyTurn()) {
            MoveCursor();
        }
        
    }
    private bool CanPlace() => gridControl.GetGridTile(x, z).UnitOnTile == null || gridControl.GetGridTile(x, z).UnitOnTile == currUnit.GetComponent<UnitManager>();
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    private void MoveCursor() {
        transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, moveCursor.position) <= cursorSen) {
            if (Mathf.Abs(moveInput.x) >= cursorSen) {

                int newX = x + (int)Mathf.Sign(moveInput.x);

                if (gridControl.IsValid(newX, z) && !gridControl.GetGridTile(newX, z).GetTallObstacle() && !charSelected) {
                    moveCursor.position += new Vector3(Mathf.Sign(moveInput.x) * gridControl.GetCellSize(), 0f, 0f);
                    x = newX;
                }
                else if (gridControl.IsValid(newX, z) && charSelected && pathFinder.canMove[newX, z]) {
                    moveCursor.position += new Vector3(Mathf.Sign(moveInput.x) * gridControl.GetCellSize(), 0f, 0f);
                    x = newX;
                }
                Debug.Log(x + " " + z);
            }

            if (Mathf.Abs(moveInput.y) >= cursorSen) {
                int newZ = z + (int)Mathf.Sign(moveInput.y);
                if (gridControl.IsValid(x, newZ) && !gridControl.GetGridTile(x, newZ).GetTallObstacle() && !charSelected) {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(moveInput.y) * gridControl.GetCellSize());
                    z = newZ;
                }
                else if (gridControl.IsValid(x, newZ) && charSelected && pathFinder.canMove[x, newZ]) {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(moveInput.y) * gridControl.GetCellSize());
                    z = newZ;
                }
                Debug.Log(x + " " + z);
            }

            if (gridControl.GetGridTile(x,z).UnitOnTile != null && startGame) combatMenu.ActivateHoverMenu(gridControl.GetGridTile(x,z).UnitOnTile);
            else combatMenu.DeactivateHoverMenu();
        }
    }

    //Moves a unit from one position to another, teleports and doesnt show movement
    private void MoveUnit(int movX, int movZ) {
        Vector3 currentPosition = currUnit.transform.position;
        currUnit.transform.position = new Vector3(gridControl.GetGridTile(movX, movZ).GetXPos(), currentPosition.y, gridControl.GetGridTile(movX, movZ).GetZPos());
    }


    // Moves cursor to a specific x and z position
    public IEnumerator MoveCursor(int moveCurX, int moveCurZ, float fspeed) {

        if(!startGame) { combatMenu.DeactivateHoverMenu(); }
        Vector3 targetPosition = new Vector3(gridControl.GetGridTile(moveCurX, moveCurZ).GetXPos(), cursorY, gridControl.GetGridTile(moveCurX, moveCurZ).GetZPos());
        
        moveCursor.position = new Vector3(gridControl.GetGridTile(moveCurX, moveCurZ).GetXPos(), cursorY, gridControl.GetGridTile(moveCurX, moveCurZ).GetZPos());

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
    // public CollideWithPlayerUnit GetPlayerCollide() {
    //     playerCollide = GameObject.Find("PlayerMove").GetComponent<CollideWithPlayerUnit>();
    //     Debug.Log("On " + x + " " + z + " AHHHHHHHHHHHHHHHHHHHHH");
    //     return playerCollide;
    // }

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
        // gridControl.GetGridTile(orgX, orgZ).UnitOnTile = gridControl.GetGridTile(x, z).UnitOnTile;
        // gridControl.GetGridTile(x, z).UnitOnTile = null;
        UnitManager temp = gridControl.GetGridTile(x, z).UnitOnTile;
        gridControl.GetGridTile(x, z).UnitOnTile = null;
        gridControl.GetGridTile(orgX, orgZ).UnitOnTile = temp;
        // pathFinder.CalcAttack(orgX, orgZ, attackRangeStat , playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
        // pathFinder.calculateMovement(orgX, orgZ, playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
        pathFinder.calculateMovement(orgX, orgZ, gridControl.GetGridTile(orgX, orgZ).UnitOnTile.getMove(), gridControl.GetGridTile(orgX, orgZ).UnitOnTile);
        pathFinder.PrintArea();
        charSelected = true;
        inMenu = false;
        // oneAction = false;
    }

    
    
 

    

    

    


    
}
