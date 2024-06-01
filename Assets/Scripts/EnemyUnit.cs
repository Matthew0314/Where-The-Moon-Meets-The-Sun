using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : UnitManager
{
    protected override void InitializeUnitData()
    {
        // Initialize Enemy-specific data here
    }

    public override int getMove() { return 0; }
    public override int getAttack() {  return 5; }

    // Additional Enemy-specific methods here
}
