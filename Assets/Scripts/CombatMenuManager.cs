using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CombatMenuManager : MonoBehaviour
{
    private IMaps _currentMap;
    private TurnManager manageTurn;    
    private PlayerGridMovement moveGrid;
    private GenerateGrid generateGrid;
    private ExecuteAction executeAction;
    private FindPath findPath;
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



    //Phases
    private Image PlayerPhase;
    private Image EnemyPhase;



    //ItemMenu
    private GameObject scrollViewContent;
    [SerializeField] GameObject buttonTemplate;
    private GameObject itemMenu;
    List<Button> itemButtons;
    int currItemIndex = 0;
    [SerializeField] GameObject buttonItemOption;

    List<Weapon> usableWeapons;
    List<Weapon> nonUsableWeapons;
    List<Weapon> listOfWeapons;
    int currWeapIndex = 0;

    bool inItemMenu = false;


    Gamepad gamepad;



    // Start is called before the first frame update
    void Start()
    {
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        moveGrid = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        executeAction = GameObject.Find("Player").GetComponent<ExecuteAction>();
        findPath = GameObject.Find("Player").GetComponent<FindPath>();

        //Action Menu
        attackButton = GameObject.Find("Canvas/AttackButton");
        itemButton = GameObject.Find("Canvas/ItemButton");
        waitButton = GameObject.Find("Canvas/WaitButton");

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

        PlayerPhase = GameObject.Find("Canvas/Phases/PPhase/PlayerPhase").GetComponent<Image>();
        EnemyPhase = GameObject.Find("Canvas/Phases/EPhase/EnemyPhase").GetComponent<Image>();
        PlayerPhase.gameObject.SetActive(false);
        EnemyPhase.gameObject.SetActive(false);

        //Item

        scrollViewContent = GameObject.Find("Canvas/ItemMenu/ScrollView/Viewport/Content");
        itemMenu = GameObject.Find("Canvas/ItemMenu");
        itemButtons = new List<Button>();
        DeactivateItemMenu();


        usableWeapons = new List<Weapon>();
        nonUsableWeapons = new List<Weapon>();

        listOfWeapons = new List<Weapon>();


    }

    void Update() {
        gamepad = Gamepad.current;
    }

    public IEnumerator ActivateActionMenu() {
        attackButton.SetActive(true);
        waitButton.SetActive(true);
        itemButton.SetActive(true);

        Debug.Log("Start Of Action");

        CheckWeapons(moveGrid.GetPlayerCollide().GetPlayer());


        List<Button> buttons = new List<Button>();
        List<string> Actions = new List<string>();

        Button attack = attackButton.GetComponent<Button>();
        Button wait = waitButton.GetComponent<Button>();
        Button item = itemButton.GetComponent<Button>();

        buttons.Add(attack);
        buttons.Add(item);
        buttons.Add(wait);

        Actions.Add("Attack");
        Actions.Add("Item");
        Actions.Add("Wait");

        bool buttonClicked = false;

        if(usableWeapons.Count <= 0) {
            buttons.Remove(attack);
            Actions.Remove("Attack");
            attackButton.SetActive(false);
        }


        int currentIndex = 0;
        Debug.Log(buttons[currentIndex] + " " + Actions[currentIndex]);
        buttons[currentIndex].Select();
        bool axisInUse = false;
        bool oneAction = false;

        while (true) {
            float vertical = Input.GetAxis("Vertical");

            Debug.Log(buttons[currentIndex]);
            

            if (!axisInUse)
            {
                if (vertical > 0.2f) // Move up
                {
                    buttons[currentIndex].OnDeselect(null);
                    // currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
                    currentIndex--;
                    if (currentIndex < 0) { currentIndex = buttons.Count - 1; }
                    buttons[currentIndex].Select();
                    axisInUse = true;
                    // yield return new WaitForSeconds(0.25f);
                }
                else if (vertical < -0.2f) // Move down
                {
                    buttons[currentIndex].OnDeselect(null);
                    // currentIndex = (currentIndex + 1) % buttons.Count;
                    currentIndex++ ;
                    if (currentIndex >= buttons.Count) { currentIndex = 0; }
                    buttons[currentIndex].Select();
                    axisInUse = true;
                    // yield return new WaitForSeconds(0.25f);
                }
            }

            if (Mathf.Abs(vertical) < 0.2f)
            {
                axisInUse = false;
            }

            if (oneAction && !buttonClicked && (Input.GetKeyDown(KeyCode.Space) || gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)) // "Submit" button
            {
                // buttons[currentIndex].onClick.Invoke();
                Debug.Log("Start Of Click");
                // buttonClicked = true;

                if (Actions[currentIndex] == "Attack") { DeactivateActionMenu(); PlayerAttack(); }
                if (Actions[currentIndex] == "Item") {  Debug.Log("Start of ITEM CHOSEN"); DeactivateActionMenu(); PlayerItem();  }
                if (Actions[currentIndex] == "Wait") { Debug.Log("Start of WAIT CHOSEN");  DeactivateActionMenu(); PlayerWait(); }

                
                break;
            }
            // if ((oneAction && gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)) { DeactivateActionMenu(); break; }

            if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)) && oneAction) {
                // moveGrid.inMenu = false;
                moveGrid.OutOfMenu();
                break;
            }
            oneAction = true;

            yield return null;
        }

        yield return null;
    }

    public void DeactivateActionMenu() {
        attackButton.SetActive(false);
        waitButton.SetActive(false);
        itemButton.SetActive(false);
    }


    public void PlayerWait() {
        
        manageTurn.RemovePlayer(moveGrid.GetPlayerCollide().GetPlayer().stats);
        executeAction.unitWait();
        manageTurn.CheckPhase();
        _currentMap.CheckClearCondition();
        
        
    }

    public void PlayerAttack() {

        UnitManager unit = moveGrid.GetPlayerCollide().GetPlayer();
        CheckWeapons(unit);

        StartCoroutine(WeaponList(unit));

        // GameObject currUnit = moveGrid.GetPlayerCollide().GetPlayerObject();
        // int attackRangeStat = moveGrid.GetPlayerCollide().GetPlayerAttack();

        // UnitManager temp = moveGrid.GetPlayerCollide().GetPlayer();
            

        // executeAction.attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), temp.primaryWeapon.Range, temp.primaryWeapon.Range1, temp.primaryWeapon.Range2, temp.primaryWeapon.Range3);
        // findPath.HighlightAttack(executeAction.attackGrid);

        // Debug.Log("Attack");
        // moveGrid.isAttacking = true;
        // List<GridTile> UnitsInRange = new List<GridTile>();
        
        
        // for (int i = 0; i < generateGrid.GetWidth(); i++) {
        //     for (int j = 0; j < generateGrid.GetLength(); j++) {
        //         if (executeAction.attackGrid[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                    
        //             UnitsInRange.Add(generateGrid.GetGridTile(i,j));
                    
        //         }
        //     }
        // }

        // if (UnitsInRange.Count == 0) {
        //     moveGrid.isAttacking = false;
        //     StartCoroutine(ActivateActionMenu());
        //     return;
        // } else {
           
        //     DeactivateActionMenu();

        //     StartCoroutine(executeAction.CycleAttackList(UnitsInRange)); 
        // }

    }

    // public void CheckWeapons(UnitManager unit) {
        
    //     List<Weapon> tempWeap = unit.GetWeapons();

    //     foreach (Weapon wep in tempWeap) {
    //         bool[,] attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), wep.Range, wep.Range1, wep.Range2, wep.Range3);
    //         for (int i = 0; i < generateGrid.GetWidth(); i++) {
    //             for (int j = 0; j < generateGrid.GetLength(); j++) {
    //                 if (attackGrid[i,j] && !usableWeapons.Contains(wep) && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                        
    //                     // UnitsInRange.Add(generateGrid.GetGridTile(i,j));
    //                     usableWeapons.Add(wep);
                        
    //                 }
    //             }
    //         }

    //         if(!usableWeapons.Contains(wep)) {
    //             nonUsableWeapons.Add(wep);
    //         }
    //     }
    // }

    // public IEnumerator WeaponList(UnitManager unit) {
    //     int weapIndex = 0;

    //     bool axisInUse = false;
    //     bool oneAction = false;

    //     while (true) {
    //         float vertical = -Input.GetAxis("Vertical");

    //         Debug.Log(buttons[currentIndex]);
            

    //         if (!axisInUse)
    //         {
    //             if (vertical > 0.2f) // Move up
    //             {
    //                 buttons[currentIndex].OnDeselect(null);
    //                 // currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
    //                 currentIndex--;
    //                 if (currentIndex < 0) { currentIndex = buttons.Count - 1; }
    //                 buttons[currentIndex].Select();
    //                 axisInUse = true;
    //                 // yield return new WaitForSeconds(0.25f);
    //             }
    //             else if (vertical < -0.2f) // Move down
    //             {
    //                 buttons[currentIndex].OnDeselect(null);
    //                 // currentIndex = (currentIndex + 1) % buttons.Count;
    //                 currentIndex++ ;
    //                 if (currentIndex >= buttons.Count) { currentIndex = 0; }
    //                 buttons[currentIndex].Select();
    //                 axisInUse = true;
    //                 // yield return new WaitForSeconds(0.25f);
    //             }
    //         }

    //         if (Mathf.Abs(vertical) < 0.2f)
    //         {
    //             axisInUse = false;
    //         }

    //         if (oneAction && (Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame))) // "Submit" button
    //         {
    //             buttons[currentIndex].onClick.Invoke();
    //             break;
    //         }
    //         if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)) && oneAction) {
    //             // moveGrid.inMenu = false;
    //             moveGrid.OutOfMenu();
    //             break;
    //         }
    //         oneAction = true;

    //         yield return null;
    //     }

    //     yield return null;

    // }

    public void PlayerItem() {
        Debug.Log("Start of Item");
        // if (inItemMenu) { return; }
        // inItemMenu = true;
        // UnitManager unit = moveGrid.GetPlayerCollide().GetPlayer();

        // Item temp = unit.GetStats().GetItemAt(0);

        // if (temp != null) {
        //     temp.Use(unit);
        // }

        // executeAction.ResetAfterAction(unit);
        ActivateItemMenu();
        StartCoroutine(ItemMenu());
    }


