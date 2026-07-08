using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;


public enum ItemTypeMenu {
    Weapon,
    Faith,
    Inventory
}

public class ItemMenu : MonoBehaviour
{


    [SerializeField] TurnManager manageTurn;
    [SerializeField] CombatMenuManager combatMenuManager;
    [SerializeField] GenerateGrid generateGrid;
    [SerializeField] PlayerGridMovement moveGrid;
    [SerializeField] ExecuteAction executeAction;


    [SerializeField] MapManager _currentMap;
    [SerializeField] GameObject itemMenu;
    [SerializeField] GameObject buttonTemplate;
    [SerializeField] GameObject itemDivider;
    [SerializeField] GameObject buttonItemOption;

    List<Weapon> usableWeapons = new List<Weapon>();
    List<Weapon> nonUsableWeapons = new List<Weapon>();
    List<Weapon> listOfWeapons = new List<Weapon>();

    List<Faith> usableFaith = new List<Faith>();
    List<Faith> nonUsableFaith = new List<Faith>();
    List<Faith> listOfFaith = new List<Faith>();




    List<Button> itemButtons = new List<Button>();
    [SerializeField] FindPath findPath;
    [SerializeField] PlayerInput playerInput;
    float sensitivity = 0.2f;
    List<GameObject> divList = new List<GameObject>();
    [SerializeField] GameObject scrollViewContent;


    void Awake() {
        // itemMenu.SetActive(false);
        usableWeapons = new List<Weapon>();
        nonUsableWeapons = new List<Weapon>();

        usableFaith = new List<Faith>();
        nonUsableFaith = new List<Faith>();

        DeactivateItemMenu();
    }


    public IEnumerator ItemMenuList(UnitManager unit, ItemTypeMenu itemType) {
        // Checks which weapons are avaliable
        // CheckWeapons(unit);
        
        int maxIndex = 0;
        int currIndex = 0;
        bool[,] attackGrid = new bool[generateGrid.GetWidth(), generateGrid.GetLength()];

        if(itemType == ItemTypeMenu.Weapon) {
            // Creates the weapon menu
            CreateWeaponMenu(unit);
            maxIndex = usableWeapons.Count + nonUsableWeapons.Count - 1;

            // Update attack range
            findPath.DestroyRange();
            attackGrid = findPath.CalculateAttack(
                moveGrid.getX(),
                moveGrid.getZ(),
                listOfWeapons[currIndex].Range,
                listOfWeapons[currIndex].Range1,
                listOfWeapons[currIndex].Range2,
                listOfWeapons[currIndex].Range3
            );
            findPath.HighlightAttack(attackGrid);
        } else if(itemType == ItemTypeMenu.Faith) {
            // Creates the faith menu
            CreateAssistMenu(unit);
            maxIndex = usableFaith.Count + nonUsableFaith.Count - 1;
        } else if(itemType == ItemTypeMenu.Inventory) {
            // Creates the inventory menu
            CreateInventoryMenu(unit);
            maxIndex = unit.GetWeapons().Count + unit.GetItems().Count + unit.GetFaith().Count - 1 ;
        }
        
        bool axisInUse = false;
        bool oneAction = false;
        moveGrid.isAttacking = false;

        if (maxIndex >= 0) itemButtons[currIndex].Select();

        findPath.DestroyRange();

        

        while (true) {
            
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


            if (!axisInUse && Mathf.Abs(vertical) > sensitivity)
            {
                // Deselect current button
                itemButtons[currIndex].OnDeselect(null);

                // Move index
                currIndex += vertical > 0 ? -1 : 1;

                // Wrap around
                if (currIndex < 0)
                    currIndex = maxIndex;
                else if (currIndex > maxIndex)
                    currIndex = 0;

                // Select new button
                itemButtons[currIndex].Select();

                if (itemType == ItemTypeMenu.Weapon)
                {
                    // Update attack range
                    findPath.DestroyRange();
                    attackGrid = findPath.CalculateAttack(
                        moveGrid.getX(),
                        moveGrid.getZ(),
                        listOfWeapons[currIndex].Range,
                        listOfWeapons[currIndex].Range1,
                        listOfWeapons[currIndex].Range2,
                        listOfWeapons[currIndex].Range3
                    );
                    findPath.HighlightAttack(attackGrid);
                }
                
                axisInUse = true;
            }

            if (Mathf.Abs(vertical) < sensitivity) axisInUse = false;

            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame()) {
                
                oneAction = false;

                if (itemType == ItemTypeMenu.Weapon)
                {
                    // Gets all enemies in range and begins the cycle attack method
                    List<GridTile> UnitsInRange = new List<GridTile>();
                    for (int i = 0; i < generateGrid.GetWidth(); i++)
                    {
                        for (int j = 0; j < generateGrid.GetLength(); j++)
                        {
                            if (attackGrid[i, j] && generateGrid.GetGridTile(i, j).UnitOnTile != null && generateGrid.GetGridTile(i, j).UnitOnTile.UnitType.Equals("Enemy"))
                            {
                                UnitsInRange.Add(generateGrid.GetGridTile(i, j));
                            }
                        }
                    }

                    if (UnitsInRange.Count <= 0) continue;

                    DeactivateItemMenu();
                    StartCoroutine(executeAction.CycleAttackList(UnitsInRange, listOfWeapons[currIndex]));

                    break;
                }
                else if (itemType == ItemTypeMenu.Faith && currIndex < usableFaith.Count)
                {
                    // Handle faith selection logic here
                    List<GridTile> temp = new List<GridTile>();
                    temp = usableFaith[currIndex].GetUnitsInRange(generateGrid.GetGridTile(moveGrid.getX(), moveGrid.getZ()).UnitOnTile);

                    if(temp.Count <= 0) continue;

                    
                    DeactivateItemMenu();
                    StartCoroutine(executeAction.CycleAssist(temp, usableFaith[currIndex]));
                    break;
                } else if (itemType == ItemTypeMenu.Inventory)
                {
                    StartCoroutine(ItemMenuOptionSelect(unit, itemButtons[currIndex], currIndex));
                    break;
                }
                
            }
            
            oneAction = true;

            yield return null;
        }

