using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpectedBattleMenu : MonoBehaviour
{   

    private Image EHealthLost;
    private Image EHealth;
    private Image PHealthLost;
    private Image PHealth;
    private GameObject Menu;

    private TextMeshProUGUI PlayerName;
    private TextMeshProUGUI PlayerWeapon;
    private TextMeshProUGUI PlayerDamage;
    private TextMeshProUGUI PlayerHit;
    private TextMeshProUGUI PlayerCrit;
    private TextMeshProUGUI PlayerCurrHealth;

    private TextMeshProUGUI EnemyName;
    private TextMeshProUGUI EnemyWeapon;
    private TextMeshProUGUI EnemyDamage;
    private TextMeshProUGUI EnemyHit;
    private TextMeshProUGUI EnemyCrit;
    private TextMeshProUGUI EnemyCurrHealth;

    [SerializeField] GameObject HPIndicator;
    GameObject HPplayer;
    GameObject HPenemy;

    // Start is called before the first frame update
    void Start()
    {
        EHealthLost = GameObject.Find("EHealthLostBar").GetComponent<Image>();
        EHealth = GameObject.Find("EnemyHealthBar").GetComponent<Image>();
        PHealthLost = GameObject.Find("PHealthLostBar").GetComponent<Image>();
        PHealth = GameObject.Find("PlayerHealthBar").GetComponent<Image>();
        Menu = GameObject.Find("Canvas/ExpectedBattleMenu");

        //Canvas/ExpectBattleMenu/Text/Player/
        PlayerName = GameObject.Find("PlayerUnitName").GetComponent<TextMeshProUGUI>();
        PlayerWeapon = GameObject.Find("PlayerWeapon").GetComponent<TextMeshProUGUI>();
        PlayerDamage = GameObject.Find("PlayerDamage").GetComponent<TextMeshProUGUI>();
        PlayerHit = GameObject.Find("PlayerHit").GetComponent<TextMeshProUGUI>();
        PlayerCrit = GameObject.Find("PlayerCrit").GetComponent<TextMeshProUGUI>();
        PlayerCurrHealth = GameObject.Find("PlayerCurrentHealth").GetComponent<TextMeshProUGUI>();

        //Canvas/ExpectBattleMenu/Text/Enemy/
        EnemyName = GameObject.Find("EnemyUnitName").GetComponent<TextMeshProUGUI>();
        EnemyWeapon = GameObject.Find("EnemyWeapon").GetComponent<TextMeshProUGUI>();
        EnemyDamage = GameObject.Find("EnemyDamage").GetComponent<TextMeshProUGUI>();
        EnemyHit = GameObject.Find("EnemyHit").GetComponent<TextMeshProUGUI>();
        EnemyCrit = GameObject.Find("EnemyCrit").GetComponent<TextMeshProUGUI>();
        EnemyCurrHealth = GameObject.Find("EnemyCurrentHealth").GetComponent<TextMeshProUGUI>();

        DeactivateMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpMenu(UnitManager player, UnitManager enemy, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {
        int playerHit = player.primaryWeapon.HitRate + (player.stats.Luck * 2) - enemy.stats.Evasion;
        int enemyHit = enemy.primaryWeapon.HitRate + (enemy.stats.Luck * 2) - player.stats.Evasion;
        int playerCrit = player.primaryWeapon.CritRate + (int)(player.stats.Luck / 2);
        int enemyCrit = enemy.primaryWeapon.CritRate + (int)(enemy.stats.Luck / 2);

        Debug.Log(expectedEnemyHP);

        if (playerHit > 100) {
            playerHit = 100;
        } else if (playerHit < 0) {
            playerHit = 0;
        }

        if (playerCrit > 100) {
            playerCrit = 100;
        } else if (playerCrit < 0) {
            playerCrit = 0;
        }
        if (enemyHit > 100) {
            enemyHit = 100;
        } else if (enemyHit < 0) {
            enemyHit = 0;
        }
        if (enemyCrit > 100) {
            enemyCrit = 100;
        } else if (enemyCrit < 0) {
            enemyCrit = 0;
        }

        PlayerName.text = player.stats.Name;
        PlayerWeapon.text = player.primaryWeapon.WeaponName;
        
        if (numPHits > 1) {
            PlayerDamage.text = $"{PDamage} x {numPHits}";
            PlayerHit.text = $"{playerHit}%";
            PlayerCrit.text = $"{playerCrit}%";
        } else if (numPHits == 0) {
            PlayerDamage.text = "-";
            PlayerHit.text = "-";
            PlayerCrit.text = "-";
        } else {
            PlayerDamage.text = $"{PDamage}";
            PlayerHit.text = $"{playerHit}%";
            PlayerCrit.text = $"{playerCrit}%";
        }

        PlayerCurrHealth.text = $"{player.currentHealth}";

        if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
        if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

        float PLostFill = ((float)expectedPlayerHP / (float)player.stats.Health);
        float PCurrFill = ((float)player.currentHealth / (float)player.stats.Health);

        PHealth.fillAmount = PLostFill;
        PHealthLost.fillAmount = PCurrFill;

        Destroy(HPplayer);

        float pXPos = (290.0f * (1.0f - PLostFill)) - 217.0f + 960f;
        HPplayer = Instantiate(HPIndicator, new Vector3(pXPos, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
        TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
        plaText.text = $"{expectedPlayerHP}";



        EnemyName.text = enemy.stats.Name;
        EnemyWeapon.text = enemy.primaryWeapon.WeaponName;
        
        if (numEHits > 1) {
            EnemyDamage.text = $"{EDamage} x {numEHits}";
            EnemyHit.text = $"{enemyHit}%";
            EnemyCrit.text = $"{enemyCrit}%";
        } else if (numEHits == 0) {
            EnemyDamage.text = "-";
            EnemyHit.text = "-";
            EnemyCrit.text = "-";
        } else {
            EnemyDamage.text = $"{EDamage}";
            EnemyHit.text = $"{enemyHit}%";
            EnemyCrit.text = $"{enemyCrit}%";
        }

        // EnemyHit.text = $"{enemyHit}";
        // EnemyCrit.text = $"{enemyCrit}";
        EnemyCurrHealth.text = $"{enemy.currentHealth}";

        float ELostFill = ((float)expectedEnemyHP / (float)enemy.stats.Health);
        float ECurrFill = ((float)enemy.currentHealth / (float)enemy.stats.Health);

        Destroy(HPenemy);

        float eXPos = (291.0f * (float)ELostFill) + 140.0f + 960f;
        HPenemy = Instantiate(HPIndicator, new Vector3(eXPos, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject eneChild = HPenemy.transform.GetChild(0).gameObject;
        TextMeshProUGUI eneText = eneChild.GetComponent<TextMeshProUGUI>();
        eneText.text = $"{expectedEnemyHP}";

        EHealth.fillAmount = ELostFill;
        EHealthLost.fillAmount = ECurrFill;

        Menu.SetActive(true);
    }

    public void DeactivateMenu() {
        Menu.SetActive(false);
    }
}
