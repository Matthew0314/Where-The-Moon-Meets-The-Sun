using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
// using LTWB.UnitStats;

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

        stats.CurrentHealth = 2;
        // currentHealth = 2;

    }

    protected void Update() {
        healthBar.fillAmount = (float)GetCurrentHealth() / (float)GetHealth(); 

        if (!turnManager.IsActive(this))
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

        yield return StartCoroutine(combatMenuManager.GainExperienceMenu(this, experience, skillType1, skillInc, sp));

        stats.AddSP(sp);

        

        stats.Experience += experience;

        PlayerStats pStats = (PlayerStats)stats;
        
        pStats.AddSkillExperience(Enum.TryParse<SkillType>(skillType1, out var skillType) ? skillType : SkillType.Sword, skillInc);

        //TODO: Maybe add a popup confirming that they leveled up a skill 
        //TODO: Make sure that experience isn't added if maxed out
        

        while (pStats.Experience >= 100)
        {
            CoreStats gains = new CoreStats();

            while (true)
            {
                gains = new CoreStats();
                int totalGainsCount = 0;

                // Loop through our core stat types dynamically
                // Exclude Movement (index 8) since it doesn't level up randomly
                for (int i = 0; i < 8; i++)
                {
                    StatType currentStat = (StatType)i;
                    int combinedGrowth = pStats.GetGrowthRate(currentStat);

                    if (UnityEngine.Random.Range(0, 101) <= combinedGrowth)
                    {
                        totalGainsCount++;
                        
                        switch (currentStat)
                        {
                            case StatType.Health:     gains.health++; break;
                            case StatType.Attack:     gains.attack++; break;
                            case StatType.Magic:      gains.magic++; break;
                            case StatType.Defense:    gains.defense++; break;
                            case StatType.Resistance: gains.resistance++; break;
                            case StatType.Speed:      gains.speed++; break;
                            case StatType.Evasion:    gains.evasion++; break;
                            case StatType.Luck:       gains.luck++; break;
                        }
                    }
                }

                if (totalGainsCount >= 2) break;
            }

            yield return StartCoroutine(combatMenuManager.LevelUpMenu(
                this, 
                gains.health, 
                gains.attack, 
                gains.magic, 
                gains.speed, 
                gains.defense, 
                gains.resistance, 
                gains.evasion, 
                gains.luck
            ));

            pStats.BaseStats.health     += gains.health;
            pStats.BaseStats.attack     += gains.attack;
            pStats.BaseStats.magic      += gains.magic;
            pStats.BaseStats.defense    += gains.defense;
            pStats.BaseStats.resistance += gains.resistance;
            pStats.BaseStats.speed      += gains.speed;
            pStats.BaseStats.evasion    += gains.evasion;
            pStats.BaseStats.luck       += gains.luck;

            pStats.CurrentHealth += gains.health;

            pStats.Level++;
            pStats.Experience -= 100;
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
        SetPrimaryWeapon(stats.GetPrimaryWeapon());
        // stats.CurrentHealth = 2;
    }

    public PlayerStats GetPlayerStats() => stats as PlayerStats;

    public PlayerClass GetPlayerClass() => GetPlayerStats().GetClass();

    public override int GetMove() => AdjustMovement(Mathf.Max(0, stats.GetClass().GetGrowth(StatType.Movement) + base.GetMove()));
    // Returns info about the characters unit/class type
    public override bool GetAirBorn() => GetPlayerClass().HassAttribute(ClassAttributes.Airborn);
    public override bool GetArmored() => GetPlayerClass().HassAttribute(ClassAttributes.Armored);
    public override bool GetMounted() => GetPlayerClass().HassAttribute(ClassAttributes.Mounted);
    public override bool GetWhisper() => GetPlayerClass().HassAttribute(ClassAttributes.Whisper);


    

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
