using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;



public class BattleStartMenu : MonoBehaviour
{
    private Button[,] buttons;
    private string[,] actions;
    private int rows = 2;
    private int columns = 2;
    [SerializeField] Button unitButton;
    [SerializeField] Button mapButton;
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject battleStartMenu;
    [SerializeField] GameObject UnitSelectButton;
    [SerializeField] GameObject UnitSelectBox;
    [SerializeField] RectTransform content;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] TextMeshProUGUI unitNumber;
    private List<UnitStats> playableRoster = new List<UnitStats>();

    private int currentRow = 0;
    private int currentCol = 0;
    public float sensitivity = 0.5f;
    public Vector2 moveInput;
    public PlayerInput playerInput;
    private bool inStartMenu = false;
    private bool inMapMenu = false;
    private PlayerGridMovement playerGridMovement;
    private MapManager _currentMap;

    private List<Button> unitButtons = new List<Button>();
    private List<UnitManager> unitList = new List<UnitManager>();
    private List<string> units = new List<string>();
    List<UnitStats> combinedList = new List<UnitStats>();
    private int selectedIndex = 0;
    private int buttonsPerRow = 2;

    private float inputCooldown = 0.2f;
    private float lastInputTime;



    [SerializeField] private GameObject InfoTextData;
    [SerializeField] private Image expBar;
    [SerializeField] private GameObject unitItemBar;
    private List<GameObject> unitItemBarsList = new List<GameObject>();

    void Awake()
    {
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        _currentMap = GameObject.Find("GridManager").GetComponent<MapManager>();

        if (unitButton == null || mapButton == null || startButton == null || exitButton == null)
        {
            Debug.LogError("One or more buttons are not assigned in the inspector.");
            return;
        }

        actions = new string[2, 2]
        {
            { "Units", "Map" },
            { "Start" , "Exit" }
        };

        buttons = new Button[2, 2]
        {
            { unitButton, mapButton },
            { startButton, exitButton }
        };
    }

    void Update()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
    }

    public IEnumerator StartMenu()
    {
        // _currentMap.InitStartTiles();
        battleStartMenu.SetActive(true);
        inStartMenu = true;

        bool axisInUse = false;
        bool oneAction = false;

        int currentRow = 0;
        int currentCol = 0;

        int rowCount = buttons.GetLength(0); // 2
        int colCount = buttons.GetLength(1); // 2

        // Select initial button
        buttons[currentRow, currentCol].Select();

        while (true)
        {
            float vertical = moveInput.y;
            float horizontal = moveInput.x;

            if (!axisInUse)
            {
                // Move Up
                if (vertical > sensitivity)
                {
                    currentRow = (currentRow - 1 + rowCount) % rowCount;
                    axisInUse = true;
                }
                // Move Down
                else if (vertical < -sensitivity)
                {
                    currentRow = (currentRow + 1) % rowCount;
                    axisInUse = true;
                }
                // Move Left
                else if (horizontal < -sensitivity)
                {
                    currentCol = (currentCol - 1 + colCount) % colCount;
                    axisInUse = true;
                }
                // Move Right
                else if (horizontal > sensitivity)
                {
                    currentCol = (currentCol + 1) % colCount;
                    axisInUse = true;
                }

                // Debug.Log(currentRow + " " + currentCol);

                // Select new button
                buttons[currentRow, currentCol].Select();
            }

            // Reset axis flag when joystick is released
            if (Mathf.Abs(vertical) < sensitivity && Mathf.Abs(horizontal) < sensitivity)
                axisInUse = false;

            // Handle Selection
            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame())
            {
                string selectedAction = actions[currentRow, currentCol];

                // if (selectedAction == "Start") { StartGame(); break; }
                // else if (selectedAction == "Exit") { ExitGame(); break; }
                // else if (selectedAction == "Units") { OpenUnits(); break; }
                // else if (selectedAction == "Map") { OpenMap(); break; }
                // else continue;

                if (selectedAction == "Start")
                {
                    break;
                }
                else if (selectedAction == "Map")
                {
                    yield return StartCoroutine(InMapMenu());
                }
                else if (selectedAction == "Units")
                {
                    yield return StartCoroutine(UnitSelect());
                }
            }

            // Handle Back
            // if (oneAction && playerInput.actions["Back"].WasPressedThisFrame())
            // {
            //     CloseMenu();
            //     break;
            // }

            oneAction = true;

            yield return null;
        }

        // _currentMap.DestroyStartTiles();
        battleStartMenu.SetActive(false);
        inStartMenu = false;

        yield return null;
    }

    private IEnumerator InMapMenu()
    {
        _currentMap.InitStartTiles();
        battleStartMenu.SetActive(false);
        inMapMenu = true;
        while (true)
        {
            if (playerInput.actions["Back"].WasPressedThisFrame() && playerGridMovement.BackToStartMenu())
            {
                battleStartMenu.SetActive(true);
                inMapMenu = false;
                _currentMap.DestroyStartTiles();

                yield break;
            }

            yield return null;
        }

        yield return null;
    }


    public bool GetInStartMenu() => inStartMenu;
    public bool GetInMapMenu() => inMapMenu;

    private IEnumerator UnitSelect()
    {
        BuildUnitSelectMenu();

        yield return null;
        if (unitButtons.Count == 0)
            yield break;

        selectedIndex = 0;
        unitButtons[selectedIndex].Select();
        UpdateUnitInfo(combinedList[selectedIndex]);

        float inputCooldown = 0.2f;
        float lastInputTime = -inputCooldown;

        while (true)
        {
            // Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

            if (Time.unscaledTime - lastInputTime > inputCooldown)
            {
                if (moveInput.x > 0.5f)
                {
                    CycleSelection(1); // right
                    lastInputTime = Time.unscaledTime;
                }
                else if (moveInput.x < -0.5f)
                {
                    CycleSelection(-1); // left
                    lastInputTime = Time.unscaledTime;
                }
                else if (moveInput.y > 0.5f)
                {
                    CycleSelection(-buttonsPerRow); // up
                    lastInputTime = Time.unscaledTime;
                }
                else if (moveInput.y < -0.5f)
                {
                    CycleSelection(buttonsPerRow); // down
                    lastInputTime = Time.unscaledTime;
                }

            }

            if (playerInput.actions["Select"].WasPressedThisFrame())
            {
                // unitButtons[selectedIndex].onClick.Invoke();
                // yield break; // Exit coroutine after selection

                // List<UnitManager> units = _currentMap.GetMapUnits();

                // string selectedUnitName = unitList[selectedIndex].stats.UnitName;

                // UnitManager matchingUnit = units.FirstOrDefault(u => u.stats.UnitName == selectedUnitName);

                // List<string> cantUse = _currentMap.GetRequiredUnits().Concat(_currentMap.GetForbiddenUnits()).ToList();

                // if (cantUse.Contains(matchingUnit.stats.UnitName)) {
                //     yield return null;
                //     continue;
                // }

                // if (matchingUnit != null)
                // {
                //     _currentMap.DespawnUnit(matchingUnit);
                //     BuildUnitSelectMenu();
                //     yield return null;
                // }

                // UnitManager matchingUnit = units.FirstOrDefault(u => u.stats.UnitName == units[selectedIndex]);

                // List<string> cantUse = _currentMap.GetRequiredUnits().Concat(_currentMap.GetForbiddenUnits()).ToList();

                // if (matchingUnit != null)
                // {
                //     _currentMap.DespawnUnit(matchingUnit);
                //     BuildUnitSelectMenu();
                //     yield return null;
                // }

                UnitStats unit = UnitRosterManager.GetPlayableUnit(units[selectedIndex]);

                List<string> cantUse = _currentMap.GetRequiredUnits().Concat(_currentMap.GetForbiddenUnits()).ToList();

                if (cantUse.Contains(unit.UnitName))
                {
                    yield return null;
                    continue;
                }

                List<UnitManager> unitsMan = _currentMap.GetMapUnits();
                UnitManager matchUnit = unitsMan.FirstOrDefault(u => u.GetUnitName() == unit.UnitName);

                if (matchUnit != null)
                {
                    _currentMap.DespawnUnit(matchUnit);
                    // BuildUnitSelectMenu();
                    ChangeTextColor(unitButtons[selectedIndex], "default", matchUnit.GetName());
                    yield return null;
                }

                else if ((_currentMap.GetMapUnits().Count < _currentMap.GetPlayerStartPositions().Length))
                {

                    _currentMap.SpawnUnit(unit, _currentMap.FindNextStartPosition());
                    // BuildUnitSelectMenu();
                    ChangeTextColor(unitButtons[selectedIndex], "blue", combinedList[selectedIndex].Name);

                    yield return null;
                }
            }

            if (playerInput.actions["Back"].WasPressedThisFrame())
            {
                // CloseUnitSelect();
                foreach (Button but in unitButtons)
                {
                    Destroy(but.gameObject);
                }
                unitButtons.Clear();
                unitList.Clear();
                units.Clear();
                combinedList.Clear();
                UnitSelectBox.SetActive(false);
                yield break; // Exit coroutine on cancel
            }

            yield return null;
        }
    }

    // private void CycleSelection(int delta)
    // {
    //     int newIndex = selectedIndex + delta;

    //     if (newIndex >= 0 && newIndex < unitButtons.Count)
    //     {
    //         selectedIndex = newIndex;
    //         unitButtons[selectedIndex].Select();
    //     }


    // }

    private void ScrollToButton(RectTransform targetButton)
    {
        Canvas.ForceUpdateCanvases();

        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        // Convert button's position to local space
        float buttonCenterY = -targetButton.localPosition.y + (targetButton.rect.height / 2);

        float upperBound = scrollRect.content.anchoredPosition.y;
        float lowerBound = upperBound + viewportHeight;

        float scrollY = Mathf.Clamp(buttonCenterY - viewportHeight / 1.25f, 0, contentHeight - viewportHeight);
        float normalized = 1 - Mathf.Clamp01(scrollY / (contentHeight - viewportHeight));

        scrollRect.verticalNormalizedPosition = normalized;
    }

    private void CycleSelection(int delta)
    {
        int newIndex = selectedIndex + delta;

        if (newIndex >= 0 && newIndex < unitButtons.Count)
        {
            selectedIndex = newIndex;
            unitButtons[selectedIndex].Select();

            // Scroll to the button
            RectTransform btnRect = unitButtons[selectedIndex].GetComponent<RectTransform>();
            ScrollToButton(btnRect);
        }
        UpdateUnitInfo(combinedList[selectedIndex]);
    }



    private void ChangeTextColor(Button button, string color, string name)
    {
        TMPro.TextMeshProUGUI[] texts = button.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

        foreach (var text in texts)
        {
            if (text.name.ToLower().Contains("name"))
            {
                Debug.LogWarning("Changing " + text.text + " to " + name);

                if (color == "green")
                {
                    color = "<color=#228B22>";  // Green
                }
                else if (color == "blue")
                {
                    color = "<color=#0000FF>"; // Blue
                }

                else if (color == "red")
                {
                    color = "<color=#FF0000>"; // Red
                }
                else
                {
                    color = "";
                }

                text.text = color + name + (color != "" ? "</color>" : "");

            }
        }

        unitNumber.text = _currentMap.GetMapUnits().Count + "/" + _currentMap.GetPlayerStartPositions().Length;

    }




    private void BuildUnitSelectMenu()
    {
        UnitSelectBox.SetActive(true);


        int buttonsPerRow = 2;
        float xSpacing = 450f;
        float ySpacing = 100f;

        Vector2 startPos = new Vector2(-225f, 400f);

        List<UnitStats> playableRoster = UnitRosterManager.GetPlayableUnits(); // ✅ Still UnitStats

        List<string> requiredUnits = _currentMap.GetRequiredUnits();
        List<UnitManager> mapUnits = _currentMap.GetMapUnits(); // ✅ UnitManagers
        List<string> forbiddenUnits = _currentMap.GetForbiddenUnits();

        // Split into groups using UnitName comparisons
        var requiredList = playableRoster
            .Where(unit => requiredUnits.Contains(unit.UnitName))
            .ToList();

        var mapUnitsOnlyList = playableRoster
            .Where(unit => !requiredUnits.Contains(unit.UnitName)
                        && !forbiddenUnits.Contains(unit.UnitName)
                        && mapUnits.Any(mu => mu.GetUnitName() == unit.UnitName))
            .ToList();

        var restList = playableRoster
            .Where(unit => !requiredUnits.Contains(unit.UnitName)
                        && !forbiddenUnits.Contains(unit.UnitName)
                        && !mapUnits.Any(mu => mu.GetUnitName() == unit.UnitName))
            .ToList();

        var forbiddenList = playableRoster
            .Where(unit => forbiddenUnits.Contains(unit.UnitName))
            .ToList();

        // Combine lists in order
        combinedList = requiredList
            .Concat(mapUnitsOnlyList)
            .Concat(restList)
            .Concat(forbiddenList)
            .ToList();

        foreach (Button but in unitButtons)
        {
            Destroy(but.gameObject);
        }
        unitButtons.Clear();
        unitList.Clear();

        for (int i = 0; i < combinedList.Count; i++)
        {
            int row = i / buttonsPerRow;
            int col = i % buttonsPerRow;

            float x = startPos.x + (col * xSpacing);
            float y = startPos.y - (row * ySpacing);

            GameObject button = Instantiate(UnitSelectButton, content);
            // RectTransform rect = button.GetComponent<RectTransform>();
            // rect.anchoredPosition = new Vector2(x, y);

            UnitStats stats = combinedList[i];
            TMPro.TextMeshProUGUI[] texts = button.GetComponentsInChildren<TMPro.TextMeshProUGUI>();


            foreach (var text in texts)
            {
                if (text.name.ToLower().Contains("name"))
                {
                    string color = "";

                    if (requiredUnits.Contains(stats.UnitName))
                    {
                        color = "<color=#228B22>"; // Green
                        UnitManager matchingUnit = mapUnits.Find(u => u.GetUnitName() == stats.UnitName);
                        unitList.Add(matchingUnit);
                    }
                    else if (mapUnits.Any(mu => mu.GetUnitName() == stats.UnitName)
                        && !requiredUnits.Contains(stats.UnitName)
                        && !forbiddenUnits.Contains(stats.UnitName))
                    {
                        color = "<color=#0000FF>"; // Blue

                        UnitManager matchingUnit = mapUnits.Find(u => u.GetUnitName() == stats.UnitName);
                        unitList.Add(matchingUnit);
                    }

                    else if (forbiddenUnits.Contains(stats.UnitName))
                        color = "<color=#FF0000>"; // Red
                    else
                        color = ""; // Default

                    text.text = color + stats.Name + (color != "" ? "</color>" : "");
                }
                else if (text.name.ToLower().Contains("health"))
                {
                    text.text = $"{stats.CurrentHealth}/{stats.Health}";
                }
            }

            Button btn = button.GetComponent<Button>();
            unitButtons.Add(btn);
            // unitList.Add()
            units.Add(stats.UnitName);

        }

        // Resize content height based on rows
        int totalRows = Mathf.CeilToInt(combinedList.Count / (float)buttonsPerRow);
        float contentHeight = totalRows * ySpacing + 50f;
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

        unitNumber.text = _currentMap.GetMapUnits().Count + "/" + _currentMap.GetPlayerStartPositions().Length;
    }



    public void UpdateUnitInfo(UnitStats stats)
    {


        TMPro.TextMeshProUGUI[] texts = InfoTextData.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

        foreach (var text in texts)
        {
            if (text.name.ToLower().Contains("level"))
            {
                text.text = stats.Level.ToString();
            }
            else if (text.name.ToLower().Contains("name"))
            {
                text.text = stats.Name;
            }
            else if (text.name.ToLower().Contains("exp"))
            {
                text.text = stats.Experience.ToString();
            }
            else if (text.name.ToLower().Contains("health"))
            {
                text.text = stats.CurrentHealth + "/" + stats.Health;
            }
            else if (text.name.ToLower().Contains("class"))
            {
                text.text = stats.UnitClass;
            }
        }

        foreach (GameObject g in unitItemBarsList) Destroy(g);
        unitItemBarsList.Clear();

        List<Weapon> weapons = stats.weapons;
        List<Item> items = stats.items;
        int count = 0;
        int offset = -52;

        foreach (Weapon w in weapons)
        {
            GameObject temp = Instantiate(unitItemBar);
            temp.transform.SetParent(InfoTextData.transform, false);
            temp.transform.position += new Vector3(0, count * offset, 0);

            TMPro.TextMeshProUGUI[] weapTexts = temp.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

            foreach (var text in weapTexts)
            {
                if (text.name.ToLower().Contains("item"))
                {
                    text.text = w.WeaponName;
                    if (stats.GetPrimaryWeapon() == w) text.text += " (e)";

                }
                else if (text.name.ToLower().Contains("uses"))
                {
                    text.text = w.Uses + "/" + w.MaxUses;
                }
            }

            count++;
            unitItemBarsList.Add(temp);
        }

        foreach (Item w in items)
        {
            GameObject temp = Instantiate(unitItemBar);
            temp.transform.SetParent(InfoTextData.transform, false);
            temp.transform.position += new Vector3(0, count * offset, 0);

            TMPro.TextMeshProUGUI[] itemTexts = temp.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

            foreach (var text in itemTexts)
            {
                if (text.name.ToLower().Contains("item"))
                {
                    text.text = w.Name;
                }
                else if (text.name.ToLower().Contains("uses"))
                {
                    text.text = w.Uses + "/" + w.MaxUses;
                }
            }

            count++;
            unitItemBarsList.Add(temp);
        }
    }







}
