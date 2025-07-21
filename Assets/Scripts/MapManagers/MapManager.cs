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
    protected BattleStartMenu battleStartMenu;


    // Initialized Difficult variable and text data
    protected string Difficulty = "Normal";
    [SerializeField] protected TextAsset enemyTextDataNormal;
    [SerializeField] protected TextAsset enemyTextDataHard;
    [SerializeField] protected TextAsset enemyTextDataEclipse;
    protected int maxEID;


    [SerializeField] protected GameObject playerStartTile;
    protected List<GameObject> startTiles = new List<GameObject>();


    // Info about map size, conditions, and how many units
    //* MUST BE INITIALIZED IN AWAKE IN EVERY CHILD CLASS
    [SerializeField] protected int length = 1;
    [SerializeField] protected int width = 1;
    [SerializeField] protected string winCondition = "";
    [SerializeField] protected string loseCondition = "";
    [SerializeField] protected int maxEIDNormal = 0;
    [SerializeField] protected int maxEIDHard = 0;
    [SerializeField] protected int maxEIDEclipse = 0;
    [SerializeField] protected Vector2Int[] playerStartPosition;
    [SerializeField] protected Vector2Int primaryStart;



    // Lists of Players, Enemies and Allies
    // * Map Allies and Map Enemies 2 will not always be used
    protected List<UnitStats> mapUnits = new List<UnitStats>();
    protected List<UnitManager> mapGameUnits = new List<UnitManager>();
    protected Queue<UnitManager> mapEnemies = new Queue<UnitManager>();
    protected Queue<UnitManager> mapEnemies2 = new Queue<UnitManager>();
    protected Queue<UnitManager> mapAllies = new Queue<UnitManager>();
    [SerializeField] protected List<string> requiredUnits = new List<string>();
    [SerializeField] protected List<string> forbiddenUnits = new List<string>();

    // Getters for each Queues and Lists
    public virtual Queue<UnitManager> GetMapEnemies1() => mapEnemies;
    public virtual Queue<UnitManager> GetMapEnemies2() => mapEnemies2;
    public virtual Queue<UnitManager> GetMapAllies() => mapAllies;
    public virtual List<UnitStats> GetMapUnitStats() => mapUnits;
    public virtual List<UnitManager> GetMapUnits() => mapGameUnits;
    public virtual List<string> GetRequiredUnits() => requiredUnits;
    public virtual List<string> GetForbiddenUnits() => forbiddenUnits;

    public virtual int GetNumberOfStartUnits() => playerStartPosition.Length;

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
    // public abstract ;
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
        battleStartMenu = GameObject.Find("Canvas").GetComponent<BattleStartMenu>();
    }

    protected virtual void Start()
    {
        // Init();
    }

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
        InitMapUnits();

        //Prints the map units on the map
        PrintCharacters();

        //Prints the enemies on the map
        InitEnemies();

        unitRos.setFaithSpells();

        // manageTurn.SetLists();
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

    protected virtual void InitMapUnits()
    {
        mapUnits.Clear(); // optional, to reset the list

        // Step 1: Add required units
        foreach (string name in requiredUnits)
        {
            UnitStats temp = UnitRosterManager.GetPlayableUnit(name);
            if (temp != null && !forbiddenUnits.Contains(temp.UnitName))
            {
                mapUnits.Add(temp);
            }
        }

        // Step 2: Fill remaining slots from the full playable pool
        List<UnitStats> allPlayable = UnitRosterManager.GetPlayableUnits();

        foreach (UnitStats unit in allPlayable)
        {
            if (mapUnits.Count >= playerStartPosition.Length)
                break;

            if (mapUnits.Contains(unit)) // already added (e.g. requiredUnits)
                continue;

            if (forbiddenUnits.Contains(unit.UnitName))
                continue;

            mapUnits.Add(unit);
        }
    }

    public virtual void InitStartTiles()
    {
        foreach (Vector2Int pos in GetPlayerStartPositions())
        {
            GridTile tile = grid.GetGridTile(pos.x, pos.y);

            Vector3 spawnPos = new Vector3(tile.GetXPos(), tile.GetYPos() + 0.005f, tile.GetZPos());

            GameObject temp = Instantiate(playerStartTile, spawnPos, Quaternion.identity);

            startTiles.Add(temp);
        }
    }

    public virtual void DestroyStartTiles()
    {
        foreach (GameObject tile in startTiles)
        {
            Destroy(tile);
        }
    }

    public virtual Vector2Int[] GetPlayerStartPositions() => playerStartPosition;

    // Standard logic for removing a Dead Unit
    // TODO: Review Logic for any bugs
    public virtual void RemoveDeadUnit(UnitManager unit, int x, int z)
    {

        if (unit.GetUnitType() == "Enemy")
        {
            Queue<UnitManager> temp = mapEnemies;

            int queueCou = temp.Count;

            for (int i = 0; i < queueCou; i++)
            {
                EnemyUnit eneTemp = temp.Dequeue() as EnemyUnit;
                EnemyUnit unitTemp = unit as EnemyUnit;
                if (eneTemp.GetUnitID() == unitTemp.GetUnitID())
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
        else if (unit.GetUnitType() == "Player")
        {
            GameObject tempObj = unit.gameObject;
            grid.GetGridTile(x, z).UnitOnTile = null;
            mapUnits.Remove(unit.GetStats());
            Destroy(tempObj);
            mapGameUnits.Remove(unit);
            manageTurn.RemovePlayer(unit.GetStats());
        }
    }


    public virtual void PrintCharacters()
    {
        // Get the list of unit starting positions
        Vector2Int[] startPositions = GetPlayerStartPositions();

        // Safety check
        if (startPositions.Length != mapUnits.Count)
        {
            Debug.LogWarning($"Mismatch between number of starting positions ({startPositions.Length}) and map units ({mapUnits.Count})");
        }

        // int count = Mathf.Min(startPositions.Length, mapUnits.Count);
        int spawnIndex = 0;

        foreach (UnitStats stats in mapUnits)
        {
            Vector2Int pos = startPositions[spawnIndex++];
            SpawnUnit(stats, pos);
        }
    }

    public void SpawnUnit(UnitStats stats, Vector2Int pos)
    {
        GameObject unitPrefab = Resources.Load("Units/" + stats.UnitClass + "/" + stats.UnitName + stats.UnitClass) as GameObject;

        GridTile tile = grid.GetGridTile(pos.x, pos.y);
        Vector3 spawnPos = new Vector3(tile.GetXPos(), tile.GetYPos() + 0.005f, tile.GetZPos());

        GameObject gridUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        PlayerUnit unitToGrid = gridUnit.GetComponent<PlayerUnit>();

        unitToGrid.XPos = pos.x;
        unitToGrid.ZPos = pos.y;

        unitToGrid.InitializeUnitData();

        tile.UnitOnTile = unitToGrid;

        if (!mapUnits.Contains(stats)) mapUnits.Add(stats);
        mapGameUnits.Add(gridUnit.GetComponent<UnitManager>());

        manageTurn.AddPlayer(stats);
    }

    public void DespawnUnit(UnitManager unit)
    {
        mapUnits.Remove(unit.GetStats());
        mapGameUnits.Remove(unit);

        grid.GetGridTile(unit.XPos, unit.ZPos).UnitOnTile = null;

        Destroy(unit.gameObject);
        manageTurn.RemovePlayer(unit.GetStats());
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
        if (primaryStart == null)
        {
            StartCoroutine(playerCursor.MoveCursor(playerCursor.getX(), playerCursor.getZ(), 200f));
        }
        else
        {
            StartCoroutine(playerCursor.MoveCursor(primaryStart.x, primaryStart.y, 200f));
            playerCursor.SetX(primaryStart.x);
            playerCursor.SetZ(primaryStart.y);
        }


        yield return StartCoroutine(combatMenuManager.PhaseStart("Player"));
        yield return new WaitForSeconds(0.5f);

        // Starts the game
        playerCursor.startGame = true;
    }

    public virtual Vector2Int FindNextStartPosition()
    {
        foreach (Vector2Int t in playerStartPosition)
        {
            if (grid.GetGridTile(t.x, t.y).UnitOnTile == null)
            {
                return t;
            }
        }

        return new Vector2Int(-1, -1);
    }

}
