using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : UnitManager
{

    protected override void Start() {
        
        Transform childImage = transform.Find("EnemyCircle/Canvas/UnitBar");
        unitCircle = transform.Find("EnemyCircle").gameObject;
        extraHealth1 = transform.Find("EnemyCircle/Canvas/UnitCircle/ExtraHealth1").GetComponent<Image>();
        healthBar = transform.Find("EnemyCircle/Canvas/UnitBar").GetComponent<Image>();

    }

    protected void Update() {
       
        healthBar.fillAmount = (float)GetCurrentHealth() / (float)GetHealth(); 
        
    }

    public override void InitializeUnitData()
    {
        // maxHealth = stats.Health;
        // currentHealth = maxHealth;
        stats.SetPrimaryWeapon(stats.GetWeaponAt(0));
        // Debug.LogWarning(stats.Name);
        extraHealth1 = transform.Find("EnemyCircle/Canvas/UnitCircle/ExtraHealth1").GetComponent<Image>();
        if(stats.HealthBars < 2) 
        {
            extraHealth1.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        }
        UnitType = "Enemy";
    }

    // public override int getMove() { return stats.Movement; }

    // public override int getMaxHealth() { return stats.Health; }
    // public override void setCurrentHealth(int health) { stats.CurrentHealth = health; }
    public override string GetUnitType() { return UnitType; }

    public override int GetMove() => Mathf.Max(0, base.GetMove());
    // public int GetEnemyID() => stats.EnemyID;

    // Additional Enemy-specific methods here
}
