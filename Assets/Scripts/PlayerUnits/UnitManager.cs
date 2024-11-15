using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    protected int maxHealth;
    public int currentHealth;
    public UnitStats stats;
    public string UnitType;
    // protected PlayerClassManager classList;
    public int movement;
    public int attackRange;
    [SerializeField] protected string unitName;
    public Weapon primaryWeapon;
    public int XPos {get; set;}
    public int ZPos {get; set;}

    protected virtual void Start()
    {
        // classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        // InitializeUnitData();
        
        // Debug.Log("Start");
    }

    public virtual void InitializeUnitData() {}


    public virtual int getMove() {return 0;}
    public virtual int getAttack() {return 0;}
    public virtual int getCurrentHealth() { return 0; }
    public virtual int getMaxHealth() { return 0; }
    public virtual void setCurrentHealth(int health) {}
    public virtual string GetUnitType() { return UnitType; }


    public virtual void ExperienceGain(int experience) {
        stats.Experience += experience;

        Debug.Log("Now has " + stats.Experience + " experience");

        if (stats.Experience > 100) {
            Debug.Log("LEVEL UP");

            

            PlayerClass unitClass = PlayerClassManager.GetUnitClass(stats.UnitClass);
            if (unitClass == null) {
                return;
            }
            if (Random.Range(0, 101) <= unitClass.Health) {
                Debug.Log("Health Level Up");
                stats.Health++;
            }

            if (Random.Range(0, 101) <= unitClass.Attack) {
                Debug.Log("Attack Level Up");
                stats.Attack++;
            }

            if (Random.Range(0, 101) <= unitClass.Magic) {
                Debug.Log("Magic Level Up");
                stats.Magic++;
            }

            if (Random.Range(0, 101) <= unitClass.Defense) {
                Debug.Log("Defense Level Up");
                stats.Defense++;
            }

            if (Random.Range(0, 101) <= unitClass.Resistance) {
                Debug.Log("Resistance Level Up");
                stats.Resistance++;
            }

            if (Random.Range(0, 101) <= unitClass.Evasion) {
                Debug.Log("Evasion Level Up");
                stats.Evasion++;
            }

            if (Random.Range(0, 101) <= unitClass.Luck) {
                Debug.Log("Luck Level Up");
                stats.Luck++;
            }

            if (Random.Range(0, 101) <= unitClass.Speed) {
                Debug.Log("Speed Level Up");
                stats.Speed++;
            }

            stats.Experience -= 100;
        }
    }

}

