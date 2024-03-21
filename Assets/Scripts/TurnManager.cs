using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private UnitRosterManager playerList = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
    private List<UnitStats> currUnits;  // Current player units
    private int turns = 0;
    private bool playerTurn;
    private bool enemyTurn; //possibly add another one for ally later on


    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true; enemyTurn = false; turns++;
        SetLists();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPhase();
    }

    private void SetLists()
    {
        currUnits = playerList.getMapUnits();
    }

    //After unit completes an action this is called
    //Possibly see if you can pass object as a parameter
    private void RemovePlayer()
    {

    }

    private void AddPlayer()
    {

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
            }
        }
        if (enemyTurn)
        {
            
        }
    }

    bool isPlayerTurn() { return playerTurn; }
    bool isEnemyTurn() { return enemyTurn; }
    
}
