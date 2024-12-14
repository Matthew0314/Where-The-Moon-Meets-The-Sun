using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : UnitManager
{

    // private Image healthBar;
   

    protected override void Start() {
        
        Transform childImage = transform.Find("EnemyCircle/Canvas/UnitBar");
        unitCircle = transform.Find("EnemyCircle").gameObject;
        healthBar = childImage.GetComponent<Image>();

    }

    protected void Update() {
       
            healthBar.fillAmount = (float)currentHealth / (float)getMaxHealth(); 
        
    }

    public override void InitializeUnitData()
    {
        // stats = UnitRosterManager.GetUnitStats(unitName);
        maxHealth = stats.Health;
        currentHealth = maxHealth;
        primaryWeapon = stats.GetWeaponAt(0);
        UnitType = "Enemy";
        Debug.Log(stats.UnitName + " Has been initlialized");
        // Initialize Enemy-specific data here
    }

    public override int getMove() { return stats.Movement; }
    public override int getAttack() {  
        if (primaryWeapon != null) {
            return primaryWeapon.Range; 
        } else { return 0; }
        
    }

    public override int getCurrentHealth() { return currentHealth; }
    public override int getMaxHealth() { return stats.Health; }
    public override void setCurrentHealth(int health) { currentHealth = health; }
    public override string GetUnitType() { return UnitType; }

    // Additional Enemy-specific methods here
}
