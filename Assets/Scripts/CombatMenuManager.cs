using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatMenuManager : MonoBehaviour
{
    private IMaps _currentMap;
    private TurnManager manageTurn;    
    private PlayerGridMovement moveGrid;
    // Start is called before the first frame update
    void Start()
    {
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        moveGrid = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void playerWait() {
        manageTurn.RemovePlayer(moveGrid.playerCollide.GetPlayer().stats);
        moveGrid.unitWait();
    }
}
