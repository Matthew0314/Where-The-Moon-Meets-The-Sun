using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Purpose of this class is to gather data on the enemy or player unit that the cursor collides with
public class CollideWithPlayerUnit : MonoBehaviour
{
    private PlayerUnit player;
    public PlayerUnit enemy;
    public FindPath path;
    public bool collPlayer;
    public GameObject currPlayer;

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
        }
        if (other.gameObject.CompareTag("EnemyUnit")) {
            enemy = other.gameObject.GetComponent<PlayerUnit>();
            Debug.Log("Hello Enemy");
        }

    }

    void OnTriggerExit(Collider other)
    {
        //If it exits both will be set to null, only if the unit hasn't been selected by the player
        if (other.gameObject.CompareTag("PlayerUnit") && collPlayer)
        {
            player = null;
            collPlayer = false;
        }
       
    }

    public PlayerUnit GetPlayer() { return player; }
    public int GetPlayerMove() { return player.getMove();  }
    public int GetPlayerAttack() { return player.getAttack(); }
  
}
