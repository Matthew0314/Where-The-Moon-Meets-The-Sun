using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public abstract class UnitManager : MonoBehaviour
{
    protected CombatMenuManager combatMenuManager;
    public GameObject unitCircle;
    public Image healthBar;
    public Image extraHealth1;

    public UnitStats stats;
    public string UnitType { get; set; }

    [SerializeField] protected string unitName;
    public int movement;
    public int attackRange;
    public Weapon primaryWeapon;

    public int XPos { get; set; }
    public int ZPos { get; set; }

    private List<StatusAilments> statusAilments = new List<StatusAilments>();

    protected virtual void Start()
    {
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
    }

    public abstract void InitializeUnitData();


    public virtual int getMove() => stats.Movement;

    // Returns Attack if regular weapon or magic if other
    public virtual int GetAttack()
    {
        if (primaryWeapon.UseMagic)
        {
            return stats.Magic + primaryWeapon.Attack;
        }
        else
        {
            return stats.Attack + primaryWeapon.Attack;
        }


    }

    public virtual int GetDefense()
    {
        return stats.Defense;
    }

    public virtual int GetResistance()
    {
        return stats.Resistance;
    }


    public virtual int getCurrentHealth() { return stats.CurrentHealth; }
    public virtual int getMaxHealth() { return stats.Health; }
    public virtual void setCurrentHealth(int health) { }

    public virtual void TakeDamage(int health)
    {
        // currentHealth -= health;
        // if (currentHealth < 0) { currentHealth = 0; }
        stats.TakeDamage(health);
    }


    public virtual string GetUnitType() { return UnitType; }


    public virtual IEnumerator ExperienceGain(int experience)
    {
        yield return null;
    }

    public virtual int GetAttackStat()
    {
        int attack = stats.Attack;
        if (primaryWeapon != null)
        {
            attack += primaryWeapon.Attack;
        }

        return attack;
    }
    public virtual void HealUnit(int health)
    {
        int curHlt = health + getCurrentHealth();

        if (curHlt > stats.Health)
        {
            curHlt = stats.Health;
        }

        stats.CurrentHealth = curHlt;
    }



    // public virtual List<Faith> GetFaith() { return stats.faith; }

    public virtual IEnumerator ExtraHealthBar()
    {
        stats.HealthBars--;
        CanvasGroup hltBar = null;
        if (stats.HealthBars == 1)
        {
            hltBar = extraHealth1.GetComponent<CanvasGroup>();
        }

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

    public virtual IEnumerator Heal(int hlt)
    {
        float elapsed = 0f;
        float initialFillAmount = healthBar.fillAmount;

        int newHealth = hlt + getCurrentHealth();
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


    public virtual UnitStats GetStats() => stats;

    // Returns Both magic and weapons
    public virtual List<Weapon> GetWeapons() => stats.weapons.Concat(stats.magic).ToList();
    public virtual List<Item> GetItems() => stats.items;
    public virtual List<Weapon> GetMagic() => stats.magic;
    public virtual List<Faith> GetFaith() => stats.faith;
    public virtual Weapon GetPrimaryWeapon() => primaryWeapon;
    
}

