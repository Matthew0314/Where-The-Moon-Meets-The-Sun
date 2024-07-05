using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class is currently being worked on right now please ignore for now
public class TurnManager : MonoBehaviour
{
    private UnitRosterManager playerList;
    private IMaps _currentMap;
    private List<UnitStats> currUnits;  // Current player units
    private Queue<UnitManager> currEnemies;
    private int turns = 0;
    private bool playerTurn;
    private bool enemyTurn; //possibly add another one for ally later on
    private PlayerGridMovement moveGrid;
    private GenerateGrid grid;


    void Start()
    {
        playerTurn = true; enemyTurn = false; turns++;
        playerList = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        moveGrid = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
    }

    //Resets Player List after every player turn
    public void SetLists()
    {
        currUnits = new List<UnitStats>();


        List<UnitStats> temp = _currentMap.GetMapUnitStats();
 
        for (int i = 0; i < temp.Count; i++)
        {
            currUnits.Add(temp[i]);
        }
    }

    //Resets Enemy List after every enemy turn
    public void SetEnemyList() {
        currEnemies = new Queue<UnitManager>();

        Queue<UnitManager> temp = _currentMap.GetMapEnemies();

        foreach(UnitManager element in temp) {
            currEnemies.Enqueue(element);
            // Debug.Log(currEnemies.Peek().stats.UnitName);
        }

    }

    //After unit completes an action this is called
    //Possibly see if you can pass object as a parameter
    public void RemovePlayer(UnitStats player)
    {
        currUnits.Remove(player);
        
        // CheckPhase();
    }

    //Removes Enemy from the queue if they have been killed during the player phase
    public void RemoveEnemy(UnitManager ene) {
        Queue<UnitManager> temp = currEnemies;

            int queueCou = temp.Count;

            for (int i = 0; i < queueCou; i++) {
                UnitManager eneTemp = temp.Dequeue();
                if (eneTemp.stats.EnemyID == ene.stats.EnemyID) {
                    
                    continue;
                }
                temp.Enqueue(eneTemp);
        }

        currEnemies = temp;
    }

    //Executes all enemy actions who are in the queue based on the AI script that is attached to them
    private IEnumerator EnemyPhase() {
        Transform objectTrans;
        Vector3 targetPosition;
        float speed;
        float step;
        int count = currEnemies.Count;

      
        Queue<UnitManager> tempQueue = new Queue<UnitManager>();

        foreach(UnitManager element in currEnemies) {
            tempQueue.Enqueue(element);
    
        }

        for (int i = 0; i < count; i++)
        {
            UnitManager temp = tempQueue.Dequeue();
            GameObject tempGameObj = temp.gameObject;

            objectTrans = moveGrid.transform;

            // Vector3 targetPosition = new Vector3(grid.GetGridTile(4, 3).GetXPos(), grid.GetGridTile(4, 3).GetYPos(), grid.GetGridTile(4, 3).GetZPos());
            // objTransform.position = Vector3.MoveTowards(objTransform.position, targetPosition, 20f * Time.deltaTime);
       

      

            targetPosition = new Vector3(grid.GetGridTile(temp.XPos, temp.ZPos).GetXPos(), grid.GetGridTile(temp.XPos, temp.ZPos).GetYPos() + 0.30f, grid.GetGridTile(temp.XPos, temp.ZPos).GetZPos());
            speed = 40f; // Speed of movement
            moveGrid.moveCursor.position = new Vector3(grid.GetGridTile(temp.XPos, temp.ZPos).GetXPos(), grid.GetGridTile(temp.XPos, temp.ZPos).GetYPos() + 0.30f, grid.GetGridTile(temp.XPos, temp.ZPos).GetZPos());

            // // Move the enemy towards the target position
            while (Vector3.Distance(objectTrans.position, targetPosition) > 0.01f)
            {
                // Calculate the step based on speed and deltaTime
                step = speed * Time.deltaTime;

                // Move the enemy towards the target position gradually
                objectTrans.position = Vector3.MoveTowards(objectTrans.position, targetPosition, step);

                yield return null; // Wait for the next frame
            }

            objectTrans.position = targetPosition; // Ensure exact position when reached

            // Debug.Log("Enemy reached the target position.");
            IEnemyAI AIenemy = tempGameObj.GetComponent<IEnemyAI>();
            Debug.Log("AI START");
            yield return StartCoroutine(AIenemy.enemyAttack(temp.gameObject));
            Debug.Log("AI END");
            if (temp.getCurrentHealth() <= 0) {
                Destroy(tempGameObj);
            }
            yield return new WaitForSeconds(1);
        }

        SetEnemyList();

        objectTrans = moveGrid.transform;

        targetPosition = new Vector3(grid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).GetXPos(), grid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).GetYPos() + 0.30f, grid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).GetZPos());
        speed = 40f;
        
        moveGrid.moveCursor.position = new Vector3(grid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).GetXPos(), grid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).GetYPos() + 0.30f, grid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).GetZPos());

        // // Move the enemy towards the target position
        while (Vector3.Distance(objectTrans.position, targetPosition) > 0.01f)
        {
            // Calculate the step based on speed and deltaTime
            step = speed * Time.deltaTime;

            // Move the enemy towards the target position gradually
            objectTrans.position = Vector3.MoveTowards(objectTrans.position, targetPosition, step);

            yield return null; // Wait for the next frame
        }

        objectTrans.position = targetPosition; // Ensure exact position when reached


        turns++; 
            
        Debug.Log("PLAYER PHASE");
        Debug.Log("Turn: " + turns);

        playerTurn = true;
        enemyTurn = false;


    }

    //After every action the player makes it checks to see if there are still units, if not then it starts the enemy phase
    public void CheckPhase()
    {
        if (playerTurn)
        {
            if(currUnits.Count == 0)
            {
                playerTurn = false;
                enemyTurn = true;
                SetLists();
                Debug.Log("ENEMY PHASE");
            }
        }
        if (enemyTurn)
        {
            
            StartCoroutine(EnemyPhase());
            
        }
    }

    public bool IsPlayerTurn() { return playerTurn; }
    public bool IsEnemyTurn() { return enemyTurn; }

    public bool IsActive(UnitStats player) { 
        return currUnits.Contains(player); 
    }
    
}
