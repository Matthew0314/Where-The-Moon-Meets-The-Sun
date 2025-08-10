using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Linq;


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
    private bool isSwapping = false;
    [SerializeField] GameObject selectedSwapping;

    public bool charSelected;
    // public CollideWithPlayerUnit playerCollide;
    private GameObject currUnit;

    private TurnManager manageTurn;
    private CombatMenuManager combatMenu;
    private BattleStartMenu battleStartMenu;
    private MapManager _currentMap;


    public bool startGame = false;
    private static PlayerInput playerInput;
    public static bool SkipCutscene { get; set; }
    private Vector2 moveInput;

    void Awake()
    {
        gridControl = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        _currentMap = GameObject.Find("GridManager").GetComponent<MapManager>();
        combatMenu = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
        battleStartMenu = GameObject.Find("Canvas").GetComponent<BattleStartMenu>();

        playerInput = GetComponent<PlayerInput>();


    }


    void Update()
    {
        // moveCursor.position = new Vector3(moveCursor.position.x, cursorY, moveCursor.position.z);

        if (battleStartMenu.GetInMapMenu())
        {
            StartMapControl();
            return;
        }
        else if (!battleStartMenu.GetInMapMenu() && !startGame) combatMenu.DeactivateHoverMenu();

        if (!startGame) return;
        // transform.position = new Vector3(transform.position.x, cursorY, transform.position.z);
        if (inMenu)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);
            return;
        }


        if (isAttacking || !manageTurn.IsPlayerTurn()) { }

        else if (playerInput.actions["Select"].WasPressedThisFrame() && gridControl.GetGridTile(x, z).UnitOnTile != null && gridControl.GetGridTile(x, z).UnitOnTile.UnitType == "Enemy" && !pathFinder.selectedEnemies.Contains(gridControl.GetGridTile(x, z).UnitOnTile))
        {
            pathFinder.SpecificEnemyRange(gridControl.GetGridTile(x, z).UnitOnTile);
        }

        else if (playerInput.actions["Select"].WasPressedThisFrame() && gridControl.GetGridTile(x, z).UnitOnTile != null && gridControl.GetGridTile(x, z).UnitOnTile.UnitType == "Enemy" && pathFinder.selectedEnemies.Contains(gridControl.GetGridTile(x, z).UnitOnTile))
        {

            pathFinder.UnSelectEnemies(gridControl.GetGridTile(x, z).UnitOnTile);
        }

        //Toggles enemy ranges for all enemies
        else if (playerInput.actions["ShowRange"].WasPressedThisFrame() && !enemyRangeActive && !inMenu && manageTurn.IsPlayerTurn())
        {
            pathFinder.EnemyRange();
            enemyRangeActive = true;
        }

        else if (playerInput.actions["ShowRange"].WasPressedThisFrame() && enemyRangeActive && !inMenu && manageTurn.IsPlayerTurn())
        {
            pathFinder.DestroyEnemyRange();
            enemyRangeActive = false;
        }


        // else if (playerInput.actions["Select"].WasPressedThisFrame() && !charSelected && playerCollide.collPlayer && manageTurn.IsActive(playerCollide.GetPlayer().stats)) {
        else if (playerInput.actions["Select"].WasPressedThisFrame() && !charSelected && gridControl.GetGridTile(x, z).UnitOnTile?.UnitType == "Player" && manageTurn.IsActive((gridControl.GetGridTile(x, z).UnitOnTile as PlayerUnit)?.GetStats()))
        {
            pathFinder.ResetArea();
            // currUnit = playerCollide.GetPlayerObject();
            currUnit = gridControl.GetGridTile(x, z).UnitOnTile.gameObject;

            // pathFinder.calculateMovement(x, z, playerCollide.GetPlayerMove(), playerCollide.GetPlayer());
            pathFinder.calculateMovement(x, z, gridControl.GetGridTile(x, z).UnitOnTile.GetMove(), gridControl.GetGridTile(x, z).UnitOnTile as PlayerUnit);

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





        if (!inMenu && manageTurn.IsPlayerTurn())
        {
            MoveCursor();
        }

    }


    private void StartMapControl()
    {
        Debug.LogError("Start Control");
        MoveCursor();

        if (playerInput.actions["Select"].WasPressedThisFrame() && IsSwappable() && !isSwapping)
        {

            StartCoroutine(SwapUnits(gridControl.GetGridTile(x, z).UnitOnTile, x, z));
        }

        // if (gridControl.GetGridTile(x,z).UnitOnTile?.UnitType != "Player") {
        //     pathFinder.calculateMovement(x, z, gridControl.GetGridTile(x, z).UnitOnTile.GetMove(), gridControl.GetGridTile(x, z).UnitOnTile);
        //     pathFinder.PrintArea();

        // } else {
        //     pathFinder.DestroyArea();

        // }
    }
    private bool CanPlace() => gridControl.GetGridTile(x, z).UnitOnTile == null || gridControl.GetGridTile(x, z).UnitOnTile == currUnit.GetComponent<UnitManager>();
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    private void MoveCursor()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, moveCursor.position) <= cursorSen)
        {
            if (Mathf.Abs(moveInput.x) >= cursorSen)
            {

                int newX = x + (int)Mathf.Sign(moveInput.x);

                if (gridControl.IsValid(newX, z) && !gridControl.GetGridTile(newX, z).GetTallObstacle() && !charSelected)
                {
                    moveCursor.position += new Vector3(Mathf.Sign(moveInput.x) * gridControl.GetCellSize(), 0f, 0f);
                    x = newX;
                }
                else if (gridControl.IsValid(newX, z) && charSelected && pathFinder.canMove[newX, z])
                {
                    moveCursor.position += new Vector3(Mathf.Sign(moveInput.x) * gridControl.GetCellSize(), 0f, 0f);
                    x = newX;
                }
                // Debug.Log(x + " " + z);
            }

            if (Mathf.Abs(moveInput.y) >= cursorSen)
            {
                int newZ = z + (int)Mathf.Sign(moveInput.y);
                if (gridControl.IsValid(x, newZ) && !gridControl.GetGridTile(x, newZ).GetTallObstacle() && !charSelected)
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(moveInput.y) * gridControl.GetCellSize());
                    z = newZ;
                }
                else if (gridControl.IsValid(x, newZ) && charSelected && pathFinder.canMove[x, newZ])
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(moveInput.y) * gridControl.GetCellSize());
                    z = newZ;
                }
                // Debug.Log(x + " " + z);
            }

            if (gridControl.GetGridTile(x, z).UnitOnTile != null && (startGame || battleStartMenu.GetInMapMenu())) combatMenu.ActivateHoverMenu(gridControl.GetGridTile(x, z).UnitOnTile);
            else combatMenu.DeactivateHoverMenu();
        }
    }

    //Moves a unit from one position to another, teleports and doesnt show movement
    private void MoveUnit(int movX, int movZ)
    {
        Vector3 currentPosition = currUnit.transform.position;
        currUnit.transform.position = new Vector3(gridControl.GetGridTile(movX, movZ).GetXPos(), currentPosition.y, gridControl.GetGridTile(movX, movZ).GetZPos());
    }

    private void MoveUnit(UnitManager unit, int movX, int movZ)
    {
        GameObject temp = unit.gameObject;
        Vector3 currentPosition = temp.transform.position;
        temp.transform.position = new Vector3(gridControl.GetGridTile(movX, movZ).GetXPos(), currentPosition.y, gridControl.GetGridTile(movX, movZ).GetZPos());
    }


    // Moves cursor to a specific x and z position
    public IEnumerator MoveCursor(int moveCurX, int moveCurZ, float fspeed)
    {

        if (!startGame) { combatMenu.DeactivateHoverMenu(); }
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

    public bool IsSwappable()
    {
        if (gridControl.GetGridTile(x, z).UnitOnTile != null && gridControl.GetGridTile(x, z).UnitOnTile.UnitType != "Player") return false;

        if (gridControl.GetGridTile(x, z).UnitOnTile != null)
        {
            UnitManager tempUnit = gridControl.GetGridTile(x, z).UnitOnTile;


            List<string> tempReq = _currentMap.GetRequiredUnits();

            foreach (string t in tempReq)
            {
                if (tempUnit.GetUnitName() == t) return false;
            }
        }
        

        Vector2Int[] temp = _currentMap.GetPlayerStartPositions();

        foreach (Vector2Int t in temp)
        {
            if (x == t.x && z == t.y) return true;
        }

        return false;
    }
    public IEnumerator SwapUnits(UnitManager chosenUnit, int firstX, int firstZ)
    {
        isSwapping = true;
        GridTile tile = gridControl.GetGridTile(firstX, firstZ);

        Vector3 spawnPos = new Vector3(tile.GetXPos(), tile.GetYPos() + 0.01f, tile.GetZPos());

        GameObject tempSwap = Instantiate(selectedSwapping, spawnPos, Quaternion.identity);
        while (true)
        {
            if (playerInput.actions["Back"].WasPressedThisFrame())
            {
                // isSwapping = false;

                Destroy(tempSwap);
                // yield return null;
                break;
            }

            if (playerInput.actions["Select"].WasPressedThisFrame() && IsSwappable() && (gridControl.GetGridTile(x, z).UnitOnTile != chosenUnit))
            {
                if (chosenUnit == null && gridControl.GetGridTile(x, z).UnitOnTile == null) continue;

                UnitManager temp = null;
                if (gridControl.GetGridTile(x, z).UnitOnTile != null)
                {
                    temp = gridControl.GetGridTile(x, z).UnitOnTile;

                }

                if (chosenUnit != null)
                {
                    gridControl.MoveUnit(chosenUnit, -1, -1, x, z);
                    MoveUnit(chosenUnit, x, z);
                    gridControl.GetGridTile(firstX, firstZ).UnitOnTile = null;
                }

                if (temp != null)
                {
                    gridControl.MoveUnit(temp, -1, -1, firstX, firstZ);
                    MoveUnit(temp, firstX, firstZ);
                    if (gridControl.GetGridTile(x, z).UnitOnTile == temp)
                    {
                        gridControl.GetGridTile(x, z).UnitOnTile = null;
                    }

                }
                // int choX = chosenUnit.XPos;
                // int choZ = chosenUnit.ZPos;
                Destroy(tempSwap);
                break;
            }
            yield return null;
        }

        yield return null;
        isSwapping = false;
    }

    public bool BackToStartMenu()
    {
        if (isSwapping) return false;

        return true;
    }

    public int getX() { return x; }
    public int getZ() { return z; }
    public int GetCurX() { return curX; }
    public int GetCurZ() { return curZ; }
    public int GetOrgX() { return orgX; }
    public int GetOrgZ() { return orgZ; }
    public int SetX(int xt) => x = xt;
    public int SetZ(int zt) => z = zt;
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

    public void OutOfMenu()
    {
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
        pathFinder.calculateMovement(orgX, orgZ, gridControl.GetGridTile(orgX, orgZ).UnitOnTile.GetMove(), gridControl.GetGridTile(orgX, orgZ).UnitOnTile);
        pathFinder.PrintArea();
        charSelected = true;
        inMenu = false;
        // oneAction = false;
    }

    public static IEnumerator CheckForSkip()
    {
        while (!SkipCutscene)
        {
            if (playerInput.actions["SkipCutscene"].WasPressedThisFrame()) // Input Manager should have "Skip" defined
            {
                SkipCutscene = true;
            }
            yield return null;
        }
    }
    
    

    
    
 

    

    

    


    
}
