using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitManager
{
    protected override void InitializeUnitData()
    {
        
        stats = UnitRosterManager.GetUnitStats(unitName);
        maxHealth = stats.Health;
        primaryWeapon = stats.GetWeaponAt(0);
        // Additional Player-specific initialization logic here
    }

    public override int getMove() { Debug.Log("Return"); return stats.getClass().Movement + stats.Movement; }
    public override int getAttack() { return primaryWeapon.Range; }

    // Additional Player-specific methods here
}
