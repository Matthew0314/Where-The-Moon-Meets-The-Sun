using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Linq;
using UnityEngine;

public class PrologueMap : MonoBehaviour, IMaps
{
    private UnitRosterManager unitRos;
    private WeaponManager manageWeapons;
    private UnitStats stats;
    private PlayerClassManager classRos;
    private PlayerGridMovement playerCursor;
    private FindPath pathFinder;
    private GenerateGrid grid;
    private TurnManager manageTurn;   
    private string[] newUnits = { "YoungFelix", "YoungLilith" };
    private int unitNum = 2;
    private int[] startGridX = { 9, 10 };
    private int[] startGridZ = { 2, 1 };
    private int length = 16;
    private int width = 24;
    private List<UnitStats> mapUnits;
    private List<UnitManager> mapGameUnits = new List<UnitManager>();
    [SerializeField] TextAsset enemyTextData;
    private Queue<UnitManager> mapEnemies = new Queue<UnitManager>();

    bool calledReinforcements = false;
  
    // Start is called before the first frame update
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

        //Reads in data for weapons, all units in the game, and all player classes
        //THESE WILL NEVER BE CALLED AGAIN AFTER THE PROLOGUE MAP
        manageWeapons.ReadCSV();
        classRos.Init();
        unitRos.ReadCSV();
        

