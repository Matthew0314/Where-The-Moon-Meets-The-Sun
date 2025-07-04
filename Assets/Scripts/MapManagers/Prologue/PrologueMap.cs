using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;

public class PrologueMap : MonoBehaviour, IMaps
{
    
    private UnitRosterManager unitRos;
    private WeaponManager manageWeapons;
    // private UnitStats stats;
    private PlayerClassManager classRos;
    private PlayerGridMovement playerCursor;
    private FindPath pathFinder;
    private GenerateGrid grid;
    private TurnManager manageTurn;  
    private CombatMenuManager combatMenuManager;
    private string[] newUnits = { "YoungFelix", "YoungLilith" };
    private int unitNum = 2;
    private int[] startGridX = { 9, 10 };
    private int[] startGridZ = { 2, 1 };
    private int length = 16;
    private int width = 24;
    private List<UnitStats> mapUnits;
    private List<UnitManager> mapGameUnits = new List<UnitManager>();
    [SerializeField] TextAsset enemyTextDataNormal;
    [SerializeField] TextAsset enemyTextDataHard;
    [SerializeField] TextAsset enemyTextDataEclipse;
    [SerializeField] EnemyInitializer enemyInitializer;
    private Queue<UnitManager> mapEnemies = new Queue<UnitManager>();
    private string winCondition = "Defeat the boss.";
    private string loseCondition = "<color=#3160BC>Felix</color> or <color=#3160BC>Lilith</color> falls in battle.";

    bool calledReinforcements = false;
    string Difficulty = "Normal";
    int maxEID;
    private ExecuteAction executeAction;
  
    void Start()
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

        //Reads in data for weapons, all units in the game, and all player classes
        // ! THESE WILL NEVER BE CALLED AGAIN AFTER THE PROLOGUE MAP
        manageWeapons.ReadCSV();
        classRos.Init();
        unitRos.ReadCSV();
        

