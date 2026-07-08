using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;


public class CombatMenuManager : MonoBehaviour
{
    [SerializeField] MapManager _currentMap;
    [SerializeField] TurnManager manageTurn;    
    [SerializeField] PlayerGridMovement moveGrid;
    [SerializeField] GenerateGrid generateGrid;
    [SerializeField] ExecuteAction executeAction;
    [SerializeField] FindPath findPath;
    [SerializeField] ActionMenu actionMenu;
    [SerializeField] AttackingMenu attackingMenu;
    [SerializeField] ItemMenu itemMenu;


    //hover menu
    [Header("Hover Menu")]
    private GameObject enemyBar;
    private GameObject playerBar;
    private GameObject hoverMenu;

    private TextMeshProUGUI levelText;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI weaponText;
    private TextMeshProUGUI unitNameText;


    //Expected Menu
    [Header("Expected Menu")]
    private Image EHealthLost;
    private Image EHealth;
    private Image PHealthLost;
    private Image PHealth;

    private Image InfoEnemy1Left;
    private Image InfoEnemy1Right;
    private Image InfoPlayerLeft;
    private Image InfoPlayerRight;
    
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

    [Header("Experience Menu")]
    private GameObject experienceMenu;
    private Image expBar;
   
    private TextMeshProUGUI expUnitName;
    private TextMeshProUGUI expNext;

    private TextMeshProUGUI expGained;

    private Image skillBar;
    private TextMeshProUGUI skillNext;
    private TextMeshProUGUI skillGained;
    private TextMeshProUGUI skillName;
    private TextMeshProUGUI SPNext;
    private TextMeshProUGUI SPGained;

    [SerializeField] GameObject HPIndicator;
    GameObject HPplayer;
    GameObject HPenemy;

    //Level Up Menu
    [Header("Level Up Menu")]
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



    //Phases
    [Header("Phases Menu")]
    private Image PlayerPhase;
    private Image EnemyPhase;



  


    [Header("Victory/Defeated Menu")]
    CanvasGroup victoryBox;
    CanvasGroup defeatBox;
    TextMeshProUGUI defeatText;
    TextMeshProUGUI victoryText;

    CanvasGroup VictoryText;
    CanvasGroup DefeatText;
    GameObject background;



    [Header("Command Points Menu")]
    [SerializeField] Image CPBar;
    [SerializeField] TextMeshProUGUI CPText;
    [SerializeField] GameObject CPParent;


    Gamepad gamepad;

    private float sensitivity = 0.2f;
    private Vector2 moveInput;
    [SerializeField] PlayerInput playerInput;
    bool skipCutscene = false;