//---------------------------------------Hover Menu---------------------------------------------------//

    public void DeactivateHoverMenu() {
        enemyBar.SetActive(false);
        playerBar.SetActive(false);
        hoverMenu.SetActive(false);
    }

    public void ActivateHoverMenu(UnitManager unit) {

        levelText.text = $"{unit.stats.Level}";
        healthText.text = $"{unit.getCurrentHealth()} / {unit.stats.Health}";

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
        PHealth.gameObject.SetActive(true);
        EHealth.gameObject.SetActive(true);
        PHealthLost.fillAmount = 1;
        int playerHit = -1;
        int enemyHit = -1;
        int playerCrit = -1;
        int enemyCrit = -1;

        if (player.primaryWeapon != null) {
            playerHit = player.primaryWeapon.HitRate + (player.stats.Luck * 4) - enemy.stats.Evasion;
            playerCrit = player.primaryWeapon.CritRate + (int)(player.stats.Luck / 2);
        }

        if (enemy.primaryWeapon != null) {
            enemyHit = enemy.primaryWeapon.HitRate + (enemy.stats.Luck * 4) - player.stats.Evasion;
            enemyCrit = enemy.primaryWeapon.CritRate + (int)(enemy.stats.Luck / 2);
        }


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
        if (player.primaryWeapon != null) {
            PlayerWeapon.text = player.primaryWeapon.WeaponName;
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

        PlayerCurrHealth.text = $"{player.getCurrentHealth()}";

        if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
        if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

        float PLostFill = ((float)expectedPlayerHP / (float)player.stats.Health);
        float PCurrFill = ((float)player.getCurrentHealth() / (float)player.stats.Health);

        PHealth.fillAmount = PLostFill;
        PHealthLost.fillAmount = PCurrFill;

        Destroy(HPplayer);

        float pXPos = (290.0f * (1.0f - PLostFill)) - 217.0f + 960f;
        HPplayer = Instantiate(HPIndicator, new Vector3(pXPos, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
        TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
        plaText.text = $"{expectedPlayerHP}";



        EnemyName.text = enemy.stats.Name;
        if (enemy.primaryWeapon != null) {
            EnemyWeapon.text = enemy.primaryWeapon.WeaponName;
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

        EnemyCurrHealth.text = $"{enemy.getCurrentHealth()}";

        float ELostFill = ((float)expectedEnemyHP / (float)enemy.stats.Health);
        float ECurrFill = ((float)enemy.getCurrentHealth() / (float)enemy.stats.Health);

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


//     public IEnumerator BattleMenu(UnitManager left, UnitManager right, bool playerOnLeft, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {

//     Image pBar;
//     Image eBar;
//     Image pLostBar;
//     Image eLostBar;

//     if (playerOnLeft) {
//         pBar = PHealth;
//         eBar = EHealth;
//         pLostBar = PHealthLost;
//         eLostBar = EHealthLost;
//         pBar.gameObject.SetActive(true);
//         eBar.gameObject.SetActive(true);
//         PHealthSwapped.gameObject.SetActive(false);
//         EHealthSwapped.gameObject.SetActive(false);
//     } else {
//         pBar = EHealthSwapped;
//         eBar = PHealthSwapped;
//         pLostBar = EHealthLost;
//         eLostBar = PHealthLost;
//         pBar.gameObject.SetActive(true);
//         eBar.gameObject.SetActive(true);
//         PHealth.gameObject.SetActive(false);
//         EHealth.gameObject.SetActive(false);
//     }

//     if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
//     if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

//     UnitManager leftUnit = playerOnLeft ? player : enemy;
//     UnitManager rightUnit = playerOnLeft ? enemy : player;

//     int leftHit = leftUnit.primaryWeapon.HitRate + (leftUnit.stats.Luck * 4) - rightUnit.stats.Evasion;
//     int rightHit = rightUnit.primaryWeapon.HitRate + (rightUnit.stats.Luck * 4) - leftUnit.stats.Evasion;
//     int leftCrit = leftUnit.primaryWeapon.CritRate + (int)(leftUnit.stats.Luck / 2);
//     int rightCrit = rightUnit.primaryWeapon.CritRate + (int)(rightUnit.stats.Luck / 2);

//     // Clamp hit and crit rates
//     leftHit = Mathf.Clamp(leftHit, 0, 100);
//     leftCrit = Mathf.Clamp(leftCrit, 0, 100);
//     rightHit = Mathf.Clamp(rightHit, 0, 100);
//     rightCrit = Mathf.Clamp(rightCrit, 0, 100);

//     // Left Unit (Player or Enemy based on position)
//     PlayerName.text = leftUnit.stats.Name;
//     PlayerWeapon.text = leftUnit.primaryWeapon.WeaponName;
//     if (numPHits > 1) {
//         PlayerDamage.text = $"{PDamage} x {numPHits}";
//         PlayerHit.text = $"{leftHit}%";
//         PlayerCrit.text = $"{leftCrit}%";
//     } else if (numPHits == 0) {
//         PlayerDamage.text = "-";
//         PlayerHit.text = "-";
//         PlayerCrit.text = "-";
//     } else {
//         PlayerDamage.text = $"{PDamage}";
//         PlayerHit.text = $"{leftHit}%";
//         PlayerCrit.text = $"{leftCrit}%";
//     }
//     PlayerCurrHealth.text = $"{leftUnit.getCurrentHealth()}";

//     float leftCurrFill = (float)(expectedPlayerHP / leftUnit.stats.Health);
//     float leftLostFill = (float)leftUnit.getCurrentHealth() / leftUnit.stats.Health;
//     pBar.fillAmount = leftCurrFill;
//     pLostBar.fillAmount = leftLostFill;

//     Destroy(HPplayer);

//     float pXPos = playerOnLeft ? (290.0f * (1.0f - leftLostFill)) - 217.0f : (291.0f * (float)leftLostFill) + 140.0f;
//     HPplayer = Instantiate(HPIndicator, new Vector3(pXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

//     GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
//     TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
//     plaText.text = $"{expectedPlayerHP}";

//     // Right Unit (Enemy or Player based on position)
//     EnemyName.text = rightUnit.stats.Name;
//     EnemyWeapon.text = rightUnit.primaryWeapon.WeaponName;
//     if (numEHits > 1) {
//         EnemyDamage.text = $"{EDamage} x {numEHits}";
//         EnemyHit.text = $"{rightHit}%";
//         EnemyCrit.text = $"{rightCrit}%";
//     } else if (numEHits == 0) {
//         EnemyDamage.text = "-";
//         EnemyHit.text = "-";
//         EnemyCrit.text = "-";
//     } else {
//         EnemyDamage.text = $"{EDamage}";
//         EnemyHit.text = $"{rightHit}%";
//         EnemyCrit.text = $"{rightCrit}%";
//     }
//     EnemyCurrHealth.text = $"{rightUnit.getCurrentHealth()}";

//     float rightCurrFill = (float)(expectedEnemyHP / rightUnit.stats.Health);
//     float rightLostFill = (float)rightUnit.getCurrentHealth() / rightUnit.stats.Health ;
//     eBar.fillAmount = rightCurrFill;
//     eLostBar.fillAmount = rightLostFill;

//     Destroy(HPenemy);

//     float eXPos = playerOnLeft ? (291.0f * (float)rightLostFill) + 140.0f : (290.0f * (1.0f - rightLostFill)) - 217.0f;
//     HPenemy = Instantiate(HPIndicator, new Vector3(eXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

//     GameObject eneChild = HPenemy.transform.GetChild(0).gameObject;
//     TextMeshProUGUI eneText = eneChild.GetComponent<TextMeshProUGUI>();
//     eneText.text = $"{expectedEnemyHP}";

//     Menu.SetActive(true);

//     yield return null;
//     // yield return new WaitForSeconds(1f);
// }

public IEnumerator BattleMenu(UnitManager left, UnitManager right, bool playerOnLeft, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {

    Image pBar;
    Image eBar;
    Image pLostBar;
    Image eLostBar;

    // Assign health bars based on whether the player is on the left
    if (playerOnLeft) {
        pBar = PHealth;
        eBar = EHealth;
        pLostBar = PHealthLost;
        eLostBar = EHealthLost;
        pBar.gameObject.SetActive(true);
        eBar.gameObject.SetActive(true);
        PHealthSwapped.gameObject.SetActive(false);
        EHealthSwapped.gameObject.SetActive(false);
    } else {
        eBar = EHealthSwapped;
        pBar = PHealthSwapped;
        pLostBar = PHealthLost;
        eLostBar = EHealthLost;
        pBar.gameObject.SetActive(true);
        eBar.gameObject.SetActive(true);
        PHealth.gameObject.SetActive(false);
        EHealth.gameObject.SetActive(false);
    }

    if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
    if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

    // Use left and right units directly instead of re-assigning them
    UnitManager leftUnit = left;
    UnitManager rightUnit = right;

    // Calculate hit and crit rates for both units
    int leftHit = -1;
    int rightHit = -1;
    int leftCrit = -1;
    int rightCrit = -1;
    if (leftUnit.primaryWeapon != null) {
        leftHit = leftUnit.primaryWeapon.HitRate + (leftUnit.stats.Luck * 4) - rightUnit.stats.Evasion;
        leftCrit = leftUnit.primaryWeapon.CritRate + (int)(leftUnit.stats.Luck / 2);
    }
    if (rightUnit.primaryWeapon != null) {
        rightHit = rightUnit.primaryWeapon.HitRate + (rightUnit.stats.Luck * 4) - leftUnit.stats.Evasion;
        rightCrit = rightUnit.primaryWeapon.CritRate + (int)(rightUnit.stats.Luck / 2);
    }
    

    // Clamp hit and crit rates between 0 and 100
    leftHit = Mathf.Clamp(leftHit, 0, 100);
    leftCrit = Mathf.Clamp(leftCrit, 0, 100);
    rightHit = Mathf.Clamp(rightHit, 0, 100);
    rightCrit = Mathf.Clamp(rightCrit, 0, 100);

    // Set the left unit (player or enemy) info
    PlayerName.text = leftUnit.stats.Name;

    if(leftUnit.primaryWeapon != null) {
        PlayerWeapon.text = leftUnit.primaryWeapon.WeaponName;

    } else {
        PlayerWeapon.text = "";
    }
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
    PlayerCurrHealth.text = $"{leftUnit.getCurrentHealth()}";

    // Update health bars and positions
    float leftCurrFill = (float)(expectedPlayerHP) / leftUnit.stats.Health;
    float leftLostFill = (float)leftUnit.getCurrentHealth() / leftUnit.stats.Health;
    pBar.fillAmount = leftCurrFill;
    pLostBar.fillAmount = leftLostFill;

    // Destroy any previous HP indicator for the player
    Destroy(HPplayer);

    // Set position for the player's health bar
    float pXPos = (290.0f * (1.0f - leftLostFill)) - 217.0f ;
    HPplayer = Instantiate(HPIndicator, new Vector3(pXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

    GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
    TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
    // PlayerCurrHealth.text = $"{leftUnit.getCurrentHealth()}";
    plaText.text = $"{expectedPlayerHP}";
    // if(playerOnLeft) {
    //     plaText.text = $"{expectedPlayerHP}";
    //     // PlayerCurrHealth.text = $"{leftUnit.getCurrentHealth()}";
    // } else {
    //     plaText.text = $"{expectedEnemyHP}";
    //     // PlayerCurrHealth.text = $"{rightUnit.getCurrentHealth()}";
    // }
    

    // Set the right unit (enemy or player) info
    EnemyName.text = rightUnit.stats.Name;

    if (rightUnit.primaryWeapon != null) {
        EnemyWeapon.text = rightUnit.primaryWeapon.WeaponName;
    } else {
        EnemyWeapon.text = "";
    }
    
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
    EnemyCurrHealth.text = $"{rightUnit.getCurrentHealth()}";

    // Update health bars and positions
    float rightCurrFill = (float)(expectedEnemyHP) / rightUnit.stats.Health;
    float rightLostFill = (float)rightUnit.getCurrentHealth() / rightUnit.stats.Health;
    eBar.fillAmount = rightCurrFill;
    eLostBar.fillAmount = rightLostFill;

    // Destroy any previous HP indicator for the enemy
    Destroy(HPenemy);

    // Set position for the enemy's health bar
    float eXPos = (291.0f * rightLostFill) + 140.0f ;
    HPenemy = Instantiate(HPIndicator, new Vector3(eXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

    GameObject eneChild = HPenemy.transform.GetChild(0).gameObject;
    TextMeshProUGUI eneText = eneChild.GetComponent<TextMeshProUGUI>();

    eneText.text = $"{expectedEnemyHP}";

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












    //--------------------------------------Phases--------------------------------------

    public IEnumerator PhaseStart(string phase) {
        Image temp = null;
        if (phase == "Player") {
            PlayerPhase.gameObject.SetActive(true);
            temp = PlayerPhase;
        } else if (phase == "Enemy") {
            EnemyPhase.gameObject.SetActive(true);
            temp = EnemyPhase;
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

    private IEnumerator FadeAndSlideToPosition(Image image, float targetAlpha, float duration, Vector3 startPosition, Vector3 endPosition) {
        float elapsed = 0f;

        // Ensure CanvasGroup for fading
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = image.gameObject.AddComponent<CanvasGroup>();
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


    public void DeactivateItemMenu() {
        itemMenu.SetActive(false);
    }

    public void ActivateItemMenu() {
        itemMenu.SetActive(true);
        currItemIndex = 0;
        currWeapIndex = 0;
        foreach (Button btn in itemButtons) {
            Destroy(btn.gameObject);
        }
        itemButtons.Clear();
    }

    public IEnumerator ItemMenu() {
        // itemMenu.SetActive(true);
        // UnitManager unit = generateGrid.GetGridTile(moveGrid.GetOrgX(), moveGrid.GetOrgZ()).UnitOnTile;
        
        UnitManager unit = moveGrid.GetPlayerCollide().GetPlayer();
        
        
        bool axisInUse = false;
        bool oneAction = false;
        
        CreateMenu(unit);
        int maxIndex = unit.GetWeapons().Count + unit.GetItems().Count - 1 ;
        // int maxIndex = 100;
        // axisInUse = false;
        if (maxIndex >= 0) {
            itemButtons[currItemIndex].Select();
        }

        while (true) {
            Debug.Log(maxIndex + " MAX Cureent" + currItemIndex);
            maxIndex = unit.GetWeapons().Count + unit.GetItems().Count - 1 ;
            // maxIndex = 100;
            float vertical = Input.GetAxis("Vertical");
            // if (gamepad != null) {
            //     vertical = gamepad.leftStick.y.ReadValue();
            // } else {
            //     vertical = Input.GetAxis("Vertical");
            // }

            // Debug.Log(buttons[currentIndex]);

            if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)) && oneAction) {
                inItemMenu = false;
                StartCoroutine(ActivateActionMenu());
                DeactivateItemMenu();
                
                break;
            }

            // if (maxIndex < 0) {
            //     yield return null;
            //     continue;
            // }

           
            

            if (!axisInUse)
            {
                Debug.Log("AXIS NOT IN USE");
                if (vertical > 0.2f) // Move up
                {
                    Debug.Log("UPPPPPPP");
                    itemButtons[currItemIndex].OnDeselect(null);
                    // currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
                    currItemIndex--;
                    if (currItemIndex < 0) { currItemIndex = maxIndex; }
                    // if (currItemIndex < 0) { currItemIndex = 0; }
                    itemButtons[currItemIndex].Select();
                    // EnsureButtonVisible(currItemIndex);
                    axisInUse = true;
                    yield return new WaitForSeconds(0.1f);
                    
                }
                else if (vertical < -0.2f) // Move down
                {
                    Debug.Log("DOWNNNNNNNNN");
                    itemButtons[currItemIndex].OnDeselect(null);
                    // currentIndex = (currentIndex + 1) % buttons.Count;
                    currItemIndex++;
                    if (currItemIndex > maxIndex) { currItemIndex = 0; }
                    // if (currItemIndex > maxIndex) { currItemIndex = maxIndex; }
                    itemButtons[currItemIndex].Select();
                    // EnsureButtonVisible(currItemIndex);
                    axisInUse = true;
                    yield return new WaitForSeconds(0.1f);
                    
                }

                // axisInUse = true; 
                
            }
             if (Mathf.Abs(vertical) < 0.2f)
            {
                Debug.Log("RESET " + vertical);
                axisInUse = false;
            }

            

            if (oneAction && (Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame))) // "Submit" button
            {
                // itemButtons[currItemIndex].onClick.Invoke();
                oneAction = false;
                StartCoroutine(ItemMenuOptionSelect(unit, itemButtons[currItemIndex]));
                break;
            }
            
            oneAction = true;

            yield return null;
        }

        yield return null;
        Debug.Log(unit);
        // yield return null;
    }

    public void CreateMenu(UnitManager user) {
        List<Weapon> weapons = user.GetWeapons();
        List<Item> items = user.GetItems();

        foreach (Button btn in itemButtons) {
            Destroy(btn.gameObject);
        }
        itemButtons = new List<Button>();

        GameObject tempBtn;

        int count = 0;

        if (weapons.Count > 0) {
            for (int i = 0; i < weapons.Count; i++) {
                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);
                // tempBtn.transform.position

                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                count++;
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = weapons[i].WeaponName;

                if(weapons[i] == user.GetPrimaryWeapon()) {
                    texts[0].text += " (E)";
                }

                texts[1].text = weapons[i].Uses + "/" + weapons[i].MaxUses;

                

                itemButtons.Add(tempBtn.GetComponent<Button>());

                // Update the text of the first and second TextMeshPro components
                // if (texts.Length >= 2) {
                //     texts[0].text = weapons[i].WeaponName;        // Set the first text
                //     texts[1].text = weapons[i].description; // Set the second text
                // } else {
                //     Debug.LogWarning("Not enough TextMeshPro components found on button prefab.");
                // }
            }
        }

        if (items.Count > 0) {
            for (int i = 0; i < items.Count; i++) {
                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);

                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                count++;
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = items[i].Name;
                texts[1].text = items[i].Uses + "/" + items[i].MaxUses;
                itemButtons.Add(tempBtn.GetComponent<Button>());

            }
        }

        // for (int i = 0; i < 100; i++) {
        //     tempBtn = (GameObject)Instantiate(buttonTemplate);
        //     tempBtn.transform.SetParent(scrollViewContent.transform, false);

        //     tempBtn.transform.position += new Vector3(0, -count * 45, 0);
        //         count++;
        //         itemButtons.Add(tempBtn.GetComponent<Button>());
        // }

    }

    private IEnumerator ItemMenuOptionSelect(UnitManager user, Button button) {
        int ind = currItemIndex;

        List<Button> optionButtons = new List<Button>();
        List<string> options = new List<string>();
        GameObject tempBtn;
        bool isWeapon = false;
        bool isItem = false;
        

        if (ind < user.GetWeapons().Count) {
            tempBtn = (GameObject)Instantiate(buttonItemOption);
            tempBtn.transform.SetParent(button.transform, false);
            optionButtons.Add(tempBtn.GetComponent<Button>());
            List<Weapon> weps = user.GetWeapons();
            if (weps[ind] == user.primaryWeapon) {
                options.Add("Unequip");
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = "Unequip";
            } else {
                options.Add("Equip");
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = "Equip";
            }
            
        }
        else if (ind < user.GetItems().Count + user.GetWeapons().Count) {
            if (user.GetItems()[ind - user.GetWeapons().Count].CanUse(user)) {
                tempBtn = (GameObject)Instantiate(buttonItemOption);
                tempBtn.transform.SetParent(button.transform, false);
                optionButtons.Add(tempBtn.GetComponent<Button>());
                options.Add("Use");
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = "Use";
            }
        }
        int index = 0;

        if (optionButtons.Count <= 0) {
            StartCoroutine(ItemMenu());
        } else {
            optionButtons[index].Select();
        }

        

        bool axisInUse = false;
        bool oneAction = false;
        

        while (optionButtons.Count > 0) {
            float vertical = Input.GetAxis("Vertical");

            // Debug.Log(buttons[currentIndex]);

            if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)) && oneAction) {

                StartCoroutine(ItemMenu());

                foreach (Button btn in optionButtons) {
                    Destroy(btn.gameObject);
                }
                break;
            }

            if (!axisInUse && optionButtons.Count > 1)
            {
                if (vertical < -0.2f) // Move up
                {
                    // Debug.Log("UPPPPPPP");
                    optionButtons[index].OnDeselect(null);
                    // currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
                    index--;
                    if (index < 0) { index = optionButtons.Count - 1; }
                    optionButtons[index].Select();
                    // EnsureButtonVisible(index);
                    axisInUse = true;
                    // yield return new WaitForSeconds(0.25f);
                }
                else if (vertical > 0.2f) // Move down
                {
                    // Debug.Log("DOWNNNNNNNNN");
                    optionButtons[index].OnDeselect(null);
                    // currentIndex = (currentIndex + 1) % buttons.Count;
                    index++ ;
                    if (index >= optionButtons.Count) { index = 0; }
                    optionButtons[index].Select();
                    // EnsureButtonVisible(index);
                    axisInUse = true;
                    // yield return new WaitForSeconds(0.25f);
                }
            }

            if (Mathf.Abs(vertical) < 0.2f)
            {
                axisInUse = false;
            }

            if (oneAction && (Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame))) // "Submit" button
            {
                if(ind < user.GetWeapons().Count) {
                    if (options[index] == "Equip") {
                        
                        List<Weapon> tempWeap = user.GetWeapons();
                        user.primaryWeapon = tempWeap[ind];
                        foreach (Button btn in optionButtons) {
                            Destroy(btn.gameObject);
                        }
                        StartCoroutine(ItemMenu());
                    } else if (options[index] == "Unequip") {
                        user.primaryWeapon = null;
                        foreach (Button btn in optionButtons) {
                            Destroy(btn.gameObject);
                        }
                        StartCoroutine(ItemMenu());
                    }
                } else if (ind < user.GetItems().Count + user.GetWeapons().Count) {
                    if (options[index] == "Use") {
                        List<Item> tempItems = user.GetItems();
                        DeactivateItemMenu();
                        yield return StartCoroutine(tempItems[ind - user.GetWeapons().Count].Use(user));

                        if(tempItems[ind - user.GetWeapons().Count].Uses <= 0) {
                            user.GetItems().Remove(tempItems[ind - user.GetWeapons().Count]);
                        }
                        
                        PlayerWait(); //Might have to change if wait abilities are implemented
                    }
                }
                break;
            }
            
            oneAction = true;

            yield return null;
        }


        yield return null;


    }

    private void EnsureButtonVisible(int currentIndex) {
        RectTransform buttonRect = itemButtons[currentIndex].GetComponent<RectTransform>();
        RectTransform contentRect = scrollViewContent.GetComponent<RectTransform>();
        RectTransform viewportRect = GameObject.Find("Canvas/ItemMenu/ScrollView").GetComponent<ScrollRect>().viewport;

        // Convert the button's position to the viewport's local space
        Vector3[] buttonCorners = new Vector3[4];
        buttonRect.GetWorldCorners(buttonCorners);

        Vector3[] viewportCorners = new Vector3[4];
        viewportRect.GetWorldCorners(viewportCorners);

        // Check if the button is out of view
        if (buttonCorners[0].y < viewportCorners[0].y + 45 || buttonCorners[1].y > viewportCorners[1].y - 45) {
            // Calculate the offset to move the button into view
            Vector2 viewportLocalPosition = viewportRect.InverseTransformPoint(buttonCorners[0]);
            Vector2 contentLocalPosition = contentRect.anchoredPosition;
            if (buttonCorners[0].y < viewportCorners[0].y + 45) {
                contentLocalPosition.y += 45;
            } else if (buttonCorners[1].y > viewportCorners[1].y - 45) {
                contentLocalPosition.y -= 45;
            }
            
            contentRect.anchoredPosition = contentLocalPosition;
        }
    }


    //------------------------------------------WEapon List----------------------------------------




    public void CheckWeapons(UnitManager unit) {
        
        List<Weapon> tempWeap = unit.GetWeapons();
        usableWeapons.Clear();
        nonUsableWeapons.Clear();

        foreach (Weapon wep in tempWeap) {
            bool[,] attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), wep.Range, wep.Range1, wep.Range2, wep.Range3);
            for (int i = 0; i < generateGrid.GetWidth(); i++) {
                for (int j = 0; j < generateGrid.GetLength(); j++) {
                    if (attackGrid[i,j] && !usableWeapons.Contains(wep) && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                        
                        // UnitsInRange.Add(generateGrid.GetGridTile(i,j));
                        usableWeapons.Add(wep);
                        
                    }
                }
            }

            if(!usableWeapons.Contains(wep)) {
                nonUsableWeapons.Add(wep);
            }
        }
    }

    public IEnumerator WeaponList(UnitManager unit) {
        CheckWeapons(unit);
        ActivateItemMenu();
        CreateWeaponMenu(unit);

        int weapIndex = 0;

        bool axisInUse = false;
        bool oneAction = false;
        currWeapIndex = 0;
        int maxIndex;
        moveGrid.isAttacking = false;

        bool[,] attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), listOfWeapons[currWeapIndex].Range, listOfWeapons[currWeapIndex].Range1, listOfWeapons[currWeapIndex].Range2, listOfWeapons[currWeapIndex].Range3);

        itemButtons[currWeapIndex].Select();

        findPath.DestroyRange();
        attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), listOfWeapons[currWeapIndex].Range, listOfWeapons[currWeapIndex].Range1, listOfWeapons[currWeapIndex].Range2, listOfWeapons[currWeapIndex].Range3);
        findPath.HighlightAttack( attackGrid);


        while (true) {
            maxIndex = usableWeapons.Count + nonUsableWeapons.Count - 1;
            // maxIndex = 100;
            float vertical = Input.GetAxis("Vertical");

            // Debug.Log(buttons[currentIndex]);

            if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame)) && oneAction) {
                StartCoroutine(ActivateActionMenu());
                DeactivateItemMenu();
                break;
            }

            if (maxIndex < 0) {
                yield return null;
                continue;
            }
            

            if (!axisInUse)
            {
                if (vertical > 0.2f) // Move up
                {
                    itemButtons[currWeapIndex].OnDeselect(null);
                    // currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
                    currWeapIndex--;
                    if (currWeapIndex < 0) { currWeapIndex = maxIndex; }
                    // if (currItemIndex < 0) { currItemIndex = 0; }
                    itemButtons[currWeapIndex].Select();
                    // EnsureButtonVisible(currWeapIndex);
                    axisInUse = true;
                    findPath.DestroyRange();
                    attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), listOfWeapons[currWeapIndex].Range, listOfWeapons[currWeapIndex].Range1, listOfWeapons[currWeapIndex].Range2, listOfWeapons[currWeapIndex].Range3);
                    findPath.HighlightAttack( attackGrid);

                    // yield return new WaitForSeconds(0.25f);
                }
                else if (vertical < -0.2f) // Move down
                {
                    itemButtons[currWeapIndex].OnDeselect(null);
                    // currentIndex = (currentIndex + 1) % buttons.Count;
                    currWeapIndex++ ;
                    if (currWeapIndex > maxIndex) { currWeapIndex = 0; }
                    // if (currItemIndex > maxIndex) { currItemIndex = maxIndex; }
                    itemButtons[currWeapIndex].Select();
                    // EnsureButtonVisible(currWeapIndex);
                    axisInUse = true;
                    findPath.DestroyRange();
                    attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), listOfWeapons[currWeapIndex].Range, listOfWeapons[currWeapIndex].Range1, listOfWeapons[currWeapIndex].Range2, listOfWeapons[currWeapIndex].Range3);
                    findPath.HighlightAttack( attackGrid);

                    // yield return new WaitForSeconds(0.25f);
                }
            }

            if (Mathf.Abs(vertical) < 0.2f)
            {
                axisInUse = false;
            }

            if (oneAction && (Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame))) // "Submit" button
            {
                // itemButtons[currItemIndex].onClick.Invoke();
                oneAction = false;
                List<GridTile> UnitsInRange = new List<GridTile>();
                for (int i = 0; i < generateGrid.GetWidth(); i++) {
                    for (int j = 0; j < generateGrid.GetLength(); j++) {
                        if (attackGrid[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                            
                            UnitsInRange.Add(generateGrid.GetGridTile(i,j));
                            
                        }
                    }
                }
                if(UnitsInRange.Count <= 0) { continue; }
                DeactivateItemMenu();
                StartCoroutine(executeAction.CycleAttackList(UnitsInRange, listOfWeapons[currWeapIndex]));
                break;
            }
            
            oneAction = true;

            yield return null;
        }

        yield return null;

    }

    public void CreateWeaponMenu(UnitManager user) {
        // List<Weapon> weapons = user.GetWeapons();
        listOfWeapons = new List<Weapon>();
        listOfWeapons.AddRange(usableWeapons);
        listOfWeapons.AddRange(nonUsableWeapons);
        // List<Item> items = user.GetItems();

        foreach (Button btn in itemButtons) {
            Destroy(btn.gameObject);
        }
        itemButtons = new List<Button>();

        GameObject tempBtn;

        int count = 0;

        Debug.Log(listOfWeapons.Count);

        if (listOfWeapons.Count > 0) {
            for (int i = 0; i < listOfWeapons.Count; i++) {
                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);
                // tempBtn.transform.position

                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                // itemButton
                count++;
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = listOfWeapons[i].WeaponName;


                if(listOfWeapons[i] == user.GetPrimaryWeapon()) {
                    texts[0].text += " (E)";
                }

                texts[1].text = listOfWeapons[i].Uses + "/" + listOfWeapons[i].MaxUses;
                itemButtons.Add(tempBtn.GetComponent<Button>());

                if (nonUsableWeapons.Contains(listOfWeapons[i])) {
                    itemButtons[i].interactable = false;
                }

                
                
                // itemButtons.Add(tempBtn.GetComponent<Button>());
            }
        }
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