using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

public abstract class MapManager : MonoBehaviour
{
    //Initializes class refrences
    protected UnitRosterManager unitRos;
    protected WeaponManager manageWeapons;
    protected PlayerClassManager classRos;
    protected PlayerGridMovement playerCursor;
    protected FindPath pathFinder;
    protected GenerateGrid grid;
    protected TurnManager manageTurn;
    protected CombatMenuManager combatMenuManager;
    protected ExecuteAction executeAction;
    [SerializeField] protected EnemyInitializer enemyInitializer;


    // Initialized Difficult variable and text data
    protected string Difficulty = "Normal";
    [SerializeField] protected TextAsset enemyTextDataNormal;
    [SerializeField] protected TextAsset enemyTextDataHard;
    [SerializeField] protected TextAsset enemyTextDataEclipse;
    protected int maxEID;


    // Info about map size, conditions, and how many units
    //* MUST BE INITIALIZED IN AWAKE IN EVERY CHILD CLASS
    protected int length = 1;
    protected int width = 1;
    protected string winCondition = "";
    protected string loseCondition = "";
    protected int unitStartNum;
    protected int maxEIDNormal = 0;
    protected int maxEIDHard = 0;
    protected int maxEIDEclipse = 0;


    // Lists of Players, Enemies and Allies
    // * Map Allies and Map Enemies 2 will not always be used
    protected List<UnitStats> mapUnits;
    protected List<UnitManager> mapGameUnits = new List<UnitManager>();
    protected Queue<UnitManager> mapEnemies = new Queue<UnitManager>();
    protected Queue<UnitManager> mapEnemies2 = new Queue<UnitManager>();
    protected Queue<UnitManager> mapAllies = new Queue<UnitManager>();

    // Getters for each Queues and Lists
    public virtual Queue<UnitManager> GetMapEnemies1() => mapEnemies;
    public virtual Queue<UnitManager> GetMapEnemies2() => mapEnemies2;
    public virtual Queue<UnitManager> GetMapAllies() => mapAllies;
    public virtual List<UnitStats> GetMapUnitStats() => mapUnits;
    public virtual List<UnitManager> GetMapUnits() => mapGameUnits;

    // Getters for Length and Width
    public virtual int GetLength() => length;
    public virtual int GetWidth() => width;

    // Getter for Difficult
    // TODO: Get rid of later
    public virtual string GetDifficulty() => Difficulty;

    // Abstract classes every class must implement
    // TODO: Move InitEnemies to parent class
    public abstract IEnumerator CheckClearCondition();
    public abstract IEnumerator CheckDefeatCondition();
    public abstract IEnumerator CheckEvents();
    protected abstract Vector2Int[] GetPlayerStartPositions();
    protected abstract IEnumerator StartMap();





    protected virtual void Awake()
    {
        //Stores componenets that will be used later 
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        unitRos = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        classRos = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        manageWeapons = GameObject.Find("GridManager").GetComponent<WeaponManager>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        playerCursor = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        executeAction = GameObject.Find("Player").GetComponent<ExecuteAction>();
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
    }

    protected virtual void Start() { }

    protected virtual void Init()
    {
        //Gets the Difficulty
        //! Change when difficulty is not tied to title screen script
        string temp = TitleScreen.GetDifficulty();
        if (temp == " ") Difficulty = "Normal";
        else Difficulty = temp;

        //Calls GenerateGrid.cs to generate the grid based on how big it is, specified by length and width variables
        grid.GenGrid(length, width);

        //Initilizes what units will be on the map initially
        unitRos.InitMapUnit(unitStartNum);

        //Prints the map units on the map
        PrintCharacters();

        //Prints the enemies on the map
        InitEnemies();

        unitRos.setFaithSpells();

        manageTurn.SetLists();
        manageTurn.SetEnemyList();

        //! Change to Start Menu instead of starting Map
        StartCoroutine(StartMap());
    }

    // Adds New Players
    protected virtual void AddNewPlayers(string[] newUnits)
    {
        for (int i = 0; i < newUnits.Length; i++)
        {
            UnitRosterManager.AddPlayableUnit(newUnits[i]);
        }
    }

    // Standard logic for removing a Dead Unit
    // TODO: Review Logic for any buigs, such as enemy range not resetting
    public virtual void RemoveDeadUnit(UnitManager unit, int x, int z)
    {

        if (unit.stats.UnitType == "Enemy")
        {
            Queue<UnitManager> temp = mapEnemies;

            int queueCou = temp.Count;

            for (int i = 0; i < queueCou; i++)
            {
                UnitManager eneTemp = temp.Dequeue();
                if (eneTemp.stats.EnemyID == unit.stats.EnemyID)
                {
                    grid.GetGridTile(x, z).UnitOnTile = null;
                    GameObject tempObj = eneTemp.gameObject;
                    if (!manageTurn.IsEnemyTurn())
                    {
                        Destroy(tempObj);
                    }
                    continue;
                }
                temp.Enqueue(eneTemp);
            }
            mapEnemies = temp;

            // if (!manageTurn.IsEnemyTurn()) {
            manageTurn.RemoveEnemy(unit);
            // }

            if (playerCursor.enemyRangeActive)
            {
                pathFinder.DestroyEnemyRange();
                pathFinder.EnemyRange();
            }
            if (pathFinder.selectedEnemies.Contains(unit))
            {
                pathFinder.UnSelectEnemies(unit);
            }
            return;
        }
        else if (unit.stats.UnitType == "Player")
        {
            GameObject tempObj = unit.gameObject;
            grid.GetGridTile(x, z).UnitOnTile = null;
            mapUnits.Remove(unit.stats);
            Destroy(tempObj);
            mapGameUnits.Remove(unit);
        }
    }

    // Standard logic for printing characters on starting areas
    public virtual void PrintCharacters()
    {
        // Get the list of unit starting positions
        Vector2Int[] startPositions = GetPlayerStartPositions();

        // Gets the map units from the roster
        mapUnits = unitRos.getMapUnits();

        // Safety check
        if (startPositions.Length != mapUnits.Count)
        {
            Debug.LogWarning($"Mismatch between number of starting positions ({startPositions.Length}) and map units ({mapUnits.Count})");
        }

        int count = Mathf.Min(startPositions.Length, mapUnits.Count);

        for (int i = 0; i < count; i++)
        {
            UnitStats stats = mapUnits[i];
            Vector2Int pos = startPositions[i];

            GameObject unitPrefab = Resources.Load("Units/" + stats.UnitClass + "/" + stats.UnitName + stats.UnitClass) as GameObject;

            GridTile tile = grid.GetGridTile(pos.x, pos.y);
            Vector3 spawnPos = new Vector3(tile.GetXPos(), tile.GetYPos() + 0.005f, tile.GetZPos());

            GameObject gridUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
            PlayerUnit unitToGrid = gridUnit.GetComponent<PlayerUnit>();

            unitToGrid.XPos = pos.x;
            unitToGrid.ZPos = pos.y;

            unitToGrid.InitializeUnitData();

            tile.UnitOnTile = unitToGrid;

            mapGameUnits.Add(gridUnit.GetComponent<UnitManager>());
        }
    }

    // Initializes Enemies on the map
    protected virtual void InitEnemies()
    {

        // Determines what file to read based on the difficulty
        string[] lines;
        if (Difficulty == "Hard")
        {
            lines = enemyTextDataHard.text.Split('\n');
            maxEID = maxEIDHard;
        }
        else if (Difficulty == "Eclipse")
        {
            lines = enemyTextDataEclipse.text.Split('\n');
            maxEID = maxEIDEclipse;
        }
        else
        {
            lines = enemyTextDataNormal.text.Split('\n');
            maxEID = maxEIDNormal;
        }

        Queue<UnitManager> temp = enemyInitializer.InitEnemies(lines, 1, maxEID, grid);

        foreach (UnitManager t in temp)
        {
            mapEnemies.Enqueue(t);
        }

    }

    // Checks if specific units have died
    protected IEnumerator MissingUnitsDefeat(List<string> requiredUnitNames)
    {
        bool anyMissing = requiredUnitNames.Any(name => !mapUnits.Any(unit => unit.UnitName == name));

        if (anyMissing)
        {
            yield return StartCoroutine(MapDefeat());
        }

        yield return null;
    }

    // Standard Defeat
    // TODO: Will have to update later with a real defeat window
    protected IEnumerator MapDefeat()
    {
        yield return StartCoroutine(combatMenuManager.VicDefText("Defeat"));

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Prologue");
            }

            yield return null;
        }
    }




    // Co routine to start the map, showing the main boss (Only does one)
    // TODO: Later on make multiple, standard one for route enemies, reach certain point, and multiple bosses
    protected IEnumerator StandardShowBossStartMap(int BossX, int BossZ)
    {
        // Deactivates the hover menu
        combatMenuManager.DeactivateHoverMenu();
        yield return new WaitForSeconds(1f);

        // Moves the cursor to where the boss is
        yield return StartCoroutine(playerCursor.MoveCursor(BossX, BossZ, 40f));

        // Deactivate hover menu again
        combatMenuManager.DeactivateHoverMenu();
        yield return new WaitForSeconds(0.5f);

        // Shows the map's victory and defeat condition
        combatMenuManager.ActivateBackground();
        yield return StartCoroutine(combatMenuManager.FadeUpVD(winCondition, loseCondition));

        // Moves the cursor back to the player, simultaneously shows the player phase popup
        StartCoroutine(playerCursor.MoveCursor(playerCursor.getX(), playerCursor.getZ(), 200f));
        yield return StartCoroutine(combatMenuManager.PhaseStart("Player"));
        yield return new WaitForSeconds(0.5f);

        // Starts the game
        playerCursor.startGame = true;
    }

}
