using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public abstract class UnitManager : MonoBehaviour
{
    protected CombatMenuManager combatMenuManager;
    public GameObject unitCircle;
    public Image healthBar;
    public Image extraHealth1;
    protected int numHealthBars = 0;
    protected int gaugeCharge = 0;
    protected int numberTimesActed = 0;

    protected UnitStats stats;
    public string UnitType { get; set; }

    [SerializeField] protected string unitName;
    // protected Weapon primaryWeapon;

    public int XPos { get; set; }
    public int ZPos { get; set; }

    private List<StatusAilments> statusAilments = new List<StatusAilments>();

    public abstract void InitializeUnitData();

    protected virtual void Start()
    {
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
    }


    // Calculates the damge based on players attack and other units res/def
    public virtual int GetDamage(UnitManager other)
    {
        if (stats.GetPrimaryWeapon().UseMagic)
        {
            
            return GetMagic() - other.GetResistance();
        }
        else
        {
            return GetAttack() - other.GetDefense();
        }
    }

    // Gets stats based on the units stats + status aliments + etc.
    public virtual int GetMove() => BoolStatusAilments(s => s.Freeze) ? int.MinValue : stats.Movement + SumStatusAilments(s => s.Movement);
    public virtual int GetAttack() => Mathf.Max(0, (stats.GetPrimaryWeapon()?.Attack ?? 0) + stats.Attack + SumStatusAilments(s => s.Attack));
    public virtual int GetMagic() => Mathf.Max(0, (stats.GetPrimaryWeapon()?.Attack ?? 0) + stats.Magic + SumStatusAilments(s => s.Magic));
    public virtual int GetDefense() => Mathf.Max(0, stats.Defense + SumStatusAilments(s => s.Defense));
    public virtual int GetResistance() => Mathf.Max(0, stats.Resistance + SumStatusAilments(s => s.Resistance));
    public virtual int GetSpeed() => Mathf.Max(0, stats.Speed + SumStatusAilments(s => s.Speed));
    public virtual int GetLuck() => stats.Luck + SumStatusAilments(s => s.Luck);
    public virtual int GetEvasion() => stats.Evasion + SumStatusAilments(s => s.Evasion);
    public virtual int GetHealth() => stats.Health;
    public virtual int GetCurrentHealth() => stats.CurrentHealth;

    // Calculates the chance that the attack will hit
    public virtual int GetBattleHit(UnitManager other) {
        int temp = GetPrimaryWeapon().HitRate + (GetLuck() * 4) - other.GetEvasion();
        return Mathf.Clamp(temp, 0, 100);
    }

    // Calculates the chance of a critical attack
    public virtual int GetBattleCrit() {
        int temp = GetPrimaryWeapon().CritRate + (int)(GetLuck() / 2);
        return Mathf.Clamp(temp, 0, 100);
    }

    // Code to check to see if unit gets an extra attack
    public virtual bool IsDoubleFaster(UnitManager other) => GetSpeed() >= other.GetSpeed() + 4;

    // Calculates the sum of the status aliments for a given stat
    private int SumStatusAilments(Func<StatusAilments, int> selector) => statusAilments.Sum(selector);

    // If any for a certain value in statusAliment is true, will return true, otherwise false;
    private bool BoolStatusAilments(Func<StatusAilments, bool> selector) => statusAilments.Any(selector);

    // return and set stats
    public virtual UnitStats GetStats() => stats;
    public virtual void SetStats(UnitStats sta) => stats = sta;

    // Return stats variables
    public virtual int GetLevel() => stats.Level;
    public virtual string GetName() => stats.Name;
    public virtual string GetUnitName() => stats.UnitName;
    public virtual string GetClass() => stats.UnitClass;
    public virtual int GetExperience() => stats.Experience;
    public virtual int GetUnitID() => stats.UnitID;
    public virtual int GetHealthBars() => stats.HealthBars;
    public virtual string GetUnitType() => stats.UnitType + numHealthBars;

    public virtual void AddHealthBar() => numHealthBars++;

    // Returns info about the characters unit/class type
    public virtual bool GetAirBorn() => stats.AirBorn;
    public virtual bool GetArmored() => stats.Armored;
    public virtual bool GetMounted() => stats.Mounted;
    public virtual bool GetWhisper() => stats.Whisper;

    // Get the original stats from stats class with aliments, etc.
    public virtual int GetBaseHealth() => stats.Health;
    public virtual int GetBaseAttack() => stats.Attack;
    public virtual int GetBaseMagic() => stats.Magic;
    public virtual int GetBaseDefense() => stats.Defense;
    public virtual int GetBaseResistance() => stats.Resistance;
    public virtual int GetBaseEvasion() => stats.Evasion;
    public virtual int GetBaseSpeed() => stats.Speed;
    public virtual int GetBaseLuck() => stats.Luck;

    // Heal and take damage
    public virtual void TakeDamage(int health) => stats.TakeDamage(health);
    public virtual void HealUnit(int health) => stats.CurrentHealth = Mathf.Min(GetCurrentHealth() + health, stats.Health);


    // Only used for player unit
    public virtual IEnumerator ExperienceGain(int experience) { yield return null; }

    // Handles removing the extra health bars
    public virtual IEnumerator ExtraHealthBar()
    {
        // stats.HealthBars--;
        if (numHealthBars > 1) numHealthBars--;
        else stats.HealthBars--;
        
        // CanvasGroup hltBar = null;
        CanvasGroup hltBar = extraHealth1.GetComponent<CanvasGroup>();
        // if (stats.HealthBars == 1)
        // {
        //     hltBar = extraHealth1.GetComponent<CanvasGroup>();
        // }

        // if (extraHealth1 != null && extraHealth1.gameObject.activeInHierarchy)
        // {
        //     hltBar = extraHealth1.GetComponent<CanvasGroup>();
        // }


        float elapsed = 0f;

        while (elapsed < 1f && hltBar != null)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / 1f);
            hltBar.alpha = alpha;
            yield return null;
        }

        elapsed = 0f;
        float initialFillAmount = 0;

        while (elapsed < 1f)
        {

            healthBar.fillAmount = Mathf.Lerp(initialFillAmount, 1, elapsed / 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }


        healthBar.fillAmount = 1;

        HealUnit(stats.Health);

        yield return null;


    }

    // Heals the Unit but has an animation to show it healing
    public virtual IEnumerator Heal(int hlt)
    {
        float elapsed = 0f;
        float initialFillAmount = healthBar.fillAmount;

        int newHealth = hlt + GetCurrentHealth();
        if (newHealth > stats.Health)
        {
            newHealth = stats.Health;
        }

        while (elapsed < 1f)
        {

            healthBar.fillAmount = Mathf.Lerp(initialFillAmount, newHealth / stats.Health, elapsed / 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }


        healthBar.fillAmount = 1;

        HealUnit(hlt);
    }

    // Add status aliments to the list
    public virtual void AddStatusAliment(string n, string t, int a, int m, int d, int r, int s, int e, int l, int mov, bool tsd, bool freeze, int turns)
    {
        if (tsd)
        {
            foreach (StatusAilments sta in statusAilments)
            {
                if (sta.TurnStartDamage) return;
            }
        }

        Type staTemp = Type.GetType(t);

        StatusAilments tempStatus = (StatusAilments)Activator.CreateInstance(staTemp, n, t, a, m, d, r, s, e, l, mov, tsd, freeze, turns);

        statusAilments.Add(tempStatus);
    }


    // Returns both physical weapons and magical weapons
    public virtual List<Weapon> GetWeapons()
    {
        List<Weapon> temp = stats.weapons.Concat(stats.magic).ToList();

        if (stats.GetPrimaryWeapon() == null) return temp;

        temp.Remove(stats.GetPrimaryWeapon());
        temp.Insert(0, stats.GetPrimaryWeapon());

        return temp;
    }

    // Returns only physical weapons
    public virtual List<Weapon> GetPhysicalWeapons()
    {
        List<Weapon> temp = stats.weapons;

        if (stats.GetPrimaryWeapon() == null || stats.magic.Contains(stats.GetPrimaryWeapon())) return temp;

        temp.Remove(stats.GetPrimaryWeapon());
        temp.Insert(0, stats.GetPrimaryWeapon());

        return temp;
    }

    // Returns the rest of the lists
    public virtual List<Item> GetItems() => stats.items;
    public virtual List<Weapon> GetMagicList() => stats.magic;
    public virtual List<Faith> GetFaith() => stats.faith;

    // Getters and setters for primary weapons
    public virtual Weapon GetPrimaryWeapon() => stats.GetPrimaryWeapon();
    public virtual void SetPrimaryWeapon(Weapon temp) => stats.SetPrimaryWeapon(temp);

    public void IncNumberTimesActed() => numberTimesActed++;
    public void ResetNumberTimesActed() => numberTimesActed = 1;
    public int GetNumberTimesActed() => numberTimesActed;


    


    
}

