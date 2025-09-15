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
    // private UnitManager DefendingEnemy;
    // private UnitManager AttackingUnit;
    // public Transform moveCursor;
    // private int attackerX;
    // private int attackerZ;
    // private int defenderX;
    // private int defenderZ;

    [Header("Action Menu")]
    [SerializeField] GameObject attackButton;
    [SerializeField]  GameObject itemButton;
    [SerializeField]  GameObject waitButton;
    [SerializeField] GameObject assistButton;
    List<GameObject> actionMenuList;


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



    //ItemMenu
    [Header("Item Menu Menu")]
    private GameObject scrollViewContent;
    [SerializeField] GameObject buttonTemplate;
    [SerializeField] GameObject itemDivider;
    List<GameObject> divList = new List<GameObject>();
    private GameObject itemMenu;
    List<Button> itemButtons;
    int currItemIndex = 0;
    [SerializeField] GameObject buttonItemOption;

    List<Weapon> usableWeapons;
    List<Weapon> nonUsableWeapons;
    List<Weapon> listOfWeapons;
    List<Faith> usableFaith;
    List<Faith> nonUsableFaith;
    int currWeapIndex = 0;


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

        // Item Menu
        scrollViewContent = GameObject.Find("Canvas/ItemMenu/ScrollView/Viewport/Content");
        itemMenu = GameObject.Find("Canvas/ItemMenu");

        DeactivateItemMenu();

        itemButtons = new List<Button>();
        usableWeapons = new List<Weapon>();
        nonUsableWeapons = new List<Weapon>();
        listOfWeapons = new List<Weapon>();
        usableFaith = new List<Faith>();
        nonUsableFaith = new List<Faith>();

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


    public void DeactivateActionMenu() {
        foreach(GameObject obj in actionMenuList) Destroy(obj);
    }

    public void PlayerWait() {
        // Removes player from actives list
        manageTurn.RemovePlayer(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile.GetStats());

        // Calls the unit wait function
        executeAction.unitWait();

        // Checks the phase and clear condition and if it has been met

        manageTurn.AfterAction(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile);
        // _currentMap.CheckClearCondition();
    }

    private IEnumerator ShowActionMenu(List<(string Label, Action OnSelect)> menuItems, List<int> CPCost)
    {
        List<Button> buttons = new List<Button>();
        int startX = 700;
        int startY = 400;
        int ind = 0;

        GameObject actionMenu = GameObject.Find("Canvas/ActionMenu");
        actionMenuList = new List<GameObject>();

        for (int i = 0; i < menuItems.Count; i++)
        {
            var item = menuItems[i];

            // Instantiate appropriate button prefab
            GameObject prefab = item.Label switch
            {
                "Attack" => attackButton,
                "Assist" => assistButton,
                "Item"   => itemButton,
                "Wait"   => waitButton,
                _        => itemButton // fallback
            };

            GameObject tempBtn = Instantiate(prefab);
            tempBtn.transform.SetParent(actionMenu.transform, false);
            tempBtn.transform.position += new Vector3(startX, startY + (-ind * 100), 0);

            // Update button text (append cost if provided)
            var texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
            bool selectable = true;

            if (texts.Length > 0)
            {
                string labelText = item.Label;
                if (CPCost != null && i < CPCost.Count)
                {
                    labelText += $" ({CPCost[i]})";

                    // If cost is too high, disable selection
                    if (CPCost[i] > manageTurn.GetCP())
                    {
                        selectable = false;
                        // Optionally make it look disabled
                        texts[0].color = Color.gray;
                    }
                }
                texts[0].text = labelText;
            }

            Button btn = tempBtn.GetComponent<Button>();
            if (!selectable)
            {
                btn.interactable = false;
            }

            buttons.Add(btn);
            actionMenuList.Add(tempBtn);
            ind++;

        }

        int currentIndex = 0;
        buttons[currentIndex].Select();

        bool axisInUse = false;
        bool oneAction = false;

        while (true)
        {
            float vertical = moveInput.y;

            if (!axisInUse)
            {
                if (vertical > sensitivity)
                {
                    buttons[currentIndex].OnDeselect(null);
                    currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
                    buttons[currentIndex].Select();
                    axisInUse = true;
                }
                else if (vertical < -sensitivity)
                {
                    buttons[currentIndex].OnDeselect(null);
                    currentIndex = (currentIndex + 1) % buttons.Count;
                    buttons[currentIndex].Select();
                    axisInUse = true;
                }
            }

            if (Mathf.Abs(vertical) < sensitivity) axisInUse = false;

            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame())
            {
                if (buttons[currentIndex].interactable) // Only allow if button is interactable
                {
                    if (CPCost != null && currentIndex < CPCost.Count) {
                        manageTurn.SetCurrentActionCost(CPCost[currentIndex]);
                    }
                    menuItems[currentIndex].OnSelect?.Invoke();
                    break;
                }
            }

            if (oneAction && playerInput.actions["Back"].WasPressedThisFrame())
            {
                DeactivateActionMenu();
                moveGrid.OutOfMenu();
                break;
            }

            oneAction = true;
            yield return null;
        }
    }

    

    public IEnumerator ActivateActionMenu()
    {
        UnitManager unit = generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile;
        CheckWeapons(unit as PlayerUnit);
        CheckFaith(unit as PlayerUnit);

        var items = new List<(string, Action)>();
        List<int> CPCost = new List<int>();

        int tCos = 0;

        if (usableWeapons.Count > 0) {
            items.Add(("Attack", () => { DeactivateActionMenu(); PlayerAttack(); }));
            tCos = 1;
            if (moveGrid.DidUnitMove()) tCos++;
            CPCost.Add(GetCPCost(tCos, unit));
        }

        if (usableFaith.Count > 0) {
            items.Add(("Assist", () => { DeactivateActionMenu(); PlayerAssist(); }));
            tCos = 1;
            if (moveGrid.DidUnitMove()) tCos++;
            CPCost.Add(GetCPCost(tCos, unit));
        }

        items.Add(("Item", () => { DeactivateActionMenu(); PlayerItem(); }));
        tCos = 1;
        if (moveGrid.DidUnitMove()) tCos++;
        CPCost.Add(GetCPCost(tCos, unit));

        tCos = 0;
        if (moveGrid.DidUnitMove()) tCos++;
        if ((_currentMap.UsingCP() && moveGrid.DidUnitMove()) || !_currentMap.UsingCP()) {
            items.Add(("Wait", () => { DeactivateActionMenu(); PlayerWait(); }));
            CPCost.Add(GetCPCost(1, unit));
        }

        yield return ShowActionMenu(items, CPCost);
    }

    // Calculate for CP cost when the unit moved already this turn
    public int GetCPCost(int baseCost, UnitManager unit)
    {
        int unitActionCount = unit.GetNumberTimesActed();
        return baseCost * (int)Mathf.Pow(2, unitActionCount);
    }


    // Passive menu for when a player selects a space without a unit
    // Right now only used for Back and End Turn however more will be added later
    public IEnumerator PassiveMenu()
    {
        var items = new List<(string, Action)>
        {
            ("Back",    () => { DeactivateActionMenu(); moveGrid.inMenu = false; }),
            ("End Turn", () => { DeactivateActionMenu(); manageTurn.EndTurn(); moveGrid.inMenu = false; })
        };

        yield return ShowActionMenu(items, null);
    }


