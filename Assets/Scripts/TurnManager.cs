using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnManager : MonoBehaviour
{ 
    enum Turn
    {
        Player,
        Enemy1,
        Enemy2,
        Ally
    }

    [SerializeField] UnitRosterManager playerList;
    [SerializeField] MapManager _currentMap;
    [SerializeField] PlayerGridMovement moveGrid;
    [SerializeField] GenerateGrid grid;
    [SerializeField] CombatMenuManager combatMenuManager;

    // private List<UnitStats> currUnits = new List<UnitStats>();  // Current player units
    private List<UnitManager> playerUnits = new List<UnitManager>(); // All player units
    private Queue<UnitManager> currEnemies;
    private Queue<UnitManager> currEnemies2;
    private Queue<UnitManager> currAllies;
    private int turns = 0;
    private int currentCP;
    private int currentActionCost;
    Turn currentTurn;

    private void Awake() {
        _currentMap = GameObject.Find("GridManager").GetComponent<MapManager>();
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
    }

    private void Start() {
        turns++;
        currentTurn = Turn.Player;
        currentCP = _currentMap.GetCP();
    }

    //Resets Player List after every player turn
    private void SetLists()
    {
        // currUnits = new List<UnitStats>();
        playerUnits = new List<UnitManager>();
        // List<UnitStats> temp = _currentMap.GetMapUnitStats();
        List<UnitManager> temp = _currentMap.GetMapUnits();
 
        for (int i = 0; i < temp.Count; i++)
        {
            // currUnits.Add(temp[i]);
            playerUnits.Add(temp[i]);
        }

        // List<UnitManager> tempU = _currentMap.GetMapUnits();

        foreach (UnitManager t in temp) {
            Debug.LogError("YAYAYAYAYAYAAY " + t.GetStats().UnitName);
            t.ResetNumberTimesActed();
        }

        if (_currentMap.UsingCP()) {
            currentCP = _currentMap.GetCP();
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
    public void RemovePlayer(UnitManager player)
    {
        if(!_currentMap.UsingCP())
            playerUnits.Remove(player);
    }

    public void AddPlayer(UnitManager player) => playerUnits.Add(player);

    //Removes Enemy from the queue if they have been killed during the player phase
    public void RemoveEnemy(UnitManager ene)
    {
        Queue<UnitManager> temp = currEnemies;

        int queueCou = temp.Count;

        for (int i = 0; i < queueCou; i++)
        {
            UnitManager eneTemp = temp.Dequeue();
            if (eneTemp.GetUnitID() == ene.GetUnitID())
            {

                continue;
            }
            temp.Enqueue(eneTemp);
        }

        currEnemies = temp;


    }

    //Executes all enemy actions who are in the queue based on the AI script that is attached to them
    private IEnumerator EnemyPhase() {

        combatMenuManager.DeactivateHoverMenu();
        combatMenuManager.DeactivateCPMenu();
        

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

            if (temp.GetCurrentHealth() <= 0) {
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

        

        

        StartCoroutine(combatMenuManager.UpdateCommandPointMenu());

        yield return StartCoroutine(CheckAilments());

        currentTurn = Turn.Player;

       


    }

    //After every action the player makes it checks to see if there are still units, if not then it starts the enemy phase
    private void CheckPhase()
    {
        if (currentTurn == Turn.Player)
        {
            if (playerUnits.Count == 0 || (_currentMap.UsingCP() && currentCP == 0))
            {
                currentTurn = Turn.Enemy1;
                SetLists();
                Debug.Log("ENEMY PHASE");
                currentTurn = Turn.Enemy1;
            }
        }
        if (currentTurn == Turn.Enemy1)
        {

            StartCoroutine(EnemyPhase());
            // currentTurn = Turn.Player;
            
        }
    }

    public void EndTurn() {
        playerUnits.Clear();
        currentCP = 0;
        SetCurrentActionCost(0);
        CheckPhase();
    }


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
    public bool IsActive(UnitManager player)
    {
        // Debug.LogError(currUnits.Contains(player));
        return playerUnits.Contains(player);
    }

    public int GetTurns() {
        return turns;
    }

    public void SetCurrentActionCost(int cost) {
        currentActionCost = cost;
    }

    public void AfterAction(UnitManager unit) {
        if(_currentMap.UsingCP())
            currentCP -= currentActionCost;
        SetCurrentActionCost(0);
        unit.IncNumberTimesActed();
        StartCoroutine(combatMenuManager.UpdateCommandPointMenu());
        CheckPhase();
    }

    public int GetCP() {
        return currentCP;
    }

    public IEnumerator CheckAilments() {
        // List<UnitManager> allUnits = _currentMap.GetMapUnits();

        // foreach (UnitManager unit in allUnits) {
        //     yield return StartCoroutine(unit.CheckAilments());
        // }
        
        yield break;
    }
    
}
