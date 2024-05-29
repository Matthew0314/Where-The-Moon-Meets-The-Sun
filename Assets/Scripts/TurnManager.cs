using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class is currently being worked on right now please ignore for now
public class TurnManager : MonoBehaviour
{
    private UnitRosterManager playerList;
    private List<UnitStats> currUnits;  // Current player units
    private int turns = 0;
    private bool playerTurn;
    private bool enemyTurn; //possibly add another one for ally later on


    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true; enemyTurn = false; turns++;
        playerList = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        SetLists(); //This needs to removed later, should be called when player clicks start
    }

    // Update is called once per frame
    void Update()
    {
        // CheckPhase();
    }

    private void SetLists()
    {
        currUnits = playerList.getMapUnits();
    }

    //After unit completes an action this is called
    //Possibly see if you can pass object as a parameter
    public void RemovePlayer(UnitStats player)
    {
        currUnits.Remove(player);
        CheckPhase();
    }

    private void AddPlayer()
    {

    }

    private void EnemyPhase() {
        
    }

    private void CheckPhase()
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
            playerTurn = true;
            enemyTurn = false;
            turns++;
            
            Debug.Log("PLAYER PHASE");
            Debug.Log("Turn: " + turns);
        }
    }

    public bool isPlayerTurn() { return playerTurn; }
    public bool isEnemyTurn() { return enemyTurn; }

    public bool isActive(UnitStats player) { return currUnits.Contains(player); }
    
}
