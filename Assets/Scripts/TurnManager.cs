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
    private Queue<EnemyUnit> currEnemies;
    private int turns = 0;
    private bool playerTurn;
    private bool enemyTurn; //possibly add another one for ally later on


    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true; enemyTurn = false; turns++;
        playerList = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        // SetLists(); //This needs to removed later, should be called when player clicks start
    }

    // Update is called once per frame
    void Update()
    {
        // CheckPhase();
    }

    public void SetLists()
    {
        currUnits = new List<UnitStats>();


        List<UnitStats> temp = playerList.getMapUnits();
 
        for (int i = 0; i < temp.Count; i++)
        {
            currUnits.Add(temp[i]);
        }
    }

    public void SetEnemyList() {
        currEnemies = new Queue<EnemyUnit>();

        Queue<EnemyUnit> temp = _currentMap.GetMapEnemies();

        foreach(EnemyUnit element in temp) {
            currEnemies.Enqueue(element);
            Debug.Log(currEnemies.Peek().stats.UnitName);
        }

    }

    //After unit completes an action this is called
    //Possibly see if you can pass object as a parameter
    public void RemovePlayer(UnitStats player)
    {
        currUnits.Remove(player);
        // CheckPhase();
    }

    private void AddPlayer()
    {

    }

    // private void EnemyPhase() {
    //     int count = currEnemies.Count;
    //     for (int i = 0; i < count; i++)
    //     {
    //         EnemyUnit temp = currEnemies.Dequeue();
    //         GameObject tempGameObj = temp.gameObject;
    //         IEnemyAI AIenemy = tempGameObj.GetComponent<IEnemyAI>();
    //         AIenemy.enemyAttack(temp.gameObject);
    //     }

    // }

    private IEnumerator EnemyPhase() {
        int count = currEnemies.Count;
        for (int i = 0; i < count; i++)
        {
            EnemyUnit temp = currEnemies.Dequeue();
            GameObject tempGameObj = temp.gameObject;
            IEnemyAI AIenemy = tempGameObj.GetComponent<IEnemyAI>();
            yield return StartCoroutine(AIenemy.enemyAttack(temp.gameObject));
        }

        SetEnemyList();

        turns++;
            
        Debug.Log("PLAYER PHASE");
        Debug.Log("Turn: " + turns);

        playerTurn = true;
        enemyTurn = false;


    }

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

    public bool isPlayerTurn() { return playerTurn; }
    public bool isEnemyTurn() { return enemyTurn; }

    public bool isActive(UnitStats player) { 
        return currUnits.Contains(player); 
    }
    
}
