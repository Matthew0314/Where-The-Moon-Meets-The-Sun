using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class is currently being worked on right now please ignore for now
public class TurnManager : MonoBehaviour
{
    [SerializeField] UnitRosterManager playerList;
    [SerializeField] MapManager _currentMap;
    private List<UnitStats> currUnits = new List<UnitStats>();  // Current player units
    private Queue<UnitManager> currEnemies;
    private Queue<UnitManager> currEnemies2;
    private Queue<UnitManager> currAllies;
    private int turns = 0;
    private bool playerTurn;
    private bool enemyTurn; //possibly add another one for ally later on
    [SerializeField] PlayerGridMovement moveGrid;
    [SerializeField] GenerateGrid grid;
    [SerializeField] CombatMenuManager combatMenuManager;

    enum Turn
    {
        Player,
        Enemy1,
        Enemy2,
        Ally
    }

    Turn currentTurn;


    void Start()
    {
        playerTurn = true; enemyTurn = false; turns++;
        currentTurn = Turn.Player;
        // playerList = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        _currentMap = GameObject.Find("GridManager").GetComponent<MapManager>();
        // moveGrid = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        // grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        // combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
    }

    //Resets Player List after every player turn
    private void SetLists()
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

        Queue<UnitManager> temp = _currentMap.GetMapEnemies1();

        foreach(UnitManager element in temp) {
            currEnemies.Enqueue(element);
        }

    }

    public void AddEnemy(UnitManager enemyAdd) {
        currEnemies.Enqueue(enemyAdd);
    }

    //After unit completes an action this is called
    //Possibly see if you can pass object as a parameter
    public void RemovePlayer(UnitStats player)
    {
        currUnits.Remove(player);
    }

    public void AddPlayer(UnitStats player) => currUnits.Add(player);

    //Removes Enemy from the queue if they have been killed during the player phase
    public void RemoveEnemy(UnitManager ene)
    {
        Queue<UnitManager> temp = currEnemies;

        int queueCou = temp.Count;

        for (int i = 0; i < queueCou; i++)
        {
            UnitManager eneTemp = temp.Dequeue();
            if (eneTemp.stats.EnemyID == ene.stats.EnemyID)
            {

                continue;
            }
            temp.Enqueue(eneTemp);
        }

        currEnemies = temp;


    }

    //Executes all enemy actions who are in the queue based on the AI script that is attached to them
    private IEnumerator EnemyPhase() {

        

        yield return StartCoroutine(_currentMap.CheckEvents());

        yield return StartCoroutine(combatMenuManager.PhaseStart("Enemy"));
        
        int count = currEnemies.Count;
 
      
        Queue<UnitManager> tempQueue = new Queue<UnitManager>();

        foreach(UnitManager element in currEnemies) {
            tempQueue.Enqueue(element);
    
        }

        for (int i = 0; i < count; i++)
        {
            UnitManager temp = tempQueue.Dequeue();
            GameObject tempGameObj = temp.gameObject;

            IEnemyAI AIenemy = tempGameObj.GetComponent<IEnemyAI>();
  
            yield return StartCoroutine(AIenemy.enemyAttack(temp.gameObject));
            bool didAct = false;
            if (AIenemy.DidAction) { didAct = true; }

            if (temp.getCurrentHealth() <= 0) {
                Destroy(tempGameObj);
                yield return new WaitForSeconds(1f);
            }

            if (didAct) { yield return new WaitForSeconds(0.5f); }
            yield return StartCoroutine(_currentMap.CheckClearCondition());
            yield return StartCoroutine(_currentMap.CheckDefeatCondition());

        }

        // yield return new WaitForSeconds(2f);

        SetEnemyList();
        
        yield return StartCoroutine(_currentMap.CheckEvents());

        yield return StartCoroutine(moveGrid.MoveCursor(moveGrid.getX(), moveGrid.getZ(), 200f));

        turns++; 

        

        yield return StartCoroutine(combatMenuManager.PhaseStart("Player"));

        // yield return StartCoroutine(moveGrid.MoveCursor(moveGrid.getX(), moveGrid.getZ()));

        
            
        Debug.Log("PLAYER PHASE");
        Debug.Log("Turn: " + turns);

        currentTurn = Turn.Player;
        playerTurn = true;
        enemyTurn = false;

       


    }

    //After every action the player makes it checks to see if there are still units, if not then it starts the enemy phase
    public void CheckPhase()
    {
        if (playerTurn)
        {
            if (currUnits.Count == 0)
            {
                playerTurn = false;
                enemyTurn = true;
                currentTurn = Turn.Enemy1;
                SetLists();
                Debug.Log("ENEMY PHASE");
                // currentTurn = Turn.Enemy1;
            }
        }
        if (enemyTurn)
        {

            StartCoroutine(EnemyPhase());
            // currentTurn = Turn.Player;
            
        }
    }

    // public bool IsPlayerTurn()
    // {
    //     return playerTurn;
    // }
    // public bool IsEnemyTurn() { return enemyTurn; }

    public bool IsPlayerTurn()
    {
        if (currentTurn == Turn.Player) return true;
        else return false;
    }

    public bool IsEnemyTurn()
    {
        if (currentTurn != Turn.Player) return true;
        else return false;
    }

    //Checks if a player hasn't been moved yet
    public bool IsActive(UnitStats player)
    {
        // Debug.LogError(currUnits.Contains(player));
        return currUnits.Contains(player);
    }

    public int GetTurns() {
        return turns;
    }
    
}