    void Awake()
    {
        _currentMap = GameObject.Find("GridManager").GetComponent<MapManager>();

        List<GameObject> actionMenuList = new List<GameObject>();

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
        InfoEnemy1Left = GameObject.Find("Canvas/ExpectedBattleMenu/LeftEnemy1Info").GetComponent<Image>();
        InfoEnemy1Right = GameObject.Find("Canvas/ExpectedBattleMenu/RightEnemy1Info").GetComponent<Image>();
        InfoPlayerLeft = GameObject.Find("Canvas/ExpectedBattleMenu/LeftPlayerInfo").GetComponent<Image>();
        InfoPlayerRight = GameObject.Find("Canvas/ExpectedBattleMenu/RightPlayerInfo").GetComponent<Image>();

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

        // Experience Menu
        experienceMenu = GameObject.Find("Canvas/ExperienceMenu");
        expBar = GameObject.Find("Canvas/ExperienceMenu/ExpBar").GetComponent<Image>();
        expUnitName = GameObject.Find("Canvas/ExperienceMenu/UnitName").GetComponent<TextMeshProUGUI>();
        expNext = GameObject.Find("Canvas/ExperienceMenu/ExpNext").GetComponent<TextMeshProUGUI>();
        expGained = GameObject.Find("Canvas/ExperienceMenu/ExpGained").GetComponent<TextMeshProUGUI>();
        skillBar = GameObject.Find("Canvas/ExperienceMenu/SkillBar").GetComponent<Image>();
        skillNext = GameObject.Find("Canvas/ExperienceMenu/SkillCurrent").GetComponent<TextMeshProUGUI>();
        skillGained = GameObject.Find("Canvas/ExperienceMenu/SkillGained").GetComponent<TextMeshProUGUI>();
        skillName = GameObject.Find("Canvas/ExperienceMenu/SkillName").GetComponent<TextMeshProUGUI>();
        SPNext = GameObject.Find("Canvas/ExperienceMenu/SPCurrent").GetComponent<TextMeshProUGUI>();
        SPGained = GameObject.Find("Canvas/ExperienceMenu/SPGained").GetComponent<TextMeshProUGUI>();


        DeactivateExperienceMenu();

        // Level Up Menu
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

        // Phases
        // TODO: Need to eventually add Ally and Enemy 2
        PlayerPhase = GameObject.Find("Canvas/Phases/PPhase/PlayerPhase").GetComponent<Image>();
        EnemyPhase = GameObject.Find("Canvas/Phases/EPhase/EnemyPhase").GetComponent<Image>();
        PlayerPhase.gameObject.SetActive(false);
        EnemyPhase.gameObject.SetActive(false);

     
        // Victory Conditions
        victoryBox = GameObject.Find("Canvas/VDConditions/Victory").GetComponent<CanvasGroup>(); ;
        defeatBox = GameObject.Find("Canvas/VDConditions/Defeat").GetComponent<CanvasGroup>(); ;
        victoryText = GameObject.Find("Canvas/VDConditions/Victory/VCondition").GetComponent<TextMeshProUGUI>();
        defeatText = GameObject.Find("Canvas/VDConditions/Defeat/DCondition").GetComponent<TextMeshProUGUI>();

        if (victoryBox != null) victoryBox.alpha = 0;
        if (defeatBox != null) defeatBox.alpha = 0;

        // Victory and Defeat Popups
        VictoryText = GameObject.Find("Canvas/Phases/Victory/VicPop").GetComponent<CanvasGroup>();
        DefeatText = GameObject.Find("Canvas/Phases/Defeat/DefPop").GetComponent<CanvasGroup>();
        
        // Background
        background = GameObject.Find("Canvas/Background");
        DeactivateBackground();

    }


    // Gets movement from player input
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    // Sets active a darker background for certain menus
    public void ActivateBackground() => background.SetActive(true);

    public void DeactivateBackground() => background.SetActive(false);
    
    IEnumerator CheckForSkip()
    {
        while (!skipCutscene)
        {
            if (playerInput.actions["SkipCutscene"].WasPressedThisFrame()) // Input Manager should have "Skip" defined
            {
                skipCutscene = true;
            }
            yield return null;
        }
    }


    

    public IEnumerator ActivateActionMenu() {yield return StartCoroutine(actionMenu.ActivateActionMenu()); }
    public IEnumerator PassiveMenu() {yield return StartCoroutine(actionMenu.PassiveMenu()); }
    public void DeactivateActionMenu() { actionMenu.DeactivateActionMenu(); }
 

    public IEnumerator AttackingList(UnitManager unit) {
        // yield return StartCoroutine(attackingMenu.WeaponList(unit));
        yield return StartCoroutine(itemMenu.ItemMenuList(unit, ItemTypeMenu.Weapon));
    }




    public IEnumerator AssistMenu(UnitManager unit) {
        yield return StartCoroutine(itemMenu.ItemMenuList(unit, ItemTypeMenu.Faith));
    }

    public void PlayerWait() {
        actionMenu.PlayerWait();
    }



    public void PlayerItem()
    {
        StartCoroutine(ItemMenu());
    }
    
    

    public IEnumerator ItemMenu() {

        yield return StartCoroutine(itemMenu.ItemMenuList(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile, ItemTypeMenu.Inventory));
    }

//---------------------------------------Hover Menu---------------------------------------------------//

    public void DeactivateHoverMenu() {
        enemyBar.SetActive(false);
        playerBar.SetActive(false);
        hoverMenu.SetActive(false);
    }

