using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    protected int maxHealth;
    public int currentHealth;
    public UnitStats stats;
    public string UnitType;
    protected PlayerClassManager classList;
    public int movement;
    public int attackRange;
    [SerializeField] protected string unitName;
    public Weapon primaryWeapon;
    public int XPos {get; set;}
    public int ZPos {get; set;}

    protected virtual void Start()
    {
        classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        // InitializeUnitData();
        
        Debug.Log("Start");
    }

    public virtual void InitializeUnitData() {}


    public virtual int getMove() {return 0;}
    public virtual int getAttack() {return 0;}
    public virtual int getCurrentHealth() { return 0; }
    public virtual int getMaxHealth() { return 0; }
    public virtual void setCurrentHealth(int health) {}
    public virtual string GetUnitType() { return UnitType; }

}