        //Initilizes the prologue map
        Init();
    }


    public void Init()
    {
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

        manageTurn.SetLists();
        manageTurn.SetEnemyList();
    }

    //Reads in the EnemyCSV, stores each of their data, and prints them on the grid
    private void InitEnemies() {
        
        string[] data = enemyTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        Type unitType = Type.GetType("EnemyStats");

        for (int i = 31; i < 186; i += 31)
        {
            //Reads in the data from the CSV file
           
            int eID = int.Parse(data[i]);
            string cName = data[i + 1];
            string cDesc = data[i + 2];
            string cType = data[i + 3];
            
            int level = int.Parse(data[i + 4]);
            int HP = int.Parse(data[i + 5]);
            int ATK = int.Parse(data[i + 6]);  
            int MAG = int.Parse(data[i + 7]);
            int DEF = int.Parse(data[i + 8]);
            int RES = int.Parse(data[i + 9]);
            int SPD = int.Parse(data[i + 10]);   
            int EVA = int.Parse(data[i + 11]);
            int LUCK = int.Parse(data[i + 12]);
            int MOVE = int.Parse(data[i + 13]);
            
            bool air = bool.Parse(data[i + 14]);
            bool mount = bool.Parse(data[i + 15]);
            bool armored = bool.Parse(data[i + 16]);
            bool whisp = bool.Parse(data[i + 17]);
            int healthBars = int.Parse(data[i + 18]);

            string loadPrefab = data[i + 19];
            int enemyX = int.Parse(data[i + 20]);
            int enemyZ = int.Parse(data[i + 21]);
            string AIenemy = data[i + 22];

            string item1 = data[i + 23];
            string item2 = data[i + 24];
            string item3 = data[i + 25];
            string item4 = data[i + 26];
            string item5 = data[i + 27];
            string item6 = data[i + 28];

            bool boss = bool.Parse(data[i + 29]);
       
            
            //Stores Necessary data in EnemyStats
            UnitStats eStats = (UnitStats)Activator.CreateInstance(unitType, eID, cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars, boss);

            for (int j = 0; j < 6; j++) {
                if (data[i + 23 + j] == "NULL") {              
                    break;
                }

                Weapon tempWeapon = WeaponManager.MakeWeapon(data[i + 23 + j]);
                eStats.AddWeapon(tempWeapon);
            }

          
           
            //Loads the enemies prefab and instantiates it on the grid tile that is specified in the CSV
            GameObject enemyPrefab = Resources.Load("Enemies/" + loadPrefab) as GameObject;
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(grid.GetGridTile(enemyX, enemyZ).GetXPos(), grid.GetGridTile(enemyX, enemyZ).GetYPos() + 0.005f, grid.GetGridTile(enemyX, enemyZ).GetZPos()), Quaternion.identity);
           

            //Ataches an AI script interface depending on the characterististics of the nemy specified in the CSV file
            Type type = Type.GetType(AIenemy);
            IEnemyAI enemyAI = newEnemy.AddComponent(type) as IEnemyAI;
            
            //Stores the enemy stats in the EnemyUnit object and stores in a queue
            UnitManager enemy = newEnemy.GetComponent<UnitManager>();
            
            enemy.stats = eStats;

            enemy.InitializeUnitData();
            enemy.XPos = enemyX;
            enemy.ZPos = enemyZ;
            grid.GetGridTile(enemyX, enemyZ).UnitOnTile = enemy;
            mapEnemies.Enqueue(enemy);
                      

            
        }
    }

    //Adds Young Felix and Young Lilith to the players roster
    public void AddNewPlayers()
    {
        for (int i = 0; i < newUnits.Length; i++)
        {
            unitRos.AddPlayableUnit(newUnits[i]);
        }

        
    }

    public void PrintCharacters()
    {
        //Gets a copy of the map units
        mapUnits = unitRos.getMapUnits();

        //For the amount of units that is allowed for this map
        for (int i=0; i < unitNum; i++)
        {
            //Loads the prefab based on the units class and instantiates it on one of the positions specified by StartGrid
            stats = mapUnits[i];
            GameObject unitPrefab = Resources.Load("Units/" + stats.UnitClass + "/" + stats.UnitName + stats.UnitClass) as GameObject;
            Instantiate(unitPrefab, new Vector3(grid.GetGridTile(startGridX[i], startGridZ[i]).GetXPos(), grid.GetGridTile(startGridX[i], startGridZ[i]).GetYPos() + 0.005f, grid.GetGridTile(startGridX[i], startGridZ[i]).GetZPos()), Quaternion.identity);
            PlayerUnit UnitToGrid = unitPrefab.GetComponent<PlayerUnit>();
            UnitToGrid.XPos = startGridX[i];
            UnitToGrid.ZPos = startGridZ[i];
            // mapUnits.Add(UnitToGrid);
            // UnitToGrid.InitializeUnitData();
            grid.GetGridTile(startGridX[i], startGridZ[i]).UnitOnTile = UnitToGrid;
            mapGameUnits.Add(unitPrefab.GetComponent<UnitManager>());
            // Debug.Log("Assigned to Tile")
            // Type typePUnit = Type.GetType("PlayerUnit");
            // UnitManager enemyUnit = unitPrefab.AddComponent(typePUnit) as UnitManager;
        }
    }


    //The clear condition for the prologue is routing all the enemies 
    public void CheckClearCondition()
    {
        if (mapEnemies.Count == 0) {
            Debug.Log("VICTORY");
        }
    }

    //Niether YoungFelix nor YoungLilith can die, check to see if alive
    public void CheckDefeatCondition()
    {
        if (!mapUnits.Any(unit => unit.UnitName == "YoungFelix") || !mapUnits.Any(unit => unit.UnitName == "YoungLilith")) {
            Debug.Log("DEFEAT");
        }
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
        if (!calledReinforcements && callNewEnemies && manageTurn.IsEnemyTurn()) {
            

            string[] data = enemyTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
            Type unitType = Type.GetType("EnemyStats");
            for (int i = 31; i < data.Length - 1; i += 31) {

                int eID = int.Parse(data[i]);
                if (eID < 6) {
                    continue;
                }
                string cName = data[i + 1];
                string cDesc = data[i + 2];
                string cType = data[i + 3];
                
                int level = int.Parse(data[i + 4]);
                int HP = int.Parse(data[i + 5]);
                int ATK = int.Parse(data[i + 6]);  
                int MAG = int.Parse(data[i + 7]);
                int DEF = int.Parse(data[i + 8]);
                int RES = int.Parse(data[i + 9]);
                int SPD = int.Parse(data[i + 10]);   
                int EVA = int.Parse(data[i + 11]);
                int LUCK = int.Parse(data[i + 12]);
                int MOVE = int.Parse(data[i + 13]);
                
                bool air = bool.Parse(data[i + 14]);
                bool mount = bool.Parse(data[i + 15]);
                bool armored = bool.Parse(data[i + 16]);
                bool whisp = bool.Parse(data[i + 17]);
                int healthBars = int.Parse(data[i + 18]);

                string loadPrefab = data[i + 19];
                int enemyX = int.Parse(data[i + 20]);
                int enemyZ = int.Parse(data[i + 21]);
                string AIenemy = data[i + 22];

                string item1 = data[i + 23];
                string item2 = data[i + 24];
                string item3 = data[i + 25];
                string item4 = data[i + 26];
                string item5 = data[i + 27];
                string item6 = data[i + 28];

                bool boss = bool.Parse(data[i + 29]);
        
                
                //Stores Necessary data in EnemyStats
                UnitStats eStats = (UnitStats)Activator.CreateInstance(unitType, eID, cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars, boss);

                for (int j = 0; j < 6; j++) {
                    if (data[i + 23 + j] == "NULL") {              
                        break;
                    }

                    Weapon tempWeapon = WeaponManager.MakeWeapon(data[i + 23 + j]);
                    eStats.AddWeapon(tempWeapon);
                }

                yield return StartCoroutine(playerCursor.MoveCursor(enemyX,enemyZ));

               
            
                //Loads the enemies prefab and instantiates it on the grid tile that is specified in the CSV
                GameObject enemyPrefab = Resources.Load("Enemies/" + loadPrefab) as GameObject;
                GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(grid.GetGridTile(enemyX, enemyZ).GetXPos(), grid.GetGridTile(enemyX, enemyZ).GetYPos() + 0.005f, grid.GetGridTile(enemyX, enemyZ).GetZPos()), Quaternion.identity);
            

                //Ataches an AI script interface depending on the characterististics of the nemy specified in the CSV file
                Type type = Type.GetType(AIenemy);
                IEnemyAI enemyAI = newEnemy.AddComponent(type) as IEnemyAI;
                
                //Stores the enemy stats in the EnemyUnit object and stores in a queue
                UnitManager enemy = newEnemy.GetComponent<UnitManager>();
                
                enemy.stats = eStats;

                enemy.InitializeUnitData();
                enemy.XPos = enemyX;
                enemy.ZPos = enemyZ;
                grid.GetGridTile(enemyX, enemyZ).UnitOnTile = enemy;
                mapEnemies.Enqueue(enemy);
                manageTurn.AddEnemy(enemy);

                if (playerCursor.enemyRangeActive) {
                    pathFinder.DestroyEnemyRange();
                    pathFinder.EnemyRange();
                }

                yield return new WaitForSeconds(1f);

                
            }

            calledReinforcements = true;
        }

        yield return null;
    }

    public Queue<UnitManager> GetMapEnemies() {
        return mapEnemies;
    }

    public List<UnitStats> GetMapUnitStats() {
        return mapUnits;
    }

    public List<UnitManager> GetMapUnits() {
        return mapGameUnits;
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

            Debug.Log("AHHHHHHHHHHHHHHHHHHHHHH " + mapEnemies.Count + " " + queueCou);

            // if (!manageTurn.IsEnemyTurn()) {
                manageTurn.RemoveEnemy(unit);
            // }
            Debug.Log("AHHHHHHHHHHHHHHHHHHHHHH " + mapEnemies.Count + " " + queueCou);

            
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

    public int GetLength() {
        return length;
    }

    public int GetWidth() { return width; }



}
