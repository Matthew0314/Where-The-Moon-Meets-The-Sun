using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverUnitMenuManager : MonoBehaviour
{

    private GameObject enemyBar;
    private GameObject playerBar;
    private GameObject hoverMenu;

    private TextMeshProUGUI levelText;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI weaponText;
    private TextMeshProUGUI unitNameText;

    // Start is called before the first frame update
    void Start()
    {
        enemyBar = GameObject.Find("Canvas/HoverUnitMenu/EnemyBar");
        playerBar = GameObject.Find("Canvas/HoverUnitMenu/PlayerBar");
        hoverMenu = GameObject.Find("Canvas/HoverUnitMenu");

        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        weaponText = GameObject.Find("WeaponText").GetComponent<TextMeshProUGUI>();
        unitNameText = GameObject.Find("UnitNameText").GetComponent<TextMeshProUGUI>();

        deactivateMenu();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deactivateMenu() {
        enemyBar.SetActive(false);
        playerBar.SetActive(false);
        hoverMenu.SetActive(false);
    }

    public void activateMenu(UnitManager unit) {

        levelText.text = $"{unit.stats.Level}";
        healthText.text = $"{unit.currentHealth} / {unit.stats.Health}";

        if (unit.primaryWeapon != null) {
            weaponText.text = unit.primaryWeapon.WeaponName;
        } else {
            weaponText.text = "";
        }

        unitNameText.text = unit.stats.Name;

        hoverMenu.SetActive(true);

        if (unit.stats.UnitType == "Player") {
            playerBar.SetActive(true);
        } else if (unit.stats.UnitType == "Enemy") {
            enemyBar.SetActive(true);
        } else {
            //Should never be called
            playerBar.SetActive(true);
        }

        
    }
}
