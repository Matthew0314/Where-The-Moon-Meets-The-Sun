using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.InputSystem;

public class ActionMenu : MonoBehaviour
{


    [SerializeField] TurnManager manageTurn;
    [SerializeField] CombatMenuManager combatMenuManager;
    [SerializeField] GenerateGrid generateGrid;
    [SerializeField] PlayerGridMovement moveGrid;
    [SerializeField] ExecuteAction executeAction;
    [SerializeField] MapManager _currentMap;

    private Vector2 moveInput;
    private float sensitivity = 0.2f;
    [SerializeField] PlayerInput playerInput;

    [Header("Action Menu")]
    [SerializeField] GameObject attackButton;
    [SerializeField]  GameObject itemButton;
    [SerializeField]  GameObject waitButton;
    [SerializeField] GameObject assistButton;
    List<GameObject> actionMenuList;

    void Awake() {
        actionMenuList = new List<GameObject>();
    }

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    
    public void DeactivateActionMenu() {
        foreach(GameObject obj in actionMenuList) Destroy(obj);
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
            // float vertical = moveInput.y;
            Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
            float vertical = moveInput.y;

            if (!axisInUse)
            {
                Debug.Log($"Vertical Input: {vertical}, Sensitivity: {sensitivity}");
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
        // Gets all the weapons and fath that can be used. This is to check later if the player can do these actions or not
        UnitManager unit = generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile;
        List<Weapon> usableWeapons = unit.GetUsableWeapons();
        List<Faith> usableFaith = unit.GetUsableFaith();

        // Stores the menu items and their corresponding actions (functions)
        var items = new List<(string, Action)>();
        
        // Stores the CP cost for each action
        List<int> CPCost = new List<int>();
        int tCos = 0;

        // Adds 
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

        items.Add(("Item", () => { DeactivateActionMenu(); combatMenuManager.PlayerItem(); }));
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
            ("Back",    () => { DeactivateActionMenu(); moveGrid.SetInMenu(false); }),
            ("End Turn", () => { DeactivateActionMenu(); manageTurn.EndTurn(); moveGrid.SetInMenu(false); })
        };

        yield return ShowActionMenu(items, null);
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

    public void PlayerAssist() => StartCoroutine(combatMenuManager.AssistMenu(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile));
    public void PlayerAttack() => StartCoroutine(combatMenuManager.AttackingList(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile));
}
