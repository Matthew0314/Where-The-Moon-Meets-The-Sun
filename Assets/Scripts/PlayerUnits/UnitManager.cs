using System.Collections;
using System.Collections.Generic;

//using System.Diagnostics;
using UnityEngine;

// public class UnitManager : MonoBehaviour
// {
//     [SerializeField] int maxHealth;
//     public UnitStats stats;
//     // public PlayerClass uClass;
//     public string UnitType;
//     //private UnitRosterManager roster;
//     private PlayerClassManager classList;
//     public int movement;
//     public int attackRange;
//     [SerializeField] string unitName;
//     //public PlayerStats stats;

//     // Start is called before the first frame update
//     void Start()
//     {
//         //roster = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
//         classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
//         if(UnitType == "Player") {
//             stats = UnitRosterManager.GetUnitStats(unitName);
//         }
        
//         // Debug.Log(stats.UnitClass);
//         // if(UnitType == "Player") {
//         //     uClass = PlayerClassManager.GetUnitClass(stats.UnitClass);
//         // }
//         // UniteType = stats.UnitType
        
//         // Debug.Log("Move Test " + uClass.Movement);

//         maxHealth = stats.Health;
//         //movement += stats.Movement;
//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }




//     void initlizeUnitData()
//     {
//         //stats.movement = movement;
//         //stats.maxHealth = maxHealth;
//     }

//     //Return Stats Section

//     public int getMove() { return stats.getClass().Movement + stats.Movement; }
//     public int getAttack() { return attackRange;  }
// }


// public abstract class UnitManager : MonoBehaviour {
//     [SerializeField] int maxHealth;
//     public UnitStats stats;
//     public PlayerClass uClass;
//     private PlayerClassManager classList;
//     protected int movement;
//     protected int attackRange;
//     public string unitName;

//     void Start() {
//          classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
//     }

//     public UnitManager() {
       
//     }

//     public abstract void InitUnit();
//     public abstract int getMove();
//     public abstract int getAttack();

// }

public class UnitManager : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    public UnitStats stats;
    public string UnitType;
    protected PlayerClassManager classList;
    public int movement;
    public int attackRange;
    [SerializeField] protected string unitName;
    public Weapon primaryWeapon;

    protected virtual void Start()
    {
        classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        InitializeUnitData();
        
        Debug.Log("Start");
    }

    protected virtual void InitializeUnitData() {}


    public virtual int getMove() {return 0;}
    public virtual int getAttack() {return 0;}

}

