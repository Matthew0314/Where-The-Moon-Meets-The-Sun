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
    [SerializeField] float speed = 20f;
    [SerializeField] LayerMask obstacleLayer;
    private static float cursorSen = .35f;
    private GenerateGrid gridControl;
    private FindPath pathFinder;
    private bool oneAction;
    private bool inMenu;
    private bool isAttacking;

    private bool charSelected;
    public CollideWithPlayerUnit playerCollide;
    private GameObject currUnit;
    private int attackRangeStat;
    private PlayerAttack attackPath;
    private TurnManager manageTurn;   

    private GameObject attackButton;
    private GameObject itemButton;
    private GameObject waitButton;




    void Start()
    {
        //creates an object reference
        gridControl = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        playerCollide = GameObject.Find("PlayerMove").GetComponent<CollideWithPlayerUnit>();
        attackPath = GameObject.Find("Player").GetComponent<PlayerAttack>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();

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

        if (Input.GetKeyDown(KeyCode.Space) && !charSelected && playerCollide.collPlayer && oneAction && manageTurn.isActive(playerCollide.GetPlayer().stats)) {
            pathFinder.ResetArea();
            currUnit = playerCollide.GetPlayerObject();
            Debug.Log("Hello");
            attackRangeStat = playerCollide.GetPlayerAttack();
            Debug.Log("No");
            pathFinder.CalcAttack(x, z, attackRangeStat , playerCollide.GetPlayerMove());
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
            pathFinder.CalcAttack(orgX, orgZ, attackRangeStat , playerCollide.GetPlayerMove());
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
            

            attackPath.CalculateAttack(x, z, attackRangeStat);
            attackPath.HighlightAttack();
            
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


    private void activateFirstMenu() {
        attackButton.SetActive(true);
        waitButton.SetActive(true);
        itemButton.SetActive(true);
        //  EventSystem.current.SetSelectedGameObject(attackButton.gameObject);
    }

    private void deactivateFirstMenu() {
        attackButton.SetActive(false);
        waitButton.SetActive(false);
        itemButton.SetActive(false);
    }
    
    public void unitWait() {
        deactivateFirstMenu();
        
        attackPath.DestroyRange();
        
        
        

        pathFinder.DestroyArea();
        charSelected = false;
        inMenu = false;            
           

        playerCollide.removePlayer();
            
    }

    public int getX() { return x;}
    public int getZ() { return z;}
    public bool isCharSelected() { return charSelected; }
    
}