//^-------------------------------------------------------ATTACK---------------------------------------------------------------------
    public void PlayerAttack() => StartCoroutine(WeaponList(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile));

    // This Method checks which weapons have an enemy within its range and adds it to the usable weapons list
    public void CheckWeapons(UnitManager unit) {
        
        // Gets Weapons that unit has in invintory
        List<Weapon> tempWeap = unit.GetWeapons();

        // Resets the usable and non usable list, nonUsable are for weapons that can't attack anything in range
        usableWeapons.Clear();
        nonUsableWeapons.Clear();

        // For each weapon determine if theres an enemy in the units range, if so add to usable weapon, if not then non usable
        foreach (Weapon wep in tempWeap) { 
            bool[,] attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), wep.Range, wep.Range1, wep.Range2, wep.Range3);
            for (int i = 0; i < generateGrid.GetWidth(); i++) {
                for (int j = 0; j < generateGrid.GetLength(); j++) {
                    if (attackGrid[i,j] && !usableWeapons.Contains(wep) && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
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
        // Checks which weapons are avaliable
        CheckWeapons(unit);

        // Creates the weapon menu
        CreateWeaponMenu(unit);

        
        bool axisInUse = false;
        bool oneAction = false;

        currWeapIndex = 0;

        int maxIndex;

        moveGrid.isAttacking = false;

        

        itemButtons[currWeapIndex].Select();

        findPath.DestroyRange();

        // Highlights attack for first weapon
        bool[,] attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), listOfWeapons[currWeapIndex].Range, listOfWeapons[currWeapIndex].Range1, listOfWeapons[currWeapIndex].Range2, listOfWeapons[currWeapIndex].Range3);
        findPath.HighlightAttack(attackGrid);


        while (true) {
            // Gets the max index
            maxIndex = usableWeapons.Count + nonUsableWeapons.Count - 1;

            float vertical = moveInput.y; 

            if (playerInput.actions["Back"].WasPressedThisFrame() && oneAction) {
                // Destroys path and sends it back to the action menu
                findPath.DestroyRange();
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
                if (vertical > sensitivity) {
                    // Deselects old button
                    itemButtons[currWeapIndex].OnDeselect(null);

                    // Change index and select button
                    currWeapIndex--;
                    if (currWeapIndex < 0) { currWeapIndex = maxIndex; }
                    itemButtons[currWeapIndex].Select();

                    // Creates a new attack range for the next weapon
                    findPath.DestroyRange();
                    attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), listOfWeapons[currWeapIndex].Range, listOfWeapons[currWeapIndex].Range1, listOfWeapons[currWeapIndex].Range2, listOfWeapons[currWeapIndex].Range3);
                    findPath.HighlightAttack(attackGrid);

                    axisInUse = true;
                }
                else if (vertical < -sensitivity) {
                    // Deselects old button
                    itemButtons[currWeapIndex].OnDeselect(null);

                    // Change index and select button
                    currWeapIndex++ ;
                    if (currWeapIndex > maxIndex) { currWeapIndex = 0; }
                    itemButtons[currWeapIndex].Select();

                    // Creates a new attack range for the next weapon
                    findPath.DestroyRange();
                    attackGrid = findPath.CalculateAttack(moveGrid.getX(), moveGrid.getZ(), listOfWeapons[currWeapIndex].Range, listOfWeapons[currWeapIndex].Range1, listOfWeapons[currWeapIndex].Range2, listOfWeapons[currWeapIndex].Range3);
                    findPath.HighlightAttack(attackGrid);

                    axisInUse = true;
                }
            }

            if (Mathf.Abs(vertical) < sensitivity) axisInUse = false;

            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame()) {
                oneAction = false;

                // Gets all enemies in range and begins the cycle attack method
                List<GridTile> UnitsInRange = new List<GridTile>();
                for (int i = 0; i < generateGrid.GetWidth(); i++) {
                    for (int j = 0; j < generateGrid.GetLength(); j++) {
                        if (attackGrid[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                            UnitsInRange.Add(generateGrid.GetGridTile(i,j));  
                        }
                    }
                }

                if(UnitsInRange.Count <= 0) continue;

                DeactivateItemMenu();
                StartCoroutine(executeAction.CycleAttackList(UnitsInRange, listOfWeapons[currWeapIndex]));

                break;
            }
            
            oneAction = true;

            yield return null;
        }

        yield return null;
    }

    // Creates the Weapon Menu
    // TODO: Add dividers for Magic when you eventually get that running
    public void CreateWeaponMenu(UnitManager user) {

        // Activates the Item menu background
        ActivateItemMenu();

        // Adds the full list of weapons, making sure that nonUsable is at the bottom
        listOfWeapons = new List<Weapon>();
        listOfWeapons.AddRange(usableWeapons);
        listOfWeapons.AddRange(nonUsableWeapons);

        // Destroys previous buttons and dividers
        foreach (Button btn in itemButtons) Destroy(btn.gameObject);
        foreach (GameObject obj in divList) Destroy(obj);

        // Creates a new list of item buttons
        itemButtons = new List<Button>();

        GameObject tempBtn;
        int count = 0;

        if (listOfWeapons.Count > 0) {
            for (int i = 0; i < listOfWeapons.Count; i++) {
                // Instantiates weapon button on the menu
                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);

                // Sets its position under the rpevious one
                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                count++;

                // Sets text to weapon name, adds (E) if its equipped
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = listOfWeapons[i].WeaponName;
                if(listOfWeapons[i] == user.GetPrimaryWeapon()) {
                    texts[0].text += " (E)";
                }

                // Sets the current uses and max uses
                if (listOfWeapons[i].MaxUses > 0) {
                    texts[1].text = listOfWeapons[i].Uses + "/" + listOfWeapons[i].MaxUses;
                } else {
                    texts[1].text = "";
                }

                // Adds to item button list
                itemButtons.Add(tempBtn.GetComponent<Button>());

                // If the weapon can't be used make buton uninteractable
                if (nonUsableWeapons.Contains(listOfWeapons[i])) {
                    itemButtons[i].interactable = false;
                }
            }
        }
    }

