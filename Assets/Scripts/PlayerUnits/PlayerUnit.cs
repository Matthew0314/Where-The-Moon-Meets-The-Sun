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
        extraHealth1 = transform.Find("PlayerCircle/Canvas/UnitCircle/ExtraHealth1").GetComponent<Image>();
        extraHealth1.gameObject.SetActive(false);
        numHealthBars++;
        // AddHealthBar();
        unitCircle = transform.Find("PlayerCircle").gameObject;
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
        originalMaterial = healthBar.material;
        turnManager = GameObject.Find("GridManager").GetComponent<TurnManager>();
        // currentHealth = 2;

    }

    protected void Update() {
        healthBar.fillAmount = (float)GetCurrentHealth() / (float)GetHealth(); 

        if (!turnManager.IsActive(this.stats))
        {
            // Apply grayscale material
            healthBar.material = grayscaleMaterial;

            // Adjust grayscale intensity (fully grayscale in this example)
            healthBar.material.SetFloat("_GrayAmount", 1f);
            // if (extraHealth.gameObject.activeInHierarchy) {
            //     // extraHealth.material.SetFloat("_GrayAmount", 1f);
            // }
        }
        else
        {
            // Reset to the original material
            healthBar.material = originalMaterial;
            // extraHealth.material 
        }
    }

    public override IEnumerator ExperienceGain(int experience, int numberTimesAttacked, string skillType1, bool killedEnemy, bool healed) {

        PlayerClass unitClass = PlayerClassManager.GetUnitClass(stats.UnitClass);

        int sp = CalculateSP(experience, killedEnemy);

        int skillInc = CalculateSkillEXP(numberTimesAttacked);

        yield return StartCoroutine(combatMenuManager.GainExperienceMenu(this, experience, skillType1, 100, sp));

        stats.AddSP(sp);

        stats.Experience += experience;

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

    private int CalculateSkillEXP(int numAtk) {
        return numAtk * 2;
    }

    private int CalculateSP(int EXPObt, bool killEne) {

        // return EXPObt / 2;
        return killEne ? EXPObt : EXPObt / 2;
    }



    public override void InitializeUnitData()
    {
        stats = UnitRosterManager.GetUnitStats(unitName);
        // maxHealth = stats.Health;
        // currentHealth = maxHealth;
        UnitType = "Player";
        // stats.SetPrimaryWeapon(stats.GetWeaponAt(0));
        stats.FindAPrimaryWeapon();
        // stats.CurrentHealth = 2;
    }

    public PlayerStats GetPlayerStats() => stats as PlayerStats;

    public PlayerClass GetPlayerClass() => GetPlayerStats().GetClass();

    public override int GetMove() => AdjustMovement(Mathf.Max(0, stats.GetClass().Movement + base.GetMove()));
    // Returns info about the characters unit/class type
    public override bool GetAirBorn() => GetPlayerClass().AirBorn;
    public override bool GetArmored() => GetPlayerClass().Armored;
    public override bool GetMounted() => GetPlayerClass().Mounted;
    public override bool GetWhisper() => GetPlayerClass().Whisper;


    

    // public override int getCurrentHealth() { return stats.CurrentHealth; }
    // public override int getMaxHealth() => stats.Health; 
    // public override void setCurrentHealth(int health) { stats.CurrentHealth = health; }
    public override string GetUnitType() { return UnitType; }


    public override void AddHealthBar() {
        if (numHealthBars < 2) {
            numHealthBars++;
            extraHealth1.gameObject.SetActive(true);
        }
    }

    public override int GetHealthBars() => numHealthBars;


    // public override int GetHealthBars()

}
