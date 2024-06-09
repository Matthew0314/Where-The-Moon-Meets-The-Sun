using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : UnitManager
{
    private Image healthBar;

    protected override void Start() {
        classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        InitializeUnitData();
        Transform childImage = transform.Find("PlayerCircle/Canvas/UnitBar");
        healthBar = childImage.GetComponent<Image>();

    }

    protected void Update() {
        healthBar.fillAmount = (float)currentHealth / (float)stats.Health; 
    }



    public override void InitializeUnitData()
    {
        
        stats = UnitRosterManager.GetUnitStats(unitName);
        maxHealth = stats.Health;
        currentHealth = maxHealth;
        primaryWeapon = stats.GetWeaponAt(0);
        Debug.Log(stats.UnitName + " Has been initlialized");

        // Additional Player-specific initialization logic here
    }

    public override int getMove() { Debug.Log("Return"); return stats.getClass().Movement + stats.Movement; }
    public override int getAttack() { return primaryWeapon.Range; }

    public override int getCurrentHealth() { return currentHealth; }
    public override int getMaxHealth() { return stats.Health; }
    public override void setCurrentHealth(int health) { currentHealth = health; }
    public override string GetUnitType() { return UnitType; }

    // Additional Player-specific methods here
}
