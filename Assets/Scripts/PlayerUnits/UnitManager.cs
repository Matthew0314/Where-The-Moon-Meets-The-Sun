using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    public Image healthBar;

    public Image extraHealth1;
    private List<StatusAilments> statusAilments = new List<StatusAilments>();

    protected virtual void Start() {
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
    }

    public virtual void InitializeUnitData() {}


    public virtual int getMove() {return stats.Movement;}

    // Returns Attack if regular weapon or magic if other
    public virtual int GetAttack() {
        if (primaryWeapon.UseMagic) {
            return stats.Magic + primaryWeapon.Attack;
        } else {
            return stats.Attack + primaryWeapon.Attack;
        }

        
    }

    public virtual int GetDefense() {
        return stats.Defense; //+ primaryWeapon.Attack;
    }

    public virtual int GetResistance() {
        return stats.Resistance;
    }


    public virtual int getCurrentHealth() { return currentHealth; }
    public virtual int getMaxHealth() { return stats.Health; }
    public virtual void setCurrentHealth(int health) {}

    public virtual void TakeDamage(int health) {
        currentHealth -= health;
        if (currentHealth < 0) { currentHealth = 0; }
    }


    public virtual string GetUnitType() { return UnitType; }


    public virtual IEnumerator ExperienceGain(int experience) {
        yield return null;
    }

    public virtual int GetAttackStat() {
        int attack = stats.Attack;
        if (primaryWeapon != null) {
            attack += primaryWeapon.Attack;
        }

        return attack;
    }
    public virtual void HealUnit(int health) {
        int curHlt = health + currentHealth;

        if (curHlt > stats.Health) {
            curHlt = stats.Health;
        }

        currentHealth = curHlt;
    }

    public virtual UnitStats GetStats() { return stats; }

    // public virtual List<Weapon> GetWeapons() { return stats.weapons; }
    public virtual List<Weapon> GetWeapons() => stats.weapons.Concat(stats.magic).ToList();
    public virtual List<Item> GetItems() { return stats.items; }
    public virtual List<Weapon> GetMagic() { return stats.magic; }
    public virtual List<Faith> GetFaith() => stats.faith;

    public virtual Weapon GetPrimaryWeapon() { return primaryWeapon; }
    // public virtual List<Faith> GetFaith() { return stats.faith; }

    public virtual IEnumerator ExtraHealthBar() {
        stats.HealthBars--;
        CanvasGroup hltBar = null;
        if (stats.HealthBars == 1) {
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

    public virtual IEnumerator Heal(int hlt) {
        float elapsed = 0f;
        float initialFillAmount = healthBar.fillAmount;

        int newHealth = hlt + currentHealth;
        if(newHealth > stats.Health) {
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
}