//^-------------------------------------------------------------ITEMS------------------------------------------------
    


    public void PlayerAssist() => StartCoroutine(AssistMenu());

    // Checks which faiths can be used
    void CheckFaith(UnitManager unit) {
        List<Faith> checking =  unit.GetFaith();
        usableFaith = new List<Faith>();
        nonUsableFaith = new List<Faith>();

        foreach(Faith fai in checking) {
            if(fai.CanUse(unit)) {
                usableFaith.Add(fai);
            } else {
                nonUsableFaith.Add(fai);
            }
        }
    }


    public IEnumerator AssistMenu() {
        // checks which faiths can be used
        CheckFaith(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile);

        // activates the item menu
        ActivateItemMenu();

        // creates an assist menu
        CreateAssistMenu();

        // instantiates max index and cur assist index
        int maxIndex = usableFaith.Count + nonUsableFaith.Count - 1;
        int curAssistIndex = 0;

        bool axisInUse = false;
        bool oneAction = false;

        moveGrid.isAttacking = false;

        itemButtons[curAssistIndex].Select();

        while(true) {

            float vertical = moveInput.y;

            // Debug.Log(buttons[currentIndex]);

            if (playerInput.actions["Back"].WasPressedThisFrame() && oneAction) {
                StartCoroutine(ActivateActionMenu());
                DeactivateItemMenu();
                break;
            }

            if (maxIndex < 0) {
                yield return null;
                continue;
            }
            

            if (!axisInUse) {
                if (vertical > sensitivity) {
                    itemButtons[curAssistIndex].OnDeselect(null);
                    curAssistIndex--;
                    if (curAssistIndex < 0) curAssistIndex = maxIndex;
                    itemButtons[curAssistIndex].Select();
                    axisInUse = true;
                }
                else if (vertical < -sensitivity) {
                    itemButtons[curAssistIndex].OnDeselect(null);
                    curAssistIndex++ ;
                    if (curAssistIndex > maxIndex) curAssistIndex = 0;
                    itemButtons[curAssistIndex].Select();
                    axisInUse = true;
                }
            }

            if (Mathf.Abs(vertical) < sensitivity) axisInUse = false;
            

            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame() && curAssistIndex < usableFaith.Count) // "Submit" button
            {
                oneAction = false;
                List<GridTile> temp = new List<GridTile>();
                temp = usableFaith[curAssistIndex].GetUnitsInRange(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile);

                if(temp.Count <= 0) continue;

                
                DeactivateItemMenu();
                StartCoroutine(executeAction.CycleAssist(temp, usableFaith[curAssistIndex]));
                break;
            }
            
            oneAction = true;

            yield return null;
        }
    }

    void CreateAssistMenu() {
        List<Faith> listOfFaith = new List<Faith>();
        listOfFaith.AddRange(usableFaith);
        listOfFaith.AddRange(nonUsableFaith);

        foreach (Button btn in itemButtons) Destroy(btn.gameObject);
        foreach (GameObject obj in divList) Destroy(obj);

        itemButtons = new List<Button>();

        GameObject tempBtn;

        int count = 0;

        if (listOfFaith.Count > 0) {
            for (int i = 0; i < listOfFaith.Count; i++) {
                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);

                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                count++;
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = listOfFaith[i].Name;

                texts[1].text = listOfFaith[i].Uses + "/" + listOfFaith[i].MaxUses;
                itemButtons.Add(tempBtn.GetComponent<Button>());

                if (nonUsableFaith.Contains(listOfFaith[i])) itemButtons[i].interactable = false;
            }
        }
    }

    //^------------------------------------ITEM---------------------------------------------------------------------
    public void PlayerItem()
    {
        currItemIndex = 0;
        StartCoroutine(ItemMenu());
    }
    
    public void DeactivateItemMenu() => itemMenu.SetActive(false);

    public void ActivateItemMenu() {
        itemMenu.SetActive(true);
        currWeapIndex = 0;
        foreach (Button btn in itemButtons) Destroy(btn.gameObject);
        foreach (GameObject obj in divList) Destroy(obj);
        itemButtons.Clear();
    }

    public IEnumerator ItemMenu() {
        ActivateItemMenu();

        
        UnitManager unit = generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile;
        
        
        bool axisInUse = false;
        bool oneAction = false;
        
        CreateMenu(unit);
        int maxIndex = unit.GetWeapons().Count + unit.GetItems().Count + unit.GetFaith().Count - 1 ;

        if (maxIndex >= 0) itemButtons[currItemIndex].Select();

        while (true) {
            maxIndex = unit.GetWeapons().Count + unit.GetItems().Count + unit.GetFaith().Count - 1 ;
            float vertical = moveInput.y;

            if (playerInput.actions["Back"].WasPressedThisFrame() && oneAction) {
                StartCoroutine(ActivateActionMenu());
                DeactivateItemMenu();
                
                break;
            }

            if (!axisInUse) {
                if (vertical > 0.2f) {
                    itemButtons[currItemIndex].OnDeselect(null);
                    currItemIndex--;
                    if (currItemIndex < 0) currItemIndex = maxIndex;
                    itemButtons[currItemIndex].Select();
                    axisInUse = true;
                    
                }
                else if (vertical < -0.2f) {
                    itemButtons[currItemIndex].OnDeselect(null);
                    currItemIndex++;
                    if (currItemIndex > maxIndex) currItemIndex = 0;
                    itemButtons[currItemIndex].Select();
                    axisInUse = true;
                }
            }

            if (Mathf.Abs(vertical) < 0.2f) axisInUse = false;


            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame()) {
                StartCoroutine(ItemMenuOptionSelect(unit, itemButtons[currItemIndex]));
                break;
            }
            
            oneAction = true;

            yield return null;
        }

        yield return null;
    }

    public void CreateMenu(UnitManager user) {
        List<Weapon> weapons = user.GetWeapons();
        List<Item> items = user.GetItems();
        List<Faith> faith = user.GetFaith();

        foreach (Button btn in itemButtons) Destroy(btn.gameObject);
        foreach (GameObject obj in divList) Destroy(obj);

        itemButtons = new List<Button>();
        divList = new List<GameObject>();

        GameObject tempBtn;
        int count = 0;

        

        if (weapons.Count > 0)
        {
            GameObject tempDiv = (GameObject)Instantiate(itemDivider);
            tempDiv.transform.SetParent(scrollViewContent.transform, false);
            tempDiv.transform.position += new Vector3(0, -count * 45, 0);
            count++;
            tempDiv.GetComponent<TextMeshProUGUI>().text = "Weapons";
            divList.Add(tempDiv);

            Weapon prim = user.GetPrimaryWeapon();

            if (prim != null)
            {

                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);

                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                count++;
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = prim.WeaponName;

                if (prim == user.GetPrimaryWeapon()) texts[0].text += " (E)";
                texts[1].text = prim.Uses + "/" + prim.MaxUses;

                itemButtons.Add(tempBtn.GetComponent<Button>());
            }
            for (int i = 0; i < weapons.Count; i++)
            {
                List<Weapon> temp1 = user.GetPhysicalWeapons();
                List<Weapon> temp2 = user.GetMagicList();

                foreach (Weapon t in temp1)
                {
                    Debug.LogError("W: " + t.WeaponName);
                }
                foreach (Weapon t in temp2)
                {
                    Debug.LogError("M: " + t.WeaponName);
                }
                if (weapons[i] == user.GetPrimaryWeapon()) continue;
                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);

                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                count++;
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = weapons[i].WeaponName;

                // if (weapons[i] == user.GetPrimaryWeapon()) texts[0].text += " (E)";
                texts[1].text = weapons[i].Uses + "/" + weapons[i].MaxUses;

                itemButtons.Add(tempBtn.GetComponent<Button>());

            }
        }

        // if (magic.Count > 0) {
        //     GameObject tempDiv = (GameObject)Instantiate(itemDivider);
        //     tempDiv.transform.SetParent(scrollViewContent.transform, false);
        //     tempDiv.transform.position += new Vector3(0, -count * 45, 0);
        //     count++;
        //     tempDiv.GetComponent<TextMeshProUGUI>().text = "Magic";
        //     divList.Add(tempDiv);
        //     for (int i = 0; i < magic.Count; i++) {
        //         tempBtn = (GameObject)Instantiate(buttonTemplate);
        //         tempBtn.transform.SetParent(scrollViewContent.transform, false);

        //         tempBtn.transform.position += new Vector3(0, -count * 45, 0);
        //         count++;
        //         TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
        //         texts[0].text = magic[i].WeaponName;

        //         if(magic[i] == user.GetPrimaryWeapon()) texts[0].text += " (E)";
        //         texts[1].text = magic[i].Uses + "/" + magic[i].MaxUses;

        //         itemButtons.Add(tempBtn.GetComponent<Button>());

        //     }
        // }

        if (faith.Count > 0) {
            GameObject tempDiv = (GameObject)Instantiate(itemDivider);
            tempDiv.transform.SetParent(scrollViewContent.transform, false);
            tempDiv.transform.position += new Vector3(0, -count * 45, 0);
            count++;
            tempDiv.GetComponent<TextMeshProUGUI>().text = "Faith";
            divList.Add(tempDiv);
            for (int i = 0; i < faith.Count; i++) {
                tempBtn = (GameObject)Instantiate(buttonTemplate);
                tempBtn.transform.SetParent(scrollViewContent.transform, false);

                tempBtn.transform.position += new Vector3(0, -count * 45, 0);
                count++;
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = faith[i].Name;
                texts[1].text = faith[i].Uses + "/" + faith[i].MaxUses;
                itemButtons.Add(tempBtn.GetComponent<Button>());
            }
        }

        if (items.Count > 0) {
            GameObject tempDiv = (GameObject)Instantiate(itemDivider);
            tempDiv.transform.SetParent(scrollViewContent.transform, false);
            tempDiv.transform.position += new Vector3(0, -count * 45, 0);
            count++;
            tempDiv.GetComponent<TextMeshProUGUI>().text = "Items";
            divList.Add(tempDiv);
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
    }

    private IEnumerator ItemMenuOptionSelect(UnitManager user, Button button) {
        int ind = currItemIndex;

        List<Button> optionButtons = new List<Button>();
        List<string> options = new List<string>();
        GameObject tempBtn;


        if (ind < user.GetWeapons().Count)
        {
            tempBtn = (GameObject)Instantiate(buttonItemOption);
            tempBtn.transform.SetParent(button.transform, false);
            optionButtons.Add(tempBtn.GetComponent<Button>());
            List<Weapon> weps = user.GetWeapons();
            if (weps[ind] == user.GetPrimaryWeapon())
            {
                options.Add("Unequip");
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = "Unequip";
            }
            else
            {
                options.Add("Equip");
                TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = "Equip";
            }
            // if (weps[ind].WeaponType != "Magic" || weps[ind].WeaponType != "Faith") {
            //     tempBtn.transform.position = new Vector3(tempBtn.transform.position.x, tempBtn.transform.position.y + 45, tempBtn.transform.position.z);
            //     tempBtn = (GameObject)Instantiate(buttonItemOption);
            //     tempBtn.transform.SetParent(button.transform, false);
            //     optionButtons.Add(tempBtn.GetComponent<Button>());
            //     options.Add("Discard");
            //     TextMeshProUGUI[] texts = tempBtn.GetComponentsInChildren<TextMeshProUGUI>();
            //     texts[0].text = "Discard";
            // }

        }
        //! This is causing problems with faith, please fix
        else if (ind < user.GetFaith().Count + user.GetWeapons().Count)
        {
            
        }
        else if (ind < user.GetItems().Count + user.GetFaith().Count + user.GetWeapons().Count)
        {
            if (user.GetItems().Count > 0 && user.GetItems()[ind - user.GetWeapons().Count - user.GetFaith().Count].CanUse(user))
            {
                Debug.LogError("USEEE");
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
                    if (options[index] == "Equip")
                    {

                        List<Weapon> tempWeap = user.GetWeapons();
                        user.SetPrimaryWeapon(tempWeap[ind]);
                        foreach (Button btn in optionButtons)
                        {
                            Destroy(btn.gameObject);
                        }
                        currItemIndex = 0;
                            
                        StartCoroutine(ItemMenu());
                    }
                    else if (options[index] == "Unequip")
                    {
                        user.SetPrimaryWeapon(null);
                        foreach (Button btn in optionButtons)
                        {
                            Destroy(btn.gameObject);
                        }
                        // currItemIndex = 0;

                        StartCoroutine(ItemMenu());
                    }
                    else if (options[index] == "Discard")
                    {
                        foreach (Button btn in optionButtons)
                        {
                            Destroy(btn.gameObject);
                        }
                        currItemIndex = 0;

                        StartCoroutine(ItemMenu());
                        
                    }
                } else if (ind < user.GetItems().Count + user.GetWeapons().Count + user.GetFaith().Count) {
                    if (options[index] == "Use") {
                        Debug.LogError("USEEE");
                        List<Item> tempItems = user.GetItems();
                        DeactivateItemMenu();
                        yield return StartCoroutine(tempItems[ind - user.GetWeapons().Count - user.GetFaith().Count].Use(user));

                        if(tempItems[ind - user.GetWeapons().Count - user.GetFaith().Count].Uses <= 0) {
                            user.GetItems().Remove(tempItems[ind - user.GetWeapons().Count - user.GetFaith().Count]);
                            Debug.LogError("AHHHHHHH " + user.GetItems().Count);
                        }
                        currItemIndex = 0;

                        
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

    // public void ActivateExperienceMenu() {
    //     experienceMenu.SetActive(true);
    // }

    // public IEnumerator GainExperienceMenu(UnitManager unit, int gainExp, string skillType1, int skillInc, int SPInc)
    // {
    //     experienceMenu.SetActive(true);
    //     expUnitName.text = unit.GetName();

    //     int currentExp = unit.GetExperience(); // Starting experience
    //     int remainingExp = gainExp;             // Experience to be added
    //     int expThreshold = 100;                 // Max experience for a level
    //     int initialExpNext = expThreshold - currentExp; // Initial experience needed to level up

    //     expGained.text = "+" + remainingExp.ToString();
    //     expNext.text = initialExpNext.ToString();

    //     while (remainingExp > 0)
    //     {
    //         // Calculate how much experience can be added in this cycle
    //         int expToAdd = Mathf.Min(expThreshold - currentExp, remainingExp);
    //         float startFill = (float)currentExp / expThreshold;
    //         float targetFill = (float)(currentExp + expToAdd) / expThreshold;

    //         float duration = (targetFill - startFill) * 1.5f;
    //         float elapsed = 0f;

    //         expBar.fillAmount = startFill;

    //         // Animate the bar fill
    //         while (elapsed < duration)
    //         {
    //             elapsed += Time.deltaTime;
    //             float t = elapsed / duration;

    //             // Update bar fill
    //             expBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

    //             // Dynamically update expGained and expNext
    //             int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(remainingExp, remainingExp - expToAdd, t));
    //             int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(initialExpNext, initialExpNext - expToAdd, t));

    //             expGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : interpolatedGain.ToString();
    //             expNext.text = interpolatedNext.ToString();

    //             if (playerInput.actions["SkipCutscene"].WasPressedThisFrame()) break;

    //             yield return null; // Wait for the next frame
    //         }

    //         // Finalize this cycle
    //         expBar.fillAmount = targetFill;
    //         currentExp += expToAdd;    // Temporarily track current experience for UI
    //         remainingExp -= expToAdd; // Reduce remaining experience

    //         // Reset bar if threshold is reached
    //         if (currentExp >= expThreshold && remainingExp > 0)
    //         {
    //             currentExp = 0;        // Reset current experience
    //             initialExpNext = expThreshold; // Reset expNext display
    //         }
    //     }

    //     // Set final UI state
    //     expGained.text = "0"; // Clear gained experience
    //     expNext.text = (expThreshold - currentExp).ToString(); // Remaining experience for next level

    //     yield return new WaitForSeconds(1f);
    //     DeactivateExperienceMenu();
    // }
    // public IEnumerator GainExperienceMenu(UnitManager unit, int gainExp, string skillType1, int skillInc, int SPInc)
    // {
    //     experienceMenu.SetActive(true);
    //     expUnitName.text = unit.GetName();

    //     // ===== EXP SETUP =====
    //     int currentExp = unit.GetExperience();
    //     int remainingExp = gainExp;
    //     int expThreshold = 100;
    //     int initialExpNext = expThreshold - currentExp;

    //     expGained.text = "+" + remainingExp.ToString();
    //     expNext.text = initialExpNext.ToString();

    //     // ===== SKILL SETUP =====
    //     // int currentSkillExp = unit.GetStats().GetSkillExperience(skillType1);
    //     // int remainingSkillExp = skillInc;
    //     // int skillThreshold = 100;
    //     // int initialSkillNext = skillThreshold - currentSkillExp;

    //     int currentSkillExp = unit.GetStats().GetSkillExperience(skillType1);
    //     int remainingSkillExp = skillInc;

    //     // Use your static method instead of hardcoding 100
    //     int skillThreshold = UnitRosterManager.GetNextLevelExpRequirement(currentSkillExp);
    //     int initialSkillNext = UnitRosterManager.GetExpToNextLevel(currentSkillExp);


    //     skillName.text = skillType1;
    //     skillGained.text = "+" + remainingSkillExp.ToString();
    //     skillNext.text = initialSkillNext.ToString();

    //     // ===== SP SETUP (instant) =====
    //     int currentSP = unit.GetStats().SP;
    //     currentSP += SPInc;
    //     int spThreshold = 100; // optional
    //     int finalSPNext = spThreshold - currentSP;

    //     SPGained.text = "0";
    //     SPNext.text = finalSPNext.ToString();

    //     // ===== LOOP: animate EXP + SKILL together =====
    //     while (remainingExp > 0 || remainingSkillExp > 0)
    //     {
    //         int expToAdd = Mathf.Min(expThreshold - currentExp, remainingExp);
    //         int skillToAdd = Mathf.Min(skillThreshold - currentSkillExp, remainingSkillExp);

    //         float startFillExp = (float)currentExp / expThreshold;
    //         float targetFillExp = (float)(currentExp + expToAdd) / expThreshold;

    //         float startFillSkill = (float)currentSkillExp / skillThreshold;
    //         float targetFillSkill = (float)(currentSkillExp + skillToAdd) / skillThreshold;

    //         float duration = Mathf.Max(
    //             (targetFillExp - startFillExp),
    //             (targetFillSkill - startFillSkill)
    //         ) * 1.5f; // longest needed duration
    //         float elapsed = 0f;

    //         // Animate both bars simultaneously
    //         while (elapsed < duration)
    //         {
    //             elapsed += Time.deltaTime;
    //             float t = elapsed / duration;

    //             // EXP updates
    //             if (remainingExp > 0)
    //             {
    //                 expBar.fillAmount = Mathf.Lerp(startFillExp, targetFillExp, t);

    //                 int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(remainingExp, remainingExp - expToAdd, t));
    //                 int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(initialExpNext, initialExpNext - expToAdd, t));

    //                 expGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : "0";
    //                 expNext.text = interpolatedNext.ToString();
    //             }

    //             // SKILL updates
    //             if (remainingSkillExp > 0)
    //             {
    //                 skillBar.fillAmount = Mathf.Lerp(startFillSkill, targetFillSkill, t);

    //                 int interpolatedSkillGain = Mathf.RoundToInt(Mathf.Lerp(remainingSkillExp, remainingSkillExp - skillToAdd, t));
    //                 int interpolatedSkillNext = Mathf.RoundToInt(Mathf.Lerp(initialSkillNext, initialSkillNext - skillToAdd, t));

    //                 skillGained.text = interpolatedSkillGain > 0 ? $"+{interpolatedSkillGain}" : "0";
    //                 skillNext.text = interpolatedSkillNext.ToString();
    //             }

    //             if (playerInput.actions["SkipCutscene"].WasPressedThisFrame()) break;
    //             yield return null;
    //         }

    //         // Finalize cycle
    //         if (remainingExp > 0)
    //         {
    //             expBar.fillAmount = targetFillExp;
    //             currentExp += expToAdd;
    //             remainingExp -= expToAdd;

    //             if (currentExp >= expThreshold && remainingExp > 0)
    //             {
    //                 currentExp = 0;
    //                 initialExpNext = expThreshold;
    //             }
    //         }

    //         if (remainingSkillExp > 0)
    //         {
    //             skillBar.fillAmount = targetFillSkill;
    //             currentSkillExp += skillToAdd;
    //             remainingSkillExp -= skillToAdd;

    //             if (currentSkillExp >= skillThreshold && remainingSkillExp > 0)
    //             {
    //                 currentSkillExp = 0;
    //                 initialSkillNext = skillThreshold;
    //             }
    //         }
    //     }

    //     // ===== Final UI state =====
    //     expGained.text = "0";
    //     expNext.text = (expThreshold - currentExp).ToString();

    //     skillGained.text = "0";
    //     skillNext.text = (skillThreshold - currentSkillExp).ToString();

    //     yield return new WaitForSeconds(1f);
    //     DeactivateExperienceMenu();
    // }
    // public IEnumerator GainExperienceMenu(UnitManager unit, int gainExp, string skillType1, int skillInc, int SPInc)
    // {
    //     experienceMenu.SetActive(true);
    //     expUnitName.text = unit.GetName();

    //     // gainExp = 200;

    //     // ===== EXP SETUP =====
    //     int currentExp = unit.GetExperience();
    //     int remainingExp = gainExp;
    //     int expThreshold = 100;
    //     int initialExpNext = expThreshold - currentExp;

    //     expGained.text = "+" + remainingExp.ToString();
    //     expNext.text = initialExpNext.ToString();

    //     // ===== SKILL SETUP =====
    //     int currentSkillExp = unit.GetStats().GetSkillExperience(skillType1);
    //     int remainingSkillExp = skillInc;

    //     int skillThreshold = UnitRosterManager.GetNextLevelExpRequirement(currentSkillExp);
    //     int initialSkillNext = UnitRosterManager.GetExpToNextLevel(currentSkillExp);

    //     skillName.text = skillType1;
    //     skillGained.text = "+" + remainingSkillExp.ToString();
    //     skillNext.text = initialSkillNext.ToString();

    //     // ===== SP SETUP (animated) =====
    //     // int spThreshold = 100; // optional cap, adjust if needed
    //     // int currentSP = unit.GetStats().SP; 
    //     // int targetSP = currentSP + SPInc;

    //     // SPGained.text = SPInc.ToString(); // start with full SP gained
    //     // SPNext.text = (spThreshold - currentSP).ToString(); // how much left before cap

    //     // // Animate SP gain
    //     // float spDuration = 1f; // how long SP should animate
    //     // float spElapsed = 0f;

    //     int currentSP = unit.GetStats().SP;
    //     int gainedSP = SPInc;

    //     // int remainingSPGain = SPInc;

    //     // initialize UI
    //     SPNext.text = currentSP.ToString();
    //     SPGained.text = "+ " + gainedSP.ToString();

    //     // ===== LOOP: animate EXP + SKILL together =====
    //     while (remainingExp > 0 || remainingSkillExp > 0)
    //     {
    //         int expToAdd = Mathf.Min(expThreshold - currentExp, remainingExp);
    //         // skillThreshold = UnitRosterManager.GetNextLevelExpRequirement(currentSkillExp);

    //         int skillToAdd = Mathf.Min(skillThreshold - currentSkillExp, remainingSkillExp);

    //         float startFillExp = (float)currentExp / expThreshold;
    //         float targetFillExp = (float)(currentExp + expToAdd) / expThreshold;

    //         float startFillSkill = (float)currentSkillExp / skillThreshold;
    //         float targetFillSkill = (float)(currentSkillExp + skillToAdd) / skillThreshold;

    //         float duration = Mathf.Max(
    //             (targetFillExp - startFillExp),
    //             (targetFillSkill - startFillSkill)
    //         ) * 1.5f; 
    //         float elapsed = 0f;

    //         // Animate both bars simultaneously
    //         while (elapsed < duration)
    //         {
    //             elapsed += Time.deltaTime;
    //             float t = elapsed / duration;

    //             // EXP updates
    //             if (remainingExp > 0)
    //             {
    //                 expBar.fillAmount = Mathf.Lerp(startFillExp, targetFillExp, t);

    //                 int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(remainingExp, remainingExp - expToAdd, t));
    //                 int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(initialExpNext, initialExpNext - expToAdd, t));

    //                 expGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : "0";
    //                 expNext.text = interpolatedNext.ToString();
    //             }

    //             // SKILL updates
    //             if (remainingSkillExp > 0)
    //             {
    //                 skillBar.fillAmount = Mathf.Lerp(startFillSkill, targetFillSkill, t);

    //                 int interpolatedSkillGain = Mathf.RoundToInt(Mathf.Lerp(remainingSkillExp, remainingSkillExp - skillToAdd, t));
    //                 int interpolatedSkillNext = Mathf.RoundToInt(Mathf.Lerp(initialSkillNext, initialSkillNext - skillToAdd, t));

    //                 skillGained.text = interpolatedSkillGain > 0 ? $"+{interpolatedSkillGain}" : "0";
    //                 skillNext.text = interpolatedSkillNext.ToString();
    //             }

    //             // SP updates (increasing text over time)
    //             if (SPInc > 0)
    //             {
    //                 elapsed += Time.deltaTime;
    //                 float tSP = Mathf.Clamp01(elapsed / duration);

    //                 int interpolatedSP = Mathf.RoundToInt(Mathf.Lerp(currentSP, currentSP + SPInc, tSP));
    //                 int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(SPInc, 0, tSP));

    //                 SPNext.text = interpolatedSP.ToString();          // current SP increasing
    //                 SPGained.text = "+ " + interpolatedGain.ToString(); // SP gain decreasing
    //             }

    //             if (playerInput.actions["SkipCutscene"].WasPressedThisFrame()) break;
    //             yield return null;
    //         }

    //         // Finalize cycle
    //         if (remainingExp > 0)
    //         {
    //             expBar.fillAmount = targetFillExp;
    //             currentExp += expToAdd;
    //             remainingExp -= expToAdd;

    //             if (currentExp >= expThreshold && remainingExp > 0)
    //             {
    //                 currentExp = 0;
    //                 initialExpNext = expThreshold;
    //             }
    //         }

    //         if (remainingSkillExp > 0)
    //         {
    //             skillBar.fillAmount = targetFillSkill;
    //             currentSkillExp += skillToAdd;
    //             remainingSkillExp -= skillToAdd;

    //             if (currentSkillExp >= skillThreshold && remainingSkillExp > 0)
    //             {
    //                 currentSkillExp = 0;
    //                 initialSkillNext = skillThreshold;
    //             }
    //         }
    //     }

    //     // ===== Final UI state =====
    //     expGained.text = "0";
    //     expNext.text = (expThreshold - currentExp).ToString();

    //     skillGained.text = "0";
    //     skillNext.text = (skillThreshold - currentSkillExp).ToString();


    //     // SPNext.text = $"+{SPInc}";
    //     // SPNext.text = finalSP.ToString();
    //     SPGained.text = "+ 0";                          // all SP gained has been "used"
    //     SPNext.text = (currentSP + SPInc).ToString();   

    //     yield return new WaitForSeconds(1f);
    //     DeactivateExperienceMenu();
    // }

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

        int initialExpNext = expThreshold - currentExp;
        expGained.text = "+" + remainingExp;
        expNext.text = initialExpNext.ToString();

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

    // private IEnumerator AnimateSkill(UnitManager unit, string skillType, int skillInc)
    // {
    //     // get exp that the unit is currently at for this skill
    //     int currentSkillExp = unit.GetStats().GetSkillExperience(skillType);

    //     // assigns how much exp they are gaining
    //     int remainingSkillExp = skillInc;

    //     // get initial thresholds
    //     int skillThreshold = UnitRosterManager.GetNextLevelExpRequirement(currentSkillExp);
    //     int initialSkillNext = UnitRosterManager.GetExpToNextLevel(currentSkillExp);

    //     // Sets the initial text
    //     skillName.text = skillType;
    //     skillGained.text = "+" + remainingSkillExp;
    //     skillNext.text = initialSkillNext.ToString();

    //     while (remainingSkillExp > 0)
    //     {
    //         int skillToAdd = Mathf.Min(skillThreshold - currentSkillExp, remainingSkillExp);

    //         float startFill = (float)currentSkillExp / skillThreshold;
    //         float targetFill = (float)(currentSkillExp + skillToAdd) / skillThreshold;
    //         float duration = (targetFill - startFill) * 1.5f;
    //         float elapsed = 0f;

    //         while (elapsed < duration)
    //         {
    //             elapsed += Time.deltaTime;
    //             float t = elapsed / duration;

    //             skillBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

    //             int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(remainingSkillExp, remainingSkillExp - skillToAdd, t));
    //             int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(initialSkillNext, initialSkillNext - skillToAdd, t));

    //             skillGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : "0";
    //             skillNext.text = interpolatedNext.ToString();

    //             yield return null;
    //         }

    //         // finalize this cycle
    //         skillBar.fillAmount = targetFill;
    //         currentSkillExp += skillToAdd;
    //         remainingSkillExp -= skillToAdd;

    //         // ✅ Recalculate threshold if leveled up
    //         if (currentSkillExp >= skillThreshold && remainingSkillExp > 0)
    //         {

    //             // recalc next level requirements
    //             skillThreshold = UnitRosterManager.GetNextLevelExpRequirement(currentSkillExp);
    //             initialSkillNext = UnitRosterManager.GetExpToNextLevel(currentSkillExp);

    //             // reset UI for the new level
    //             skillBar.fillAmount = 0f;
    //             skillNext.text = initialSkillNext.ToString();

    //             currentSkillExp = 0;

    //         }
    //     }

    //     skillGained.text = "0";
    //     skillNext.text = (skillThreshold - currentSkillExp).ToString();
    // }

    // private IEnumerator AnimateSkill(UnitManager unit, string skillType, int skillInc)
    // {
    //     // get exp that the unit is currently at for this skill
    //     int skillExp = unit.GetStats().GetSkillExperience(skillType);

    //     int currLevel = UnitRosterManager.GetCurrentLevel(skillExp);

    //     int skillThreshold = UnitRosterManager.GetExpBetweenLevels(currLevel);

    //     int prevTotalExpForLevel = UnitRosterManager.GetTotalExpForLevel(currLevel);
    //     int currentSkillExp = skillExp - prevTotalExpForLevel;

    //     int remainingSkillExp = skillInc;

    //     // int skillThreshold = UnitRosterManager.GetNextLevelExpRequirement(currentSkillExp);


    //     skillName.text = skillType;
    //     skillGained.text = "+" + remainingSkillExp;
    //     skillNext.text = UnitRosterManager.GetExpToNextLevel(skillExp).ToString();

    //     while (remainingSkillExp > 0)
    //     {
    //         int skillToAdd = Mathf.Min(skillThreshold - currentSkillExp, remainingSkillExp);

    //         float startFill = (float)currentSkillExp / skillThreshold;
    //         float targetFill = (float)(currentSkillExp + skillToAdd) / skillThreshold;
    //         float duration = (targetFill - startFill) * 1.5f;
    //         float elapsed = 0f;

    //         while (elapsed < duration)
    //         {
    //             elapsed += Time.deltaTime;
    //             float t = elapsed / duration;

    //             skillBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

    //             int interpolatedGain = Mathf.RoundToInt(Mathf.Lerp(remainingSkillExp, remainingSkillExp - skillToAdd, t));
    //             int interpolatedNext = Mathf.RoundToInt(Mathf.Lerp(initialSkillNext, initialSkillNext - skillToAdd, t));

    //             skillGained.text = interpolatedGain > 0 ? $"+{interpolatedGain}" : "0";
    //             skillNext.text = interpolatedNext.ToString();

    //             yield return null;
    //         }

    //         // finalize this cycle
    //         skillBar.fillAmount = targetFill;
    //         currentSkillExp += skillToAdd;
    //         remainingSkillExp -= skillToAdd;

    //         // ✅ Recalculate threshold if leveled up
    //         if (currentSkillExp >= skillThreshold && remainingSkillExp > 0)
    //         {

    //             // recalc next level requirements
    //             skillThreshold = UnitRosterManager.GetNextLevelExpRequirement(currentSkillExp);
    //             initialSkillNext = UnitRosterManager.GetExpToNextLevel(currentSkillExp);

    //             // reset UI for the new level
    //             skillBar.fillAmount = 0f;
    //             skillNext.text = initialSkillNext.ToString();

    //             currentSkillExp = 0;

    //         }
    //     }

    //     skillGained.text = "0";
    //     skillNext.text = (skillThreshold - currentSkillExp).ToString();
    // }

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

// public IEnumerator BattleMenu(UnitManager left, UnitManager right, bool playerOnLeft, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {

//     Image pBar;
//     Image eBar;
//     Image pLostBar;
//     Image eLostBar;

//     // Assign health bars based on whether the player is on the left
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
//         eBar = EHealthSwapped;
//         pBar = PHealthSwapped;
//         pLostBar = PHealthLost;
//         eLostBar = EHealthLost;
//         pBar.gameObject.SetActive(true);
//         eBar.gameObject.SetActive(true);
//         PHealth.gameObject.SetActive(false);
//         EHealth.gameObject.SetActive(false);
//     }

//     if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
//     if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

//     // Use left and right units directly instead of re-assigning them
//     UnitManager leftUnit = left;
//     UnitManager rightUnit = right;

//     // Calculate hit and crit rates for both units
//     int leftHit = -1;
//     int rightHit = -1;
//     int leftCrit = -1;
//     int rightCrit = -1;
//     if (leftUnit.primaryWeapon != null) {
//         leftHit = leftUnit.primaryWeapon.HitRate + (leftUnit.stats.Luck * 4) - rightUnit.stats.Evasion;
//         leftCrit = leftUnit.primaryWeapon.CritRate + (int)(leftUnit.stats.Luck / 2);
//     }
//     if (rightUnit.primaryWeapon != null) {
//         rightHit = rightUnit.primaryWeapon.HitRate + (rightUnit.stats.Luck * 4) - leftUnit.stats.Evasion;
//         rightCrit = rightUnit.primaryWeapon.CritRate + (int)(rightUnit.stats.Luck / 2);
//     }
    

//     // Clamp hit and crit rates between 0 and 100
//     leftHit = Mathf.Clamp(leftHit, 0, 100);
//     leftCrit = Mathf.Clamp(leftCrit, 0, 100);
//     rightHit = Mathf.Clamp(rightHit, 0, 100);
//     rightCrit = Mathf.Clamp(rightCrit, 0, 100);

//     // Set the left unit (player or enemy) info
//     PlayerName.text = leftUnit.stats.Name;

//     if(leftUnit.primaryWeapon != null) {
//         PlayerWeapon.text = leftUnit.primaryWeapon.WeaponName;

//     } else {
//         PlayerWeapon.text = "";
//     }
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

//     // Update health bars and positions
//     float leftCurrFill = (float)(expectedPlayerHP) / leftUnit.stats.Health;
//     float leftLostFill = (float)leftUnit.getCurrentHealth() / leftUnit.stats.Health;
//     pBar.fillAmount = leftCurrFill;
//     pLostBar.fillAmount = leftLostFill;

//     // Destroy any previous HP indicator for the player
//     Destroy(HPplayer);

//     // Set position for the player's health bar
//     float pXPos = (290.0f * (1.0f - leftLostFill)) - 217.0f ;
//     HPplayer = Instantiate(HPIndicator, new Vector3(pXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

//     GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
//     TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
//     // PlayerCurrHealth.text = $"{leftUnit.getCurrentHealth()}";
//     plaText.text = $"{expectedPlayerHP}";
//     // if(playerOnLeft) {
//     //     plaText.text = $"{expectedPlayerHP}";
//     //     // PlayerCurrHealth.text = $"{leftUnit.getCurrentHealth()}";
//     // } else {
//     //     plaText.text = $"{expectedEnemyHP}";
//     //     // PlayerCurrHealth.text = $"{rightUnit.getCurrentHealth()}";
//     // }
    

//     // Set the right unit (enemy or player) info
//     EnemyName.text = rightUnit.stats.Name;

//     if (rightUnit.primaryWeapon != null) {
//         EnemyWeapon.text = rightUnit.primaryWeapon.WeaponName;
//     } else {
//         EnemyWeapon.text = "";
//     }
    
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

//     // Update health bars and positions
//     float rightCurrFill = (float)(expectedEnemyHP) / rightUnit.stats.Health;
//     float rightLostFill = (float)rightUnit.getCurrentHealth() / rightUnit.stats.Health;
//     eBar.fillAmount = rightCurrFill;
//     eLostBar.fillAmount = rightLostFill;

//     // Destroy any previous HP indicator for the enemy
//     Destroy(HPenemy);

//     // Set position for the enemy's health bar
//     float eXPos = (291.0f * rightLostFill) + 140.0f ;
//     HPenemy = Instantiate(HPIndicator, new Vector3(eXPos + 960f, 540f, 0f), Quaternion.identity, Menu.transform);

//     GameObject eneChild = HPenemy.transform.GetChild(0).gameObject;
//     TextMeshProUGUI eneText = eneChild.GetComponent<TextMeshProUGUI>();

//     eneText.text = $"{expectedEnemyHP}";

//     // if(playerOnLeft) {
//     //     eneText.text = $"{expectedEnemyHP}";
//     //     // EnemyCurrHealth.text = $"{rightUnit.getCurrentHealth()}";
//     // } else {
//     //     eneText.text = $"{expectedPlayerHP}";
//     //     // EnemyCurrHealth.text = $"{leftUnit.getCurrentHealth()}";
//     // }


//     // Activate the menu
//     Menu.SetActive(true);

//     yield return null;
//     // yield return new WaitForSeconds(1f);
// }












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



    // private void EnsureButtonVisible(int currentIndex) {
    //     RectTransform buttonRect = itemButtons[currentIndex].GetComponent<RectTransform>();
    //     RectTransform contentRect = scrollViewContent.GetComponent<RectTransform>();
    //     RectTransform viewportRect = GameObject.Find("Canvas/ItemMenu/ScrollView").GetComponent<ScrollRect>().viewport;

    //     // Convert the button's position to the viewport's local space
    //     Vector3[] buttonCorners = new Vector3[4];
    //     buttonRect.GetWorldCorners(buttonCorners);

    //     Vector3[] viewportCorners = new Vector3[4];
    //     viewportRect.GetWorldCorners(viewportCorners);

    //     // Check if the button is out of view
    //     if (buttonCorners[0].y < viewportCorners[0].y + 45 || buttonCorners[1].y > viewportCorners[1].y - 45) {
    //         // Calculate the offset to move the button into view
    //         Vector2 viewportLocalPosition = viewportRect.InverseTransformPoint(buttonCorners[0]);
    //         Vector2 contentLocalPosition = contentRect.anchoredPosition;
    //         if (buttonCorners[0].y < viewportCorners[0].y + 45) {
    //             contentLocalPosition.y += 45;
    //         } else if (buttonCorners[1].y > viewportCorners[1].y - 45) {
    //             contentLocalPosition.y -= 45;
    //         }

    //         contentRect.anchoredPosition = contentLocalPosition;
    //     }
    // }


    //------------------------------------------Weapon List----------------------------------------









    //-------------------------------------Victory and Defeat Conditions---------------------------------------



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

        if (left.UnitType == "Player") {
            lBar = PHealth;
            PHealth.gameObject.SetActive(true);
            PHealthSwapped.gameObject.SetActive(false);
        } else {
            lBar = PHealthSwapped;
            PHealthSwapped.gameObject.SetActive(true);
            PHealth.gameObject.SetActive(false);
        }

        if (right.UnitType == "Player") {
            rBar = EHealthSwapped;
            EHealthSwapped.gameObject.SetActive(true);
            EHealth.gameObject.SetActive(false);
        } else {
            rBar = EHealth;
            EHealth.gameObject.SetActive(true);
            EHealthSwapped.gameObject.SetActive(false);
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
































