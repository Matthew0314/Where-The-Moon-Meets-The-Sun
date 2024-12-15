using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : UnitManager
{
    // private Image healthBar;
    private Material originalMaterial;
    private TurnManager turnManager;
    [SerializeField] private Material grayscaleMaterial;

    protected override void Start() {
        // classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        InitializeUnitData();
        Transform childImage = transform.Find("PlayerCircle/Canvas/UnitBar");
        healthBar = childImage.GetComponent<Image>();
        unitCircle = transform.Find("PlayerCircle").gameObject;
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
        originalMaterial = healthBar.material;
        turnManager = GameObject.Find("GridManager").GetComponent<TurnManager>();

    }

    protected void Update() {
        healthBar.fillAmount = (float)currentHealth / (float)stats.Health; 

        if (!turnManager.IsActive(this.stats))
        {
            // Apply grayscale material
            healthBar.material = grayscaleMaterial;

            // Adjust grayscale intensity (fully grayscale in this example)
            healthBar.material.SetFloat("_GrayAmount", 1f);
        }
        else
        {
            // Reset to the original material
            healthBar.material = originalMaterial;
        }
    }

    public override IEnumerator ExperienceGain(int experience) {
        

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



    public override void InitializeUnitData()
    {
        
        stats = UnitRosterManager.GetUnitStats(unitName);
        maxHealth = stats.Health;
        currentHealth = maxHealth;
        UnitType = "Player"; 
        primaryWeapon = stats.GetWeaponAt(0);
        Debug.Log(stats.UnitName + " Has been initlialized");

    }

    public override int getMove() { return stats.getClass().Movement + stats.Movement; }
    public override int getAttack() { 
        if (primaryWeapon != null) {
            return primaryWeapon.Range;
        } else { return 0;}
    }

    

    public override int getCurrentHealth() { return currentHealth; }
    public override int getMaxHealth() { 
        if (stats == null){
            Debug.LogError("HELP THERES NO STATS!");
        }
        return stats.Health; 
        }
    public override void setCurrentHealth(int health) { currentHealth = health; }
    public override string GetUnitType() { return UnitType; }

}