    public void ActivateHoverMenu(UnitManager unit) {

        levelText.text = $"{unit.GetLevel()}";
        healthText.text = $"{unit.GetCurrentHealth()} / {unit.GetHealth()}";

        if (unit.GetPrimaryWeapon() != null) {
            weaponText.text = unit.GetPrimaryWeapon().WeaponName;
        } else {
            weaponText.text = "";
        }

        unitNameText.text = unit.GetName();

        hoverMenu.SetActive(true);
        playerBar.SetActive(false);
        playerBar.SetActive(false);

        if (unit.GetUnitType() == "Player")
        {
            playerBar.SetActive(true);
        }
        else if (unit.GetUnitType() == "Enemy")
        {
            enemyBar.SetActive(true);
        }
        // } else {
            //     //Should never be called
            //     playerBar.SetActive(true);
            // }     
        }

//-------------------------------------Expected Menu--------------------------------------------------//

    public void SetUpExpectedMenu(UnitManager player, UnitManager enemy, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {
        PHealthSwapped.gameObject.SetActive(false);
        EHealthSwapped.gameObject.SetActive(false);
        PHealth.gameObject.SetActive(true);
        EHealth.gameObject.SetActive(true);
        PHealthLost.fillAmount = 1;
        int playerHit = -1;
        int enemyHit = -1;
        int playerCrit = -1;
        int enemyCrit = -1;

        InfoEnemy1Left.gameObject.SetActive(false);
        InfoEnemy1Right.gameObject.SetActive(true);
        InfoPlayerLeft.gameObject.SetActive(true);
        InfoPlayerRight.gameObject.SetActive(false);

        if (player.GetPrimaryWeapon() != null) {
            playerHit = player.GetBattleHit(enemy);
            playerCrit = player.GetBattleCrit();
        }

        if (enemy.GetPrimaryWeapon() != null) {
            enemyHit = enemy.GetBattleHit(player);
            enemyCrit = enemy.GetBattleCrit();
        }


        PlayerName.text = player.GetName();
        if (player.GetPrimaryWeapon() != null) {
            PlayerWeapon.text = player.GetPrimaryWeapon().WeaponName;
        } else {
            PlayerWeapon.text = "";
        }
        
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

        PlayerCurrHealth.text = $"{player.GetCurrentHealth()}";

        if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
        if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

        float PLostFill = ((float)expectedPlayerHP / (float)player.GetHealth());
        float PCurrFill = ((float)player.GetCurrentHealth() / (float)player.GetHealth());

        PHealth.fillAmount = PLostFill;
        PHealthLost.fillAmount = PCurrFill;

        Destroy(HPplayer);

        float pXPos = (290.0f * (1.0f - PLostFill)) - 217.0f + 960f;
        HPplayer = Instantiate(HPIndicator, new Vector3(pXPos, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
        TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
        plaText.text = $"{expectedPlayerHP}";



        EnemyName.text = enemy.GetName();
        if (enemy.GetPrimaryWeapon() != null) {
            EnemyWeapon.text = enemy.GetPrimaryWeapon().WeaponName;
        } else {
            EnemyWeapon.text = "";
        }
        
        
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

        EnemyCurrHealth.text = $"{enemy.GetCurrentHealth()}";

        float ELostFill = ((float)expectedEnemyHP / (float)enemy.GetHealth());
        float ECurrFill = ((float)enemy.GetCurrentHealth() / (float)enemy.GetHealth());

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

    public IEnumerator GainExperienceMenu(UnitManager unit, int gainExp, string skillType, int skillInc, int SPInc)
    {
        experienceMenu.SetActive(true);
        expUnitName.text = unit.GetName();


        // launch all 3 animations in parallel
        yield return StartCoroutine(RunAll(
            AnimateExperience(unit, gainExp),
            AnimateSkill(unit, skillType, skillInc),
            AnimateSP(unit, SPInc)
        ));

        yield return new WaitForSeconds(1f);
        DeactivateExperienceMenu();
    }

    private IEnumerator AnimateExperience(UnitManager unit, int gainExp)
    {
        int currentExp = unit.GetExperience();
        int expThreshold = 100;
        int remainingExp = gainExp;

        expBar.fillAmount = (float)currentExp / expThreshold;

        int initialExpNext = expThreshold - currentExp;
        expGained.text = "+" + remainingExp;
        expNext.text = initialExpNext.ToString();

        yield return new WaitForSeconds(0.4f);


        while (remainingExp > 0)
        {
            int expToAdd = Mathf.Min(expThreshold - currentExp, remainingExp);

            float startFill = (float)currentExp / expThreshold;
            float targetFill = (float)(currentExp + expToAdd) / expThreshold;
            float duration = (targetFill - startFill) * 1.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                expBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

                int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(remainingExp, remainingExp - expToAdd, t));
                int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(initialExpNext, initialExpNext - expToAdd, t));

                expGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : "0";
                expNext.text = interpolatedNext.ToString();

                yield return null;
            }

            // finalize this cycle
            expBar.fillAmount = targetFill;
            currentExp += expToAdd;
            remainingExp -= expToAdd;

            if (currentExp >= expThreshold && remainingExp > 0)
            {
                currentExp = 0;
                initialExpNext = expThreshold;
            }
        }

        expGained.text = "0";
        expNext.text = (expThreshold - currentExp).ToString();
    }

    private IEnumerator AnimateSkill(UnitManager unit, string skillType, int skillInc)
    {
        // total exp the unit currently has
        int skillExp = unit.GetStats().GetSkillExperience(skillType);

        // determine current level & thresholds
        int currLevel = UnitRosterManager.GetCurrentLevel(skillExp);
        int prevTotalExpForLevel = UnitRosterManager.GetTotalExpForLevel(currLevel);
        int skillThreshold = UnitRosterManager.GetExpBetweenLevels(currLevel);

        // exp already gained into this level
        int currentSkillExp = skillExp - prevTotalExpForLevel;

        // exp still to animate
        int remainingSkillExp = skillInc;

        // setup UI
        skillName.text = skillType;
        skillGained.text = "+" + remainingSkillExp;
        skillNext.text = UnitRosterManager.GetExpToNextLevel(skillExp).ToString();

        skillBar.fillAmount = (float)currentSkillExp / skillThreshold;

        yield return new WaitForSeconds(0.4f);


        while (remainingSkillExp > 0)
        {
            // how much we can add before reaching next level
            int skillToAdd = Mathf.Min(skillThreshold - currentSkillExp, remainingSkillExp);

            float startFill = (float)currentSkillExp / skillThreshold;
            float targetFill = (float)(currentSkillExp + skillToAdd) / skillThreshold;
            float duration = (targetFill - startFill) * 1.5f;
            float elapsed = 0f;

            int startRemaining = remainingSkillExp;
            int startNext = UnitRosterManager.GetExpToNextLevel(skillExp);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // fill bar animation
                skillBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

                // animate text updates
                int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(startRemaining, startRemaining - skillToAdd, t));
                int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(startNext, startNext - skillToAdd, t));

                skillGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : "0";
                skillNext.text = interpolatedNext.ToString();

                yield return null;
            }

