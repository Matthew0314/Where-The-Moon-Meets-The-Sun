using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatMenuManager : MonoBehaviour
{
    private IMaps _currentMap;
    private TurnManager manageTurn;    
    private PlayerGridMovement moveGrid;
    private GenerateGrid generateGrid;
    private ExecuteAction executeAction;
    // private UnitManager DefendingEnemy;
    // private UnitManager AttackingUnit;
    // public Transform moveCursor;
    // private int attackerX;
    // private int attackerZ;
    // private int defenderX;
    // private int defenderZ;

    //Action Menu
    private GameObject attackButton;
    private GameObject itemButton;
    private GameObject waitButton;


    //hover menu
    private GameObject enemyBar;
    private GameObject playerBar;
    private GameObject hoverMenu;

    private TextMeshProUGUI levelText;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI weaponText;
    private TextMeshProUGUI unitNameText;


    //Expected Menu
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

    private Image EHealthSwapped;
    private Image PHealthSwapped;

    //Experience Menu

    private GameObject experienceMenu;
    private Image expBar;
    private TextMeshProUGUI expUnitName;
    private TextMeshProUGUI expNext;

    private TextMeshProUGUI expGained;


    [SerializeField] GameObject HPIndicator;
    GameObject HPplayer;
    GameObject HPenemy;




    //Level Up Menu

    private GameObject levelUpMenu;
    private TextMeshProUGUI lvName;
    private TextMeshProUGUI lvClass;
    private TextMeshProUGUI lvLevel;
    private TextMeshProUGUI lvHP;
    private TextMeshProUGUI lvStr;
    private TextMeshProUGUI lvMag;
    private TextMeshProUGUI lvSpd;
    private TextMeshProUGUI lvDef;
    private TextMeshProUGUI lvRes;
    private TextMeshProUGUI lvEva;
    private TextMeshProUGUI lvLck;
    private TextMeshProUGUI lvHPGR;
    private TextMeshProUGUI lvStrGR;
    private TextMeshProUGUI lvMagGR;
    private TextMeshProUGUI lvSpdGR;
    private TextMeshProUGUI lvDefGR;
    private TextMeshProUGUI lvResGR;
    private TextMeshProUGUI lvEvaGR;
    private TextMeshProUGUI lvLckGR;

    // Start is called before the first frame update
    void Start()
    {
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        moveGrid = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        executeAction = GameObject.Find("Player").GetComponent<ExecuteAction>();

        //Action Menu
        attackButton = GameObject.Find("Canvas/AttackButton");
        itemButton = GameObject.Find("Canvas/WaitButton");
        waitButton = GameObject.Find("Canvas/ItemButton");

        DeactivateActionMenu();

        //Hover Menu
        enemyBar = GameObject.Find("Canvas/HoverUnitMenu/EnemyBar");
        playerBar = GameObject.Find("Canvas/HoverUnitMenu/PlayerBar");
        hoverMenu = GameObject.Find("Canvas/HoverUnitMenu");

        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        weaponText = GameObject.Find("WeaponText").GetComponent<TextMeshProUGUI>();
        unitNameText = GameObject.Find("UnitNameText").GetComponent<TextMeshProUGUI>();

        DeactivateHoverMenu();

        //Expected Menu
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

        PHealthSwapped = GameObject.Find("Canvas/ExpectedBattleMenu/PHealthBarSwapped").GetComponent<Image>();
        EHealthSwapped = GameObject.Find("Canvas/ExpectedBattleMenu/EHealthBarSwapped").GetComponent<Image>();

        PHealthSwapped.gameObject.SetActive(false);
        EHealthSwapped.gameObject.SetActive(false);

        DeactivateExpectedMenu();


        experienceMenu = GameObject.Find("Canvas/ExperienceMenu");
        expBar = GameObject.Find("Canvas/ExperienceMenu/ExpBar").GetComponent<Image>();
        expUnitName = GameObject.Find("Canvas/ExperienceMenu/UnitName").GetComponent<TextMeshProUGUI>();
        expNext = GameObject.Find("Canvas/ExperienceMenu/ExpNext").GetComponent<TextMeshProUGUI>();
        expGained = GameObject.Find("Canvas/ExperienceMenu/ExpGained").GetComponent<TextMeshProUGUI>();

        DeactivateExperienceMenu();

        levelUpMenu = GameObject.Find("Canvas/LevelUpMenu");
        lvName = GameObject.Find("Canvas/LevelUpMenu/LvName").GetComponent<TextMeshProUGUI>();
        lvClass = GameObject.Find("Canvas/LevelUpMenu/LvClass").GetComponent<TextMeshProUGUI>();
        lvLevel = GameObject.Find("Canvas/LevelUpMenu/LvLevel").GetComponent<TextMeshProUGUI>();

        lvHP = GameObject.Find("Canvas/LevelUpMenu/LvHP").GetComponent<TextMeshProUGUI>();
        lvStr = GameObject.Find("Canvas/LevelUpMenu/LvStr").GetComponent<TextMeshProUGUI>();
        lvMag = GameObject.Find("Canvas/LevelUpMenu/LvMag").GetComponent<TextMeshProUGUI>();
        lvSpd = GameObject.Find("Canvas/LevelUpMenu/LvSpd").GetComponent<TextMeshProUGUI>();
        lvDef = GameObject.Find("Canvas/LevelUpMenu/LvDef").GetComponent<TextMeshProUGUI>();
        lvRes = GameObject.Find("Canvas/LevelUpMenu/LvRes").GetComponent<TextMeshProUGUI>();
        lvEva = GameObject.Find("Canvas/LevelUpMenu/LvEva").GetComponent<TextMeshProUGUI>();
        lvLck = GameObject.Find("Canvas/LevelUpMenu/LvLck").GetComponent<TextMeshProUGUI>();

        lvHPGR = GameObject.Find("Canvas/LevelUpMenu/LvHPGR").GetComponent<TextMeshProUGUI>();
        lvStrGR = GameObject.Find("Canvas/LevelUpMenu/LvStrGR").GetComponent<TextMeshProUGUI>();
        lvMagGR = GameObject.Find("Canvas/LevelUpMenu/LvMagGR").GetComponent<TextMeshProUGUI>();
        lvSpdGR = GameObject.Find("Canvas/LevelUpMenu/LvSpdGR").GetComponent<TextMeshProUGUI>();
        lvDefGR = GameObject.Find("Canvas/LevelUpMenu/LvDefGR").GetComponent<TextMeshProUGUI>();
        lvResGR = GameObject.Find("Canvas/LevelUpMenu/LvResGR").GetComponent<TextMeshProUGUI>();
        lvEvaGR = GameObject.Find("Canvas/LevelUpMenu/LvEvaGR").GetComponent<TextMeshProUGUI>();
        lvLckGR = GameObject.Find("Canvas/LevelUpMenu/LvLckGR").GetComponent<TextMeshProUGUI>();

        DeactivateLevelUpMenu();



    }

    public void ActivateActionMenu() {
        attackButton.SetActive(true);
        waitButton.SetActive(true);
        itemButton.SetActive(true);
    }

    public void DeactivateActionMenu() {
        attackButton.SetActive(false);
        waitButton.SetActive(false);
        itemButton.SetActive(false);
    }


    public void PlayerWait() {
        
        manageTurn.RemovePlayer(moveGrid.playerCollide.GetPlayer().stats);
        executeAction.unitWait();
        manageTurn.CheckPhase();
        _currentMap.CheckClearCondition();
        
        
    }

    public void PlayerAttack() {

        Debug.Log("Attack");
        moveGrid.isAttacking = true;
        List<GridTile> UnitsInRange = new List<GridTile>();
        
        
        for (int i = 0; i < generateGrid.GetWidth(); i++) {
            for (int j = 0; j < generateGrid.GetLength(); j++) {
                if (executeAction.attackGrid[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                    
                    UnitsInRange.Add(generateGrid.GetGridTile(i,j));
                    
                }
            }
        }

        if (UnitsInRange.Count == 0) {
            moveGrid.isAttacking = false;
            return;
        }

        DeactivateActionMenu();

        StartCoroutine(executeAction.CycleAttackList(UnitsInRange));

    }


//---------------------------------------Hover Menu---------------------------------------------------//

    public void DeactivateHoverMenu() {
        enemyBar.SetActive(false);
        playerBar.SetActive(false);
        hoverMenu.SetActive(false);
    }

    public void ActivateHoverMenu(UnitManager unit) {

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

//-------------------------------------Expected Menu--------------------------------------------------//

public void SetUpExpectedMenu(UnitManager player, UnitManager enemy, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {
        PHealthSwapped.gameObject.SetActive(false);
        EHealthSwapped.gameObject.SetActive(false);
        int playerHit = player.primaryWeapon.HitRate + (player.stats.Luck * 4) - enemy.stats.Evasion;
        int enemyHit = enemy.primaryWeapon.HitRate + (enemy.stats.Luck * 4) - player.stats.Evasion;
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

    public void DeactivateExpectedMenu() {
        Menu.SetActive(false);
    }

//------------------------------------------------Experience Menu-------------------------------------------------------------

    public void DeactivateExperienceMenu() {
        experienceMenu.SetActive(false);
    }

    // public void ActivateExperienceMenu() {
    //     experienceMenu.SetActive(true);
    // }

    public IEnumerator GainExperienceMenu(UnitManager unit, int gainExp)
    {
        experienceMenu.SetActive(true);
        expUnitName.text = unit.stats.Name;

        int currentExp = unit.stats.Experience; // Starting experience
        int remainingExp = gainExp;             // Experience to be added
        int expThreshold = 100;                 // Max experience for a level
        int initialExpNext = expThreshold - currentExp; // Initial experience needed to level up

        expGained.text = "+" + remainingExp.ToString();
        expNext.text = initialExpNext.ToString();

        while (remainingExp > 0)
        {
            // Calculate how much experience can be added in this cycle
            int expToAdd = Mathf.Min(expThreshold - currentExp, remainingExp);
            float startFill = (float)currentExp / expThreshold;
            float targetFill = (float)(currentExp + expToAdd) / expThreshold;

            float duration = (targetFill - startFill) * 1.5f;
            float elapsed = 0f;

            expBar.fillAmount = startFill;

            // Animate the bar fill
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Update bar fill
                expBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

                // Dynamically update expGained and expNext
                int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(remainingExp, remainingExp - expToAdd, t));
                int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(initialExpNext, initialExpNext - expToAdd, t));

                expGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : interpolatedGain.ToString();
                expNext.text = interpolatedNext.ToString();

                yield return null; // Wait for the next frame
            }

            // Finalize this cycle
            expBar.fillAmount = targetFill;
            currentExp += expToAdd;    // Temporarily track current experience for UI
            remainingExp -= expToAdd; // Reduce remaining experience

            // Reset bar if threshold is reached
            if (currentExp >= expThreshold && remainingExp > 0)
            {
                currentExp = 0;        // Reset current experience
                initialExpNext = expThreshold; // Reset expNext display
            }
        }

        // Set final UI state
        expGained.text = "0"; // Clear gained experience
        expNext.text = (expThreshold - currentExp).ToString(); // Remaining experience for next level

        yield return new WaitForSeconds(1f);
        DeactivateExperienceMenu();
    }


// -----------------------------------------------Level Up Menu------------------------------------------------------

    public void DeactivateLevelUpMenu() {
        lvHPGR.gameObject.SetActive(false);
        lvStrGR.gameObject.SetActive(false);
        lvMagGR.gameObject.SetActive(false);
        lvSpdGR.gameObject.SetActive(false);
        lvDefGR.gameObject.SetActive(false);
        lvResGR.gameObject.SetActive(false);
        lvEvaGR.gameObject.SetActive(false);
        lvLckGR.gameObject.SetActive(false);
        levelUpMenu.SetActive(false);
    }

    public IEnumerator LevelUpMenu(UnitManager unit, int hp, int str, int mag, int spd, int def, int res, int eva, int lck)
    {
        // Activate the level-up menu
        levelUpMenu.SetActive(true);

        // Set initial values for the unit's stats
        lvName.text = unit.stats.Name;
        lvClass.text = unit.stats.UnitClass;
        lvLevel.text = unit.stats.Level.ToString();

        int HP = unit.stats.Health;
        int Str = unit.stats.Attack;
        int Mag = unit.stats.Magic;
        int Spd = unit.stats.Speed;
        int Def = unit.stats.Defense;
        int Res = unit.stats.Resistance;
        int Eva = unit.stats.Evasion;
        int Lck = unit.stats.Luck;

        // Display current stats
        lvHP.text = HP.ToString();
        lvStr.text = Str.ToString();
        lvMag.text = Mag.ToString();
        lvSpd.text = Spd.ToString();
        lvDef.text = Def.ToString();
        lvRes.text = Res.ToString();
        lvEva.text = Eva.ToString();
        lvLck.text = Lck.ToString();

        float waitTime = 0.8f;

        yield return StartCoroutine(AnimateLevelText(lvLevel, unit.stats.Level));

        // Update stats incrementally and display changes
        if (hp > 0)
        {
            lvHPGR.text = "+" + hp.ToString();
            HP += hp;
            lvHP.text = HP.ToString();
            lvHPGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        if (str > 0)
        {
            lvStrGR.text = "+" + str.ToString();
            Str += str;
            lvStr.text = Str.ToString();
            lvStrGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        if (mag > 0)
        {
            lvMagGR.text = "+" + mag.ToString();
            Mag += mag;
            lvMag.text = Mag.ToString();
            lvMagGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        if (spd > 0)
        {
            lvSpdGR.text = "+" + spd.ToString();
            Spd += spd;
            lvSpd.text = Spd.ToString();
            lvSpdGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        if (def > 0)
        {
            lvDefGR.text = "+" + def.ToString();
            Def += def;
            lvDef.text = Def.ToString();
            lvDefGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        if (res > 0)
        {
            lvResGR.text = "+" + res.ToString();
            Res += res;
            lvRes.text = Res.ToString();
            lvResGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        if (eva > 0)
        {
            lvEvaGR.text = "+" + eva.ToString();
            Eva += eva;
            lvEva.text = Eva.ToString();
            lvEvaGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        if (lck > 0)
        {
            lvLckGR.text = "+" + lck.ToString();
            Lck += lck;
            lvLck.text = Lck.ToString();
            lvLckGR.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
        }

        // Wait before deactivating the menu
        yield return new WaitForSeconds(1f);

        DeactivateLevelUpMenu();
    }

    private IEnumerator AnimateLevelText(TextMeshProUGUI levelText, int level)
    {
        Vector3 originalScale = levelText.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f; // Increase size by 50%
        float animationTime = 0.5f; // Time for animation
        float elapsedTime = 0f;

        // Scale up
        while (elapsedTime < animationTime)
        {
            levelText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }



        // Ensure it's at the target scale
        levelText.transform.localScale = targetScale;

        int newLevel = level + 1;
        levelText.text = newLevel.ToString();

        

        // Reset timer for scale down
        elapsedTime = 0f;

        // Scale down
        while (elapsedTime < animationTime)
        {
            levelText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's back to the original scale
        levelText.transform.localScale = originalScale;
    }


//---------------------------------------Battle Menu----------------------------------//


    public IEnumerator BattleMenu(UnitManager player, UnitManager enemy, bool playerOnLeft, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {

        Image pBar;
        Image eBar;
        Image pLostBar;
        Image eLostBar;

        if (playerOnLeft) {
            pBar = PHealth;
            eBar = EHealth;
            pLostBar = PHealthLost;
            eLostBar = EHealthLost;
        } else {
            pBar = PHealthSwapped;
            eBar = EHealthSwapped;
            pLostBar = PHealthLost;
            eLostBar = EHealthLost;
        }

        if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
        if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

        // PHealthSwapped.gameObject.SetActive(false);
        // EHealthSwapped.gameObject.SetActive(false);

        UnitManager leftUnit = playerOnLeft ? player : enemy;
        UnitManager rightUnit = playerOnLeft ? enemy : player;

        int leftHit = leftUnit.primaryWeapon.HitRate + (leftUnit.stats.Luck * 4) - rightUnit.stats.Evasion;
        int rightHit = rightUnit.primaryWeapon.HitRate + (rightUnit.stats.Luck * 4) - leftUnit.stats.Evasion;
        int leftCrit = leftUnit.primaryWeapon.CritRate + (int)(leftUnit.stats.Luck / 2);
        int rightCrit = rightUnit.primaryWeapon.CritRate + (int)(rightUnit.stats.Luck / 2);

        // Clamp hit and crit rates
        leftHit = Mathf.Clamp(leftHit, 0, 100);
        leftCrit = Mathf.Clamp(leftCrit, 0, 100);
        rightHit = Mathf.Clamp(rightHit, 0, 100);
        rightCrit = Mathf.Clamp(rightCrit, 0, 100);

        // Left Unit (Player or Enemy based on position)
        PlayerName.text = leftUnit.stats.Name;
        PlayerWeapon.text = leftUnit.primaryWeapon.WeaponName;
        if (numPHits > 1) {
            PlayerDamage.text = $"{PDamage} x {numPHits}";
            PlayerHit.text = $"{leftHit}%";
            PlayerCrit.text = $"{leftCrit}%";
        } else if (numPHits == 0) {
            PlayerDamage.text = "-";
            PlayerHit.text = "-";
            PlayerCrit.text = "-";
        } else {
            PlayerDamage.text = $"{PDamage}";
            PlayerHit.text = $"{leftHit}%";
            PlayerCrit.text = $"{leftCrit}%";
        }
        PlayerCurrHealth.text = $"{leftUnit.currentHealth}";

        float leftLostFill = (float)expectedPlayerHP / leftUnit.stats.Health;
        float leftCurrFill = (float)leftUnit.currentHealth / leftUnit.stats.Health;
        pBar.fillAmount = leftLostFill;
        pLostBar.fillAmount = leftCurrFill;

        Destroy(HPplayer);
        float leftXPos = (290.0f * (1.0f - leftLostFill)) - 217.0f + 960f;
        HPplayer = Instantiate(HPIndicator, new Vector3(leftXPos, 540f, 0f), Quaternion.identity, Menu.transform);
        GameObject leftChild = HPplayer.transform.GetChild(0).gameObject;
        TextMeshProUGUI leftText = leftChild.GetComponent<TextMeshProUGUI>();
        leftText.text = $"{expectedPlayerHP}";

        // Right Unit (Enemy or Player based on position)
        EnemyName.text = rightUnit.stats.Name;
        EnemyWeapon.text = rightUnit.primaryWeapon.WeaponName;
        if (numEHits > 1) {
            EnemyDamage.text = $"{EDamage} x {numEHits}";
            EnemyHit.text = $"{rightHit}%";
            EnemyCrit.text = $"{rightCrit}%";
        } else if (numEHits == 0) {
            EnemyDamage.text = "-";
            EnemyHit.text = "-";
            EnemyCrit.text = "-";
        } else {
            EnemyDamage.text = $"{EDamage}";
            EnemyHit.text = $"{rightHit}%";
            EnemyCrit.text = $"{rightCrit}%";
        }
        EnemyCurrHealth.text = $"{rightUnit.currentHealth}";

        float rightLostFill = (float)expectedEnemyHP / rightUnit.stats.Health;
        float rightCurrFill = (float)rightUnit.currentHealth / rightUnit.stats.Health;
        eBar.fillAmount = rightLostFill;
        eLostBar.fillAmount = rightCurrFill;

        Destroy(HPenemy);
        float rightXPos = (291.0f * rightLostFill) + 140.0f + 960f;
        HPenemy = Instantiate(HPIndicator, new Vector3(rightXPos, 540f, 0f), Quaternion.identity, Menu.transform);
        GameObject rightChild = HPenemy.transform.GetChild(0).gameObject;
        TextMeshProUGUI rightText = rightChild.GetComponent<TextMeshProUGUI>();
        rightText.text = $"{expectedEnemyHP}";

        Menu.SetActive(true);

        yield return null;
        // yield return new WaitForSeconds(1f);
    }

}


































 /*public IEnumerator CycleAttackList() {
        bool isAttacking = false;
        int currentIndex = 0;
        AttackingUnit = moveGrid.playerCollide.GetPlayer();
        attackerX = moveGrid.GetCurX();
        attackerZ = moveGrid.GetCurZ();

        while(true) {

            Vector3 currentPosition = moveGrid.moveCursor.transform.position;
            moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            if (Input.GetKeyDown(KeyCode.Space)) {
                DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
                
                isAttacking = true;
                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                Debug.Log("Hello");
                //Go to another IEnumerator to show attacking stats
                break;
            }

            if (Input.GetKeyDown(KeyCode.B)) {
                Debug.Log("Hello");
                break;
            }

            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= .15) {
                float rawHorizontalInput = Input.GetAxis("Horizontal");

                // Determine the sign of the input
                float horizontalSign = Mathf.Sign(rawHorizontalInput);

                // Round down to -1 if negative, round up to 1 if positive
                int horizontalInput = (int)Mathf.Ceil(horizontalSign);

                // Move through the list based on the horizontal input
                if (horizontalInput > 0)
                {
                    // Move up in the list
                    currentIndex++;
                
                    if (currentIndex >= UnitsInRange.Count)
                    {
                        currentIndex = 0; // Wrap around to the start
                        
                    }
                    
                }
                else if (horizontalInput < 0)
                {
                    // Move down in the list
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = UnitsInRange.Count - 1; // Wrap around to the end
                    }
                    
                
                }

                Debug.Log("Index Changed");

                currentPosition = moveGrid.moveCursor.transform.position;
                moveGrid.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());


                yield return new WaitForSeconds(0.5f);
            }
            
           
            // Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            // moveCursor.transform.position = Vector3.MoveTowards(moveCursor.transform.position, targetPosition, 30.0f * Time.deltaTime);

            // Move the cursor towards the target position using interpolation
            // moveGrid.moveCursor.position = Vector3.Lerp(moveGrid.moveCursor.position, targetPosition, 20.0f * Time.deltaTime);
            
            // currentPosition = moveGrid.moveCursor.transform.position;
            // moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            

            yield return null;
        }

        Debug.Log("Broke Free");

        if (isAttacking) {
            //Start Attacking based on primary weapons
            Debug.Log(DefendingEnemy.primaryWeapon.WeaponName);
            Debug.Log(AttackingUnit.primaryWeapon.WeaponName);
            AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, attackerX, attackerZ, defenderX, defenderZ);
            Debug.Log(AttackingUnit.stats.UnitName);
            moveGrid.moveCursor.position = new Vector3(generateGrid.GetGridTile(attackerX, attackerZ).GetXPos(), generateGrid.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, generateGrid.GetGridTile(attackerX, attackerZ).GetZPos());
            manageTurn.RemovePlayer(AttackingUnit.stats);
            moveGrid.ResetAfterAction(AttackingUnit);
            manageTurn.CheckPhase();
            _currentMap.CheckClearCondition();
        }

        moveGrid.isAttacking = false;

        yield return null;
    }*/


    // public void unitAttack(Queue<UnitManager> attacking, Queue<UnitManager> defending) {
    //     int queueSize = attacking.Count;
    //     for (int i = 0; i < queueSize; i++) {
    //         UnitManager atk = attacking.Dequeue();
    //         UnitManager def = defending.Dequeue();

    //         int damage = atk.stats.Attack + atk.primaryWeapon.Attack - def.stats.Defense;

    //         float multiplier = 1;

    //         if (def.stats.Mounted) {
    //             multiplier += atk.primaryWeapon.MultMounted - 1; 
    //         }
    //         if (def.stats.AirBorn) {
    //             multiplier += atk.primaryWeapon.MultAirBorn - 1; 
    //         }
    //         if (def.stats.Armored) {
    //             multiplier += atk.primaryWeapon.MultArmored - 1; 
    //         }
    //         if (def.stats.Whisper) {
    //             multiplier += atk.primaryWeapon.MultWhisper - 1; 
    //         }

    //         Debug.Log("defender current health " + def.currentHealth + " " + def.stats.Health);

    //         damage = (int)(damage * multiplier);

    //         Debug.Log(atk.stats.UnitName + " Did" + damage + " damage to " + def.stats.UnitName);

    //         def.currentHealth -= damage;

    //         Debug.Log("defender current health " + def.currentHealth + " " + def.stats.Health);

    //         if (def.currentHealth <= 0) {
    //             Debug.Log(def.stats.UnitName + "Has died");
    //             break;
    //         }
    //     }

    // }