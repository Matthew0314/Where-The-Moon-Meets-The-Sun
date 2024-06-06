using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitManager
{
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

    public override int getMove() { return 0; }
    public override int getAttack() {  return 5; }

    public override int getCurrentHealth() { return currentHealth; }
    public override int getMaxHealth() { return maxHealth; }
    public override void setCurrentHealth(int health) { currentHealth = health; }
    public override string GetUnitType() { return UnitType; }

    // Additional Enemy-specific methods here
}
