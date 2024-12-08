using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    protected int maxHealth;
    protected int currentHealth;
    public UnitStats stats;
    public string UnitType;
    // protected PlayerClassManager classList;
    public int movement;
    public int attackRange;
    [SerializeField] protected string unitName;
    public Weapon primaryWeapon;
    public int XPos {get; set;}
    public int ZPos {get; set;}
    public GameObject unitCircle;
    public CombatMenuManager combatMenuManager;

    protected virtual void Start()
    {
        // classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        // InitializeUnitData();
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
        
        
        // Debug.Log("Start");
    }

    public virtual void InitializeUnitData() {}


    public virtual int getMove() {return 0;}
    public virtual int getAttack() {return 0;}
    public virtual int getCurrentHealth() { return currentHealth; }
    public virtual int getMaxHealth() { return 0; }
    public virtual void setCurrentHealth(int health) {}

    public virtual void TakeDamage(int health) {
        currentHealth -= health;
        if (currentHealth < 0) { currentHealth = 0; }
    }
    public virtual string GetUnitType() { return UnitType; }


    public virtual IEnumerator ExperienceGain(int experience) {
        

        Debug.Log("Now has " + stats.Experience + " experience");
        PlayerClass unitClass = PlayerClassManager.GetUnitClass(stats.UnitClass);

        yield return StartCoroutine(combatMenuManager.GainExperienceMenu(this, experience));

        stats.Experience += experience;

        PlayerStats pStats = (PlayerStats)stats;
        

        while (stats.Experience > 100) {
            Debug.Log("LEVEL UP");

            

            // PlayerClass unitClass = PlayerClassManager.GetUnitClass(stats.UnitClass);
            // if (unitClass == null) {
            //     return;
            // }
            int hlt = 0;
            int atk = 0;
            int mag = 0;
            int def = 0;
            int res = 0;
            int eva = 0;
            int luk = 0;
            int spd = 0;

            while (true) {
                hlt = 0;
                atk = 0;
                mag = 0;
                def = 0;
                res = 0;
                eva = 0;
                luk = 0;
                spd = 0;
                if (Random.Range(0, 101) <= unitClass.Health + pStats.HealthGR) {
                    Debug.Log("Health Level Up");
                    hlt++;
                }

                if (Random.Range(0, 101) <= unitClass.Attack + pStats.AttackGR) {
                    Debug.Log("Attack Level Up");
                    // stats.Attack++;
                    atk++;
                }

                if (Random.Range(0, 101) <= unitClass.Magic + pStats.MagicGR) {
                    Debug.Log("Magic Level Up");
                    // stats.Magic++;
                    mag++;
                }

                if (Random.Range(0, 101) <= unitClass.Defense + pStats.DefenseGR) {
                    Debug.Log("Defense Level Up");
                    // stats.Defense++;
                    def++;
                }

                if (Random.Range(0, 101) <= unitClass.Resistance + pStats.ResistanceGR) {
                    Debug.Log("Resistance Level Up");
                    // stats.Resistance++;
                    res++;
                }

                if (Random.Range(0, 101) <= unitClass.Evasion + pStats.EvasionGR) {
                    Debug.Log("Evasion Level Up");
                    // stats.Evasion++;
                    eva++;
                }

                if (Random.Range(0, 101) <= unitClass.Luck + pStats.LuckGR) {
                    Debug.Log("Luck Level Up");
                    // stats.Luck++;
                    luk++;
                }

                if (Random.Range(0, 101) <= unitClass.Speed + pStats.SpeedGR) {
                    Debug.Log("Speed Level Up");
                    // stats.Speed++;
                    spd++;
                }

                if (hlt + atk + mag + def + res + eva + luk + spd >= 2) {
                    break;
                }

            }

            yield return StartCoroutine(combatMenuManager.LevelUpMenu(this, hlt, atk, mag, spd, def, res, eva, luk));

            stats.Health += hlt;
            stats.Attack += atk;
            stats.Magic += mag;
            stats.Defense += def;
            stats.Resistance += res;
            stats.Evasion += eva;
            stats.Luck += luk;
            stats.Speed += spd;

            stats.Level++;
            stats.Experience -= 100;
        }

        yield return null;
    }

}