        yield return null;
    }

    // Creates the Weapon Menu
    // TODO: Add dividers for Magic when you eventually get that running
    public void CreateWeaponMenu(UnitManager unit) {

        // Activates the Item menu background
        ActivateItemMenu();

        listOfWeapons.Clear();
        usableWeapons.Clear();
        nonUsableWeapons.Clear();

        listOfWeapons = unit.GetWeapons();
        usableWeapons = unit.GetUsableWeapons();
        

        foreach (Weapon wep in listOfWeapons) {
            if (!usableWeapons.Contains(wep)) {
                nonUsableWeapons.Add(wep);
            }
        }

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
                if(listOfWeapons[i] == unit.GetPrimaryWeapon()) {
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

    void CreateAssistMenu(UnitManager unit) {

        ActivateItemMenu();

        listOfFaith.Clear();
        usableFaith.Clear();
        nonUsableFaith.Clear();

        listOfFaith = unit.GetFaith();
        usableFaith = unit.GetUsableFaith();

        foreach (Faith faith in listOfFaith) {
            if (!usableFaith.Contains(faith)) {
                nonUsableFaith.Add(faith);
            }
        }

        listOfFaith = new List<Faith>();
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


    public void CreateInventoryMenu(UnitManager user) {

        ActivateItemMenu();
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


    private IEnumerator ItemMenuOptionSelect(UnitManager user, Button button, int currIndex) {
        int ind = currIndex;

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
            StartCoroutine(ItemMenuList(user, ItemTypeMenu.Inventory));
        } else {
            optionButtons[index].Select();
        }

        

        bool axisInUse = false;
        bool oneAction = false;
        

        while (optionButtons.Count > 0) {
            Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
            float vertical = moveInput.y;

            // Debug.Log(buttons[currentIndex]);

            if (playerInput.actions["Back"].WasPressedThisFrame() && oneAction) {

                StartCoroutine(ItemMenuList(user, ItemTypeMenu.Inventory));

                foreach (Button btn in optionButtons) {
                    Destroy(btn.gameObject);
                }
                break;
            }

            if (!axisInUse && optionButtons.Count > 1 && Mathf.Abs(vertical) > 0.2f)
            {
                // Deselect current button
                optionButtons[index].OnDeselect(null);

                // Move index
                index += vertical > 0 ? 1 : -1;

                // Wrap around
                if (index < 0)
                    index = optionButtons.Count - 1;
                else if (index >= optionButtons.Count)
                    index = 0;

                // Select new button
                optionButtons[index].Select();

                axisInUse = true;
            }

            if (Mathf.Abs(vertical) < 0.2f)
            {
                axisInUse = false;
            }

            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame()) // "Submit" button
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
                        currIndex = 0;
                            
                        StartCoroutine(ItemMenuList(user, ItemTypeMenu.Inventory));
                    }
                    else if (options[index] == "Unequip")
                    {
                        user.SetPrimaryWeapon(null);
                        foreach (Button btn in optionButtons)
                        {
                            Destroy(btn.gameObject);
                        }
                        // currItemIndex = 0;

                        StartCoroutine(ItemMenuList(user, ItemTypeMenu.Inventory));
                    }
                    else if (options[index] == "Discard")
                    {
                        foreach (Button btn in optionButtons)
                        {
                            Destroy(btn.gameObject);
                        }
                        currIndex = 0;

                        StartCoroutine(ItemMenuList(user, ItemTypeMenu.Inventory));
                        
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
                        currIndex = 0;

                        
                        combatMenuManager.PlayerWait(); //Might have to change if wait abilities are implemented
                    }
                }
                break;
            }
            
            oneAction = true;

            yield return null;
        }


        yield return null;


    }

    public void DeactivateItemMenu() {
        foreach (Button btn in itemButtons) Destroy(btn.gameObject);
        foreach (GameObject obj in divList) Destroy(obj);
        itemButtons.Clear();
        itemMenu.SetActive(false);
    }

    public void ActivateItemMenu() {
        itemMenu.SetActive(true);
        foreach (Button btn in itemButtons) Destroy(btn.gameObject);
        foreach (GameObject obj in divList) Destroy(obj);
        itemButtons.Clear();
    }
}