            // finalize this cycle
            skillBar.fillAmount = targetFill;
            currentSkillExp += skillToAdd;
            remainingSkillExp -= skillToAdd;
            skillExp += skillToAdd;

            // leveled up?
            if (currentSkillExp >= skillThreshold && remainingSkillExp > 0)
            {
                currLevel++;
                prevTotalExpForLevel = UnitRosterManager.GetTotalExpForLevel(currLevel);
                skillThreshold = UnitRosterManager.GetExpBetweenLevels(currLevel);

                // reset bar for next level
                currentSkillExp = 0;
                skillBar.fillAmount = 0f;
                skillNext.text = UnitRosterManager.GetExpToNextLevel(skillExp).ToString();
            }
        }

        // clean up UI at the end
        skillGained.text = "0";
        skillNext.text = (skillThreshold - currentSkillExp).ToString();
    }

    private IEnumerator AnimateSP(UnitManager unit, int spInc)
    {
        int currentSP = unit.GetStats().SP;
        int targetSP = currentSP + spInc;

        SPNext.text = currentSP.ToString();
        SPGained.text = $"+{spInc}";

        float duration = 1f;
        float elapsed = 0f;

        yield return new WaitForSeconds(0.4f);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            int interpolatedSP = Mathf.RoundToInt(Mathf.Lerp(currentSP, targetSP, t));
            int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(spInc, 0, t));

            SPNext.text = interpolatedSP.ToString();
            SPGained.text = "+ " + interpolatedGain;

            yield return null;
        }

        SPNext.text = targetSP.ToString();
        SPGained.text = "+0";
    }

    private IEnumerator RunAll(params IEnumerator[] coroutines)
    {
        int finished = 0;

        foreach (var coroutine in coroutines)
        {
            StartCoroutine(RunSingle(coroutine, () => finished++));
        }

        // wait until all finish
        while (finished < coroutines.Length)
            yield return null;
    }

    private IEnumerator RunSingle(IEnumerator coroutine, System.Action onComplete)
    {
        yield return StartCoroutine(coroutine);
        onComplete?.Invoke();
    }


