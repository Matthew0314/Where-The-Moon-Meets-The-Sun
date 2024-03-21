using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWithPlayerUnit : MonoBehaviour
{
    private PlayerUnit player;
    public PlayerUnit enemy;
    public FindPath path;
    public bool collPlayer;
    public GameObject currPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
/*        player = null;
        collPlayer = false;
        enemy = null;*/
        
    }


    void OnTriggerEnter(Collider other)
    {
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
