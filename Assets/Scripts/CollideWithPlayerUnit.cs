using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Purpose of this class is to gather data on the enemy or player unit that the cursor collides with
public class CollideWithPlayerUnit : MonoBehaviour
{
    [SerializeField] PlayerGridMovement movementVars;
    [SerializeField] CombatMenuManager hoverMenu;
    private PlayerUnit player;
    public UnitManager enemy;
    public bool collPlayer;
    public bool cantPlace;
    private GameObject currPlayer;
    
    void Start () {
        // movementVars = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        // hoverMenu = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        //Determines where to store game object depending on if its a player or enemy
        //collPlayer indicates if a unit has been selected, if so then anotherone can't be
        if (other.gameObject.CompareTag("PlayerUnit") && !collPlayer)
        {
            Debug.Log("Collide");
            player = other.gameObject.GetComponent<PlayerUnit>();
            collPlayer = true;
            currPlayer = other.gameObject;
            cantPlace = false;
        }
        if ((other.gameObject.CompareTag("PlayerUnit") || other.gameObject.CompareTag("EnemyUnit")) && collPlayer && (other.gameObject != currPlayer)) {
            cantPlace = true;
        }
        if (other.gameObject.CompareTag("EnemyUnit")) {
            enemy = other.gameObject.GetComponent<UnitManager>();
            Debug.Log("Hello Enemy");
            Debug.Log(enemy.getAttack());
        }

        if ((other.gameObject.CompareTag("EnemyUnit") || other.gameObject.CompareTag("PlayerUnit")) && !movementVars.isAttacking) {
            UnitManager temp = other.gameObject.GetComponent<UnitManager>();
            hoverMenu.ActivateHoverMenu(temp);
        }

    }

    void OnTriggerExit(Collider other)
    {
        
        if (!movementVars.isCharSelected() && collPlayer && other.CompareTag("PlayerUnit"))
        {
            player = null;
            collPlayer = false;
        }
        cantPlace = false;

       
        hoverMenu.DeactivateHoverMenu();

      
        
       
    }

    

    public void removePlayer() {
        Debug.Log("Remove Player");
        player = null;
        collPlayer = false;

    }

    //Returns player unit data that would be needed
    public GameObject GetPlayerObject() {return currPlayer;}
    public UnitManager GetPlayer() { Debug.Log("GETTING PLAYER"); return player; }
    public int GetPlayerMove() { return player.getMove();  }
    public int GetPlayerAttack() { return player.getAttack(); }
  
}