// -----------------------------------------------Level Up Menu------------------------------------------------------

    public void DeactivateLevelUpMenu()
    {
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
        lvName.text = unit.GetName();
        lvClass.text = unit.GetClass();
        lvLevel.text = unit.GetLevel().ToString();

        int HP = unit.GetBaseHealth();
        int Str = unit.GetBaseAttack();
        int Mag = unit.GetBaseMagic();
        int Spd = unit.GetBaseSpeed();
        int Def = unit.GetBaseDefense();
        int Res = unit.GetBaseResistance();
        int Eva = unit.GetBaseEvasion();
        int Lck = unit.GetBaseLuck();

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

        StartCoroutine(CheckForSkip());


        yield return StartCoroutine(AnimateLevelText(lvLevel, unit.GetLevel()));

        // Update stats incrementally and display changes
        if (hp > 0)
        {
            lvHPGR.text = "+" + hp.ToString();
            HP += hp;
            lvHP.text = HP.ToString();
            lvHPGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        if (str > 0)
        {
            lvStrGR.text = "+" + str.ToString();
            Str += str;
            lvStr.text = Str.ToString();
            lvStrGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        if (mag > 0)
        {
            lvMagGR.text = "+" + mag.ToString();
            Mag += mag;
            lvMag.text = Mag.ToString();
            lvMagGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        if (spd > 0)
        {
            lvSpdGR.text = "+" + spd.ToString();
            Spd += spd;
            lvSpd.text = Spd.ToString();
            lvSpdGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        if (def > 0)
        {
            lvDefGR.text = "+" + def.ToString();
            Def += def;
            lvDef.text = Def.ToString();
            lvDefGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        if (res > 0)
        {
            lvResGR.text = "+" + res.ToString();
            Res += res;
            lvRes.text = Res.ToString();
            lvResGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        if (eva > 0)
        {
            lvEvaGR.text = "+" + eva.ToString();
            Eva += eva;
            lvEva.text = Eva.ToString();
            lvEvaGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        if (lck > 0)
        {
            lvLckGR.text = "+" + lck.ToString();
            Lck += lck;
            lvLck.text = Lck.ToString();
            lvLckGR.gameObject.SetActive(true);
            yield return WaitForTime(waitTime);
        }

        StopCoroutine(CheckForSkip());
        skipCutscene = false;

        // Wait before deactivating the menu
        yield return new WaitForSeconds(1f);

        DeactivateLevelUpMenu();
    }

    private IEnumerator WaitForTime(float waitTime)
    {
        float timer = 0f;

        while (timer < waitTime)
        {
            if (skipCutscene) yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        
    }
    


    private IEnumerator AnimateLevelText(TextMeshProUGUI levelText, int level)
    {
        Vector3 originalScale = levelText.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f; // Increase size by 50%
        float animationTime = 0.5f; // Time for animation
        float elapsedTime = 0f;
        int newLevel;

        // Scale up
        while (elapsedTime < animationTime)
        {
            if (skipCutscene)
            {
                newLevel = level + 1;
                levelText.text = newLevel.ToString();
                levelText.transform.localScale = originalScale;
                yield break;
            }
            levelText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }



        // Ensure it's at the target scale
        levelText.transform.localScale = targetScale;

        newLevel = level + 1;
        levelText.text = newLevel.ToString();



        // Reset timer for scale down
        elapsedTime = 0f;

        // Scale down
        while (elapsedTime < animationTime)
        {
            if (skipCutscene)
            {
                levelText.transform.localScale = originalScale;
                yield break;
            }
            levelText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's back to the original scale
        levelText.transform.localScale = originalScale;
    }






    //--------------------------------------Phases--------------------------------------

    public IEnumerator PhaseStart(string phase) {
        Image temp = null;
        if (phase == "Player")
        {
            PlayerPhase.gameObject.SetActive(true);
            temp = PlayerPhase;
        }
        else if (phase == "Enemy")
        {
            EnemyPhase.gameObject.SetActive(true);
            temp = EnemyPhase;
        }
        else
        {
            yield break;
        }
        if (temp != null) {
            RectTransform rectTransform = temp.rectTransform;

            // Move to the starting position (off-screen to the right)
            Vector3 originalPosition = rectTransform.localPosition;
            Vector3 startPosition = originalPosition + new Vector3(1000, 0, 0); // Adjust as needed
            rectTransform.localPosition = startPosition;

            // Fade in and slide in
            yield return StartCoroutine(FadeAndSlideToPosition(temp, 1f, 0.25f, startPosition, originalPosition));

            // Wait for 1 second
            yield return new WaitForSeconds(1.15f);

            // Fade out and slide out
            Vector3 endPosition = originalPosition - new Vector3(1000, 0, 0); // Adjust as needed
            yield return StartCoroutine(FadeAndSlideToPosition(temp, 0f, 0.25f, originalPosition, endPosition));

            // Reset position to the original
            rectTransform.localPosition = originalPosition;
        }
        PlayerPhase.gameObject.SetActive(false);
        EnemyPhase.gameObject.SetActive(false);

    }

    public IEnumerator VicDefText(string endType) {
        Image temp = null;
        if (endType == "Victory") { temp = VictoryText.gameObject.GetComponent<Image>(); }
        else if (endType == "Defeat") { temp = DefeatText.gameObject.GetComponent<Image>(); }

        background.SetActive(true);



        if (temp != null) {
            Debug.LogWarning("Defeat");
            RectTransform rectTransform = temp.rectTransform;
            Vector3 originalPosition = rectTransform.localPosition;
            Vector3 startPosition = originalPosition + new Vector3(1000, 0, 0);
            yield return StartCoroutine(FadeAndSlideToPosition(temp, 1f, 0.25f, startPosition, originalPosition));
        }

        yield return null;
    }

    private IEnumerator FadeAndSlideToPosition(Image image, float targetAlpha, float duration, Vector3 startPosition, Vector3 endPosition) {
        float elapsed = 0f;

        // Ensure CanvasGroup for fading
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = image.gameObject.AddComponent<CanvasGroup>();
        }

        // TextMeshProUGUI turnTxt = canvasGroup.transform.Find("Turns").GetComponent<TextMeshProUGUI>();
        Transform turnsTransform = canvasGroup.transform.Find("Turns");
        if (turnsTransform != null) {
            turnsTransform.GetComponent<TextMeshProUGUI>().text = "Turn: " + manageTurn.GetTurns();
        }
        

        // Get child text, if any
        // Text childText = image.GetComponentInChildren<Text>();

        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Lerp position and alpha
            image.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            // Apply alpha to child text
            // if (childText != null)
            // {
            //     Color textColor = childText.color;
            //     textColor.a = canvasGroup.alpha;
            //     childText.color = textColor;
            // }

            yield return null;
        }

        // Finalize position and alpha
        image.rectTransform.localPosition = endPosition;
        canvasGroup.alpha = targetAlpha;

    //     if (childText != null)
    //     {
    //         Color finalTextColor = childText.color;
    //         finalTextColor.a = targetAlpha;
    //         childText.color = finalTextColor;
    //     }
    // }
    }




    public IEnumerator FadeUpVD(string vCond, string Dcond)
    {
        victoryText.text = vCond;
        defeatText.text = Dcond;

        skipCutscene = false;
        StartCoroutine(CheckForSkip());

        // Fade up the first image
        yield return StartCoroutine(FadeUpCanvasGroup(victoryBox, 0.5f, 50f));

        // Wait before starting the second fade-up
        if (!skipCutscene) yield return new WaitForSeconds(0.5f);

        // Fade up the second image
        yield return StartCoroutine(FadeUpCanvasGroup(defeatBox, 0.5f, 50f));

        // yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (playerInput.actions["Select"].WasPressedThisFrame())
            {
                victoryBox.gameObject.SetActive(false);
                defeatBox.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
        
        StopCoroutine(CheckForSkip());
        skipCutscene = false;

        
    }

    IEnumerator FadeUpCanvasGroup(CanvasGroup canvasGroup, float duration, float distance)
    {
        float elapsed = 0f;

        // Get the RectTransform dynamically from the CanvasGroup's GameObject
        RectTransform rectTransform = canvasGroup.GetComponent<RectTransform>();
        float initialY = rectTransform.anchoredPosition.y;
        float targetY = initialY + distance;

        while (elapsed < duration)
        {
            if (skipCutscene) break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Fade in (alpha)
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            // Move up (position)
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                Mathf.Lerp(initialY, targetY, t)
            );

            yield return null;
        }

        // Ensure final values are set
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetY);
    }




    public IEnumerator BattleMenu(UnitManager left, UnitManager right, int expectedLeftHP, int expectedRightHP, int lDamage, int rDamage, int numLHits, int numRHits, Weapon leftWeap, Weapon rightWeap) {

        Image lBar;
        Image rBar;
        Image lLostBar;
        Image rLostBar;

        lLostBar = PHealthLost;
        rLostBar = EHealthLost;


        if (left.GetUnitType() == "Player") {
            lBar = PHealth;
            PHealth.gameObject.SetActive(true);
            PHealthSwapped.gameObject.SetActive(false);
            InfoEnemy1Left.gameObject.SetActive(false);
            InfoPlayerLeft.gameObject.SetActive(true);
        } else {
            lBar = PHealthSwapped;
            PHealthSwapped.gameObject.SetActive(true);
            PHealth.gameObject.SetActive(false);
            InfoEnemy1Left.gameObject.SetActive(true);
            InfoPlayerLeft.gameObject.SetActive(false);
        }

        if (right.GetUnitType() == "Player") {
            rBar = EHealthSwapped;
            EHealthSwapped.gameObject.SetActive(true);
            EHealth.gameObject.SetActive(false);
            InfoPlayerRight.gameObject.SetActive(true);
            InfoEnemy1Right.gameObject.SetActive(false);

        } else {
            rBar = EHealth;
            EHealth.gameObject.SetActive(true);
            EHealthSwapped.gameObject.SetActive(false);
            InfoPlayerRight.gameObject.SetActive(false);
            InfoEnemy1Right.gameObject.SetActive(true);

        }
        
        if (expectedLeftHP < 0) { expectedLeftHP = 0; }
        if (expectedRightHP < 0) { expectedRightHP = 0; }

        // Use left and right units directly instead of re-assigning them
        UnitManager leftUnit = left;
        UnitManager rightUnit = right;

        // Calculate hit and crit rates for both units
        int leftHit = -1;
        int rightHit = -1;
        int leftCrit = -1;
        int rightCrit = -1;
        PlayerWeapon.text = "";
        EnemyWeapon.text = "";

        if (leftWeap != null) {
            leftHit = leftUnit.GetBattleHit(rightUnit);
            leftCrit = leftUnit.GetBattleCrit();
            PlayerWeapon.text = leftWeap.WeaponName;
        } 
        if (rightWeap != null) {
            rightHit = rightUnit.GetBattleHit(leftUnit);
            rightCrit = rightUnit.GetBattleCrit();
            EnemyWeapon.text = rightWeap.WeaponName;
        }
        

        // Clamp hit and crit rates between 0 and 100
        leftHit = Mathf.Clamp(leftHit, 0, 100);
        leftCrit = Mathf.Clamp(leftCrit, 0, 100);
        rightHit = Mathf.Clamp(rightHit, 0, 100);
        rightCrit = Mathf.Clamp(rightCrit, 0, 100);

        // Set the left unit (player or enemy) info
        PlayerName.text = leftUnit.GetName();

        if (numLHits > 1) {
            PlayerDamage.text = $"{lDamage} x {numLHits}";
            PlayerHit.text = $"{leftHit}%";
            PlayerCrit.text = $"{leftCrit}%";
        } else if (numLHits == 0) {
            PlayerDamage.text = "-";
            PlayerHit.text = "-";
            PlayerCrit.text = "-";
        } else {
            PlayerDamage.text = $"{lDamage}";
            PlayerHit.text = $"{leftHit}%";
            PlayerCrit.text = $"{leftCrit}%";
        }
        PlayerCurrHealth.text = $"{leftUnit.GetCurrentHealth()}";

        // Update health bars and positions
        float leftCurrFill = (float)(expectedLeftHP) / leftUnit.GetHealth();
        float leftLostFill = (float)leftUnit.GetCurrentHealth() / leftUnit.GetHealth();
        lBar.fillAmount = leftCurrFill;
        lLostBar.fillAmount = leftLostFill;

        // Destroy any previous HP indicator for the player
        Destroy(HPplayer);

        // Set position for the player's health bar
        float pXPos = (290.0f * (1.0f - leftLostFill)) - 217.0f ;
        HPplayer = Instantiate(HPIndicator, new Vector3(pXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
        TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
        // PlayerCurrHealth.text = $"{leftUnit.getCurrentHealth()}";
        plaText.text = $"{expectedLeftHP}";
    
        

        // Set the right unit (enemy or player) info
        EnemyName.text = rightUnit.GetName();


        
        if (numRHits > 1) {
            EnemyDamage.text = $"{rDamage} x {numRHits}";
            EnemyHit.text = $"{rightHit}%";
            EnemyCrit.text = $"{rightCrit}%";
        } else if (numRHits == 0) {
            EnemyDamage.text = "-";
            EnemyHit.text = "-";
            EnemyCrit.text = "-";
        } else {
            EnemyDamage.text = $"{rDamage}";
            EnemyHit.text = $"{rightHit}%";
            EnemyCrit.text = $"{rightCrit}%";
        }
        EnemyCurrHealth.text = $"{rightUnit.GetCurrentHealth()}";

        // Update health bars and positions
        float rightCurrFill = (float)(expectedRightHP) / rightUnit.GetHealth();
        float rightLostFill = (float)rightUnit.GetCurrentHealth() / rightUnit.GetHealth();
        rBar.fillAmount = rightCurrFill;
        rLostBar.fillAmount = rightLostFill;

        // Destroy any previous HP indicator for the enemy
        Destroy(HPenemy);

        // Set position for the enemy's health bar
        float eXPos = (291.0f * rightLostFill) + 140.0f ;
        HPenemy = Instantiate(HPIndicator, new Vector3(eXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject eneChild = HPenemy.transform.GetChild(0).gameObject;
        TextMeshProUGUI eneText = eneChild.GetComponent<TextMeshProUGUI>();

        eneText.text = $"{expectedRightHP}";

        // if(playerOnLeft) {
        //     eneText.text = $"{expectedEnemyHP}";
        //     // EnemyCurrHealth.text = $"{rightUnit.getCurrentHealth()}";
        // } else {
        //     eneText.text = $"{expectedPlayerHP}";
        //     // EnemyCurrHealth.text = $"{leftUnit.getCurrentHealth()}";
        // }


        // Activate the menu
        Menu.SetActive(true);

        yield return null;
        // yield return new WaitForSeconds(1f);
    }




    public IEnumerator UpdateCommandPointMenu() {
        ActivateCPMenu();
        CPText.text = manageTurn.GetCP().ToString();
        CPBar.fillAmount = (float)((float)manageTurn.GetCP() / (float)_currentMap.GetCP());
        yield break;
    }

    public void DeactivateCPMenu() => CPParent.SetActive(false);
    public void ActivateCPMenu() => CPParent.SetActive(true);










}




