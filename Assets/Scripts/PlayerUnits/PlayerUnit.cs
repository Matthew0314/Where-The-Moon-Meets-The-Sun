using System.Collections;
using System.Collections.Generic;

//using System.Diagnostics;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] int maxHealth;
    public UnitStats stats;
    public PlayerClass uClass;
    //private UnitRosterManager roster;
    private PlayerClassManager classList;
    public int movement;
    public int attackRange;
    [SerializeField] string unitName;
    //public PlayerStats stats;

    // Start is called before the first frame update
    void Start()
    {
        //roster = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        stats = UnitRosterManager.GetUnitStats(unitName);
        Debug.Log(stats.UnitClass);
        uClass = classList.GetUnitClass(stats.UnitClass);
        Debug.Log("Move Test " + uClass.Movement);

        maxHealth = stats.Health;
        //movement += stats.Movement;
    }

    // Update is called once per frame
    void Update()
    {

    }




    void initlizeUnitData()
    {
        //stats.movement = movement;
        //stats.maxHealth = maxHealth;
    }

    //Return Stats Section

    public int getMove() { return uClass.Movement + stats.Movement; }
    public int getAttack() { return attackRange;  }
}