        //Initilizes the prologue map
        Init();
    }


    public void Init()
    {
        //Gets the Difficulty
        string temp = TitleScreen.GetDifficulty();
        if (temp == " ") Difficulty = "Normal";
        else Difficulty = temp;

        //Calls GenerateGrid.cs to generate the grid based on how big it is, specified by length and width variables
        grid.GenGrid(length, width);

        //If there are new players being added to the players roster, this will be called
        AddNewPlayers();

        //Initilizes what units will be on the map initially
        unitRos.InitMapUnit(unitNum);

        //Prints the map units on the map
        PrintCharacters();

        //Prints the enemies on the map
        InitEnemies();

        unitRos.setFaithSpells();

        manageTurn.SetLists();
        manageTurn.SetEnemyList();

        StartCoroutine(StartMap());
    }

    private void InitEnemies() {

        // Determines what file to read based on the difficulty
        string[] lines;
        if (Difficulty == "Hard") {
            lines = enemyTextDataHard.text.Split('\n');
            maxEID = 5;
        } else if (Difficulty == "Eclipse") {
            lines = enemyTextDataEclipse.text.Split('\n');
            maxEID = 8;
        } else {
            lines = enemyTextDataNormal.text.Split('\n');
            maxEID = 5;
        }

        Queue<UnitManager> temp = enemyInitializer.InitEnemies(lines, 1, maxEID, grid);

        foreach (UnitManager t in temp) {
            mapEnemies.Enqueue(t);
        }

    }
    //Adds Young Felix and Young Lilith to the players roster
    public void AddNewPlayers() {
        for (int i = 0; i < newUnits.Length; i++) {
            UnitRosterManager.AddPlayableUnit(newUnits[i]);
        }
    }

    public void PrintCharacters() {
        //Gets a copy of the map units
        mapUnits = unitRos.getMapUnits();

        //For the amount of units that is allowed for this map
        for (int i=0; i < unitNum; i++) {
            //Loads the prefab based on the units class and instantiates it on one of the positions specified by StartGrid
            UnitStats stats = mapUnits[i];
            GameObject unitPrefab = Resources.Load("Units/" + stats.UnitClass + "/" + stats.UnitName + stats.UnitClass) as GameObject;

            //Instantiates unit at certain position
            GameObject gridUnit = Instantiate(unitPrefab, new Vector3(grid.GetGridTile(startGridX[i], startGridZ[i]).GetXPos(), grid.GetGridTile(startGridX[i], startGridZ[i]).GetYPos() + 0.005f, grid.GetGridTile(startGridX[i], startGridZ[i]).GetZPos()), Quaternion.identity);
            PlayerUnit UnitToGrid = gridUnit.GetComponent<PlayerUnit>();
            
            // Assignes the X and Y position to the UnitManager assigned to the unit prefab
            UnitToGrid.XPos = startGridX[i];
            UnitToGrid.ZPos = startGridZ[i];

            // Initializes Unit data
            UnitToGrid.InitializeUnitData();

            // Assignes the UnitManager to the grid tile
            grid.GetGridTile(startGridX[i], startGridZ[i]).UnitOnTile = UnitToGrid;

            // Adds it to map game units list
            mapGameUnits.Add(unitPrefab.GetComponent<UnitManager>());
        }
    }


    //The clear condition for the prologue is routing all the enemies 
    public IEnumerator CheckClearCondition()
    {
        if (!mapEnemies.Any(unit => unit.stats.EnemyID == 5)) {
            yield return StartCoroutine(combatMenuManager.VicDefText("Victory"));
            playerCursor.startGame = false;
            while (true) {

                Debug.Log("VICTORY!!!");
                yield return null;
            }
            
        }

        yield return null;
    }

    //Niether YoungFelix nor YoungLilith can die, check to see if alive
    public IEnumerator CheckDefeatCondition()
    {
        if (!mapUnits.Any(unit => unit.UnitName == "YoungFelix") || !mapUnits.Any(unit => unit.UnitName == "YoungLilith")) {
            yield return StartCoroutine(combatMenuManager.VicDefText("Defeat"));
            while (true) {
                if (Input.GetKeyDown(KeyCode.R)) {
                    SceneManager.LoadScene("Prologue");
                }

                yield return null;
            }
            
        }

        yield return null;
    }

    public IEnumerator CheckEvents() {
        bool callNewEnemies = false;

        if (!calledReinforcements && manageTurn.IsEnemyTurn()) {
            for (int i = 11; i <= 16; i++) {
                for (int j = 7; j <= 15; j++) {
                    if (grid.GetGridTile(i,j).UnitOnTile != null && grid.GetGridTile(i,j).UnitOnTile.UnitType == "Player") {
                        callNewEnemies = true;
                    }
                }
            }
        }
        if (!calledReinforcements && callNewEnemies && manageTurn.IsEnemyTurn() && Difficulty != "Normal") {

            string[] data;

            if (Difficulty == "Hard") data = enemyTextDataHard.text.Split('\n');
            else if (Difficulty == "Eclipse") data = enemyTextDataEclipse.text.Split('\n');
            else data = enemyTextDataNormal.text.Split('\n');

            Type unitType = Type.GetType("EnemyStats");

            yield return StartCoroutine(enemyInitializer.SpawnReinforcements(data, maxEID, 99, grid, playerCursor, pathFinder, manageTurn, mapEnemies));

            calledReinforcements = true;
        }

        yield return null;
    }



    public void RemoveDeadUnit(UnitManager unit, int x, int z) {

        if (unit.stats.UnitType == "Enemy") {
            Queue<UnitManager> temp = mapEnemies;

            int queueCou = temp.Count;
            
            for (int i = 0; i < queueCou; i++) {
                UnitManager eneTemp = temp.Dequeue();
                if (eneTemp.stats.EnemyID == unit.stats.EnemyID) {
                    grid.GetGridTile(x, z).UnitOnTile = null;
                    GameObject tempObj = eneTemp.gameObject;
                    if (!manageTurn.IsEnemyTurn()) {
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
            
            if (playerCursor.enemyRangeActive) {
                pathFinder.DestroyEnemyRange();
                pathFinder.EnemyRange();
            }
            if (pathFinder.selectedEnemies.Contains(unit)) {
                pathFinder.UnSelectEnemies(unit);
            }
            return;
        } else if (unit.stats.UnitType == "Player") {
            GameObject tempObj = unit.gameObject;
            grid.GetGridTile(x, z).UnitOnTile = null;
            mapUnits.Remove(unit.stats);
            Destroy(tempObj);
            mapGameUnits.Remove(unit);
        }
    }

    // Co routine to start the map
    public IEnumerator StartMap() {
        // Deactivates the hover menu
        combatMenuManager.DeactivateHoverMenu();
        yield return new WaitForSeconds(1f);

        // Moves the cursor to where the boss is
        yield return StartCoroutine(playerCursor.MoveCursor(21, 13, 40f));

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

    public int GetLength() => length;

    public int GetWidth() => width;
    public string GetDifficulty() => Difficulty;
    public Queue<UnitManager> GetMapEnemies1() => mapEnemies;
    public Queue<UnitManager> GetMapEnemies2() => null;
    public Queue<UnitManager> GetMapAllies() => null;

    public List<UnitStats> GetMapUnitStats() => mapUnits;

    public List<UnitManager> GetMapUnits() => mapGameUnits;
}
