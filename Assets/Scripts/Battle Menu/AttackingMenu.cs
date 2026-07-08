using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class AttackingMenu : MonoBehaviour
{


    [SerializeField] TurnManager manageTurn;
    [SerializeField] CombatMenuManager combatMenuManager;
    [SerializeField] GenerateGrid generateGrid;
    [SerializeField] PlayerGridMovement moveGrid;
    [SerializeField] ExecuteAction executeAction;


    [SerializeField] MapManager _currentMap;
    [SerializeField] GameObject itemMenu;
    [SerializeField] GameObject buttonTemplate;

    List<Weapon> usableWeapons = new List<Weapon>();
    List<Weapon> nonUsableWeapons = new List<Weapon>();
    List<Weapon> listOfWeapons = new List<Weapon>();
    List<Button> itemButtons = new List<Button>();
    int currWeapIndex = 0;
    [SerializeField] FindPath findPath;
    [SerializeField] PlayerInput playerInput;
    float sensitivity = 0.2f;
    List<GameObject> divList = new List<GameObject>();
    [SerializeField] GameObject scrollViewContent;


    void Awake() {
        // itemMenu.SetActive(false);
        usableWeapons = new List<Weapon>();
        nonUsableWeapons = new List<Weapon>();
    }


    public IEnumerator WeaponList(UnitManager unit) {
        // Checks which weapons are avaliable
        // CheckWeapons(unit);
        listOfWeapons = unit.GetWeapons();
        Debug.Log("List of Weapons: " + listOfWeapons.Count);
        usableWeapons = unit.GetUsableWeapons();
        Debug.Log("Usable Weapons: " + usableWeapons.Count);
        

        foreach (Weapon wep in listOfWeapons) {
            if (!usableWeapons.Contains(wep)) {
                nonUsableWeapons.Add(wep);
            }
        }

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

            // float vertical = moveInput.y; 
            Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
            float vertical = moveInput.y;

            if (playerInput.actions["Back"].WasPressedThisFrame() && oneAction) {
                // Destroys path and sends it back to the action menu
                findPath.DestroyRange();
                StartCoroutine(combatMenuManager.ActivateActionMenu());
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
        Debug.Log("Usable Weapons: " + usableWeapons.Count);
        Debug.Log("Non-Usable Weapons: " + nonUsableWeapons.Count);
        listOfWeapons.AddRange(usableWeapons);
        listOfWeapons.AddRange(nonUsableWeapons);
        Debug.Log("List of Weapons: " + listOfWeapons.Count);

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

    public void DeactivateItemMenu() => itemMenu.SetActive(false);

    public void ActivateItemMenu() {
        itemMenu.SetActive(true);
        currWeapIndex = 0;
        foreach (Button btn in itemButtons) Destroy(btn.gameObject);
        foreach (GameObject obj in divList) Destroy(obj);
        itemButtons.Clear();
    }
}
