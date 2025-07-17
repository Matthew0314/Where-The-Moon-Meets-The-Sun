using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : UnitManager
{
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
        // currentHealth = 2;

    }

    protected void Update() {
        healthBar.fillAmount = (float)getCurrentHealth() / (float)stats.Health; 

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

        PlayerClass unitClass = PlayerClassManager.GetUnitClass(stats.UnitClass);

        yield return StartCoroutine(combatMenuManager.GainExperienceMenu(this, experience + 200));

        stats.Experience += experience + 200;

        PlayerStats pStats = (PlayerStats)stats;
        

        while (stats.Experience >= 100) {

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
                if (Random.Range(0, 101) <= unitClass.Health + pStats.HealthGR) hlt++;
                if (Random.Range(0, 101) <= unitClass.Attack + pStats.AttackGR) atk++;
                if (Random.Range(0, 101) <= unitClass.Magic + pStats.MagicGR) mag++;
                if (Random.Range(0, 101) <= unitClass.Defense + pStats.DefenseGR) def++;
                if (Random.Range(0, 101) <= unitClass.Resistance + pStats.ResistanceGR) res++;
                if (Random.Range(0, 101) <= unitClass.Evasion + pStats.EvasionGR) eva++;
                if (Random.Range(0, 101) <= unitClass.Luck + pStats.LuckGR) luk++;
                if (Random.Range(0, 101) <= unitClass.Speed + pStats.SpeedGR) spd++;

                if (hlt + atk + mag + def + res + eva + luk + spd >= 2) break;

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
        // maxHealth = stats.Health;
        // currentHealth = maxHealth;
        UnitType = "Player";
        primaryWeapon = stats.GetWeaponAt(0);
        // stats.CurrentHealth = 2;
    }

    public override int getMove() { return stats.getClass().Movement + base.getMove(); }


    

    public override int getCurrentHealth() { return stats.CurrentHealth; }
    public override int getMaxHealth() => stats.Health; 
    public override void setCurrentHealth(int health) { stats.CurrentHealth = health; }
    public override string GetUnitType() { return UnitType; }

}
