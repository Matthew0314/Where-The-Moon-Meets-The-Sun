using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : UnitManager
{
    private Image healthBar;
    private Material originalMaterial;
    private TurnManager turnManager;
    [SerializeField] private Material grayscaleMaterial;

    protected override void Start() {
        // classList = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        InitializeUnitData();
        Transform childImage = transform.Find("PlayerCircle/Canvas/UnitBar");
        healthBar = childImage.GetComponent<Image>();
        unitCircle = transform.Find("PlayerCircle").gameObject;
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
        originalMaterial = healthBar.material;
        turnManager = GameObject.Find("GridManager").GetComponent<TurnManager>();

    }

    protected void Update() {
        healthBar.fillAmount = (float)currentHealth / (float)stats.Health; 

        if (!turnManager.IsActive(this.stats))
        {
            // Apply grayscale material
            healthBar.material = grayscaleMaterial;

            // Adjust grayscale intensity (fully grayscale in this example)
            healthBar.material.SetFloat("_GrayAmount", 1f);
        }
        else
        {
            // Reset to the original material
            healthBar.material = originalMaterial;
        }
    }



    public override void InitializeUnitData()
    {
        
        stats = UnitRosterManager.GetUnitStats(unitName);
        maxHealth = stats.Health;
        currentHealth = maxHealth;
        primaryWeapon = stats.GetWeaponAt(0);
        Debug.Log(stats.UnitName + " Has been initlialized");

    }

    public override int getMove() { return stats.getClass().Movement + stats.Movement; }
    public override int getAttack() { return primaryWeapon.Range; }

    public override int getCurrentHealth() { return currentHealth; }
    public override int getMaxHealth() { return stats.Health; }
    public override void setCurrentHealth(int health) { currentHealth = health; }
    public override string GetUnitType() { return UnitType; }

}
