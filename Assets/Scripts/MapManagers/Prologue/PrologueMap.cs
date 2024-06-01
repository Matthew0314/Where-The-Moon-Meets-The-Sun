using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class PrologueMap : MonoBehaviour, IMaps
{
    private UnitRosterManager unitRos;
    private WeaponManager manageWeapons;
    private UnitStats stats;
    private PlayerClassManager classRos;
    // private PlayerClass uClass;
    private GenerateGrid grid;
    private TurnManager manageTurn;   
    private string[] newUnits = { "YoungFelix", "YoungLilith" };
    private int unitNum = 2;
    private int[] startGridX = { 2, 3 };
    private int[] startGridZ = { 1, 2 };
    private int length = 10;
    private int width = 15;
    private List<UnitStats> mapUnits;
    // private EnemyStats eStats;
    [SerializeField] TextAsset enemyTextData;
    private Queue<UnitManager> mapEnemies = new Queue<UnitManager>();
  
    // Start is called before the first frame update
    void Start()
    {
        //Stores componenets that will be used later 
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        unitRos = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        classRos = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        manageWeapons = GameObject.Find("GridManager").GetComponent<WeaponManager>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();

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

        for (int i = 23; i < data.Length - 1; i += 23)
        {
            //Reads in the data from the CSV file
            string cName = data[i];
            string cDesc = data[i + 1];
            string cType = data[i + 2];
            
            int level = int.Parse(data[i + 3]);
            int HP = int.Parse(data[i + 4]);
            int ATK = int.Parse(data[i + 5]);  
            int MAG = int.Parse(data[i + 6]);
            int DEF = int.Parse(data[i + 7]);
            int RES = int.Parse(data[i + 8]);
            int SPD = int.Parse(data[i + 9]);   
            int EVA = int.Parse(data[i + 10]);
            int LUCK = int.Parse(data[i + 11]);
            int MOVE = int.Parse(data[i + 12]);
            
            bool air = bool.Parse(data[i + 13]);
            bool mount = bool.Parse(data[i + 14]);
            bool armored = bool.Parse(data[i + 15]);
            bool whisp = bool.Parse(data[i + 16]);
            int healthBars = int.Parse(data[i + 17]);

            string loadPrefab = data[i + 18];
            int enemyX = int.Parse(data[i + 19]);
            int enemyZ = int.Parse(data[i + 20]);
            string AIenemy = data[i + 21];
            
            //Stores Necessary data in EnemyStats
            UnitStats eStats = (UnitStats)Activator.CreateInstance(unitType, cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars);
            // eStats = new EnemyStats(cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars);

            Debug.Log("Init Enemy Stats");
            //Loads the enemies prefab and instantiates it on the grid tile that is specified in the CSV
            GameObject enemyPrefab = Resources.Load("Enemies/" + loadPrefab) as GameObject;
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(grid.GetGridTile(enemyX, enemyZ).GetXPos(), grid.GetGridTile(enemyX, enemyZ).GetYPos() + 0.005f, grid.GetGridTile(enemyX, enemyZ).GetZPos()), Quaternion.identity);
            // Type typeUnit = Type.GetType("EnemyUnit");
            // UnitManager enemyUnit = newEnemy.AddComponent(typeUnit) as UnitManager;

            //Ataches an AI script interface depending on the characterististics of the nemy specified in the CSV file
            Type type = Type.GetType(AIenemy);
            IEnemyAI enemyAI = newEnemy.AddComponent(type) as IEnemyAI;
            
            //Stores the enemy stats in the EnemyUnit object and stores in a queue
            UnitManager enemy = newEnemy.GetComponent<UnitManager>();
            enemy.stats = eStats;
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
            // Type typePUnit = Type.GetType("PlayerUnit");
            // UnitManager enemyUnit = unitPrefab.AddComponent(typePUnit) as UnitManager;
        }
    }


    //The clear condition for the prologue is routing all the enemies 
    public void CheckClearCondition()
    {
        //check to see if all enemies are removed from the list
        //implement later
    }

    //Niether YoungFelix nor YoungLilith can die, check to see if alive
    public void CheckDefeatCondition()
    {
        //Check to see if felix and lilith have been removed after every action
        //Game over if they have been removed
    }

    public Queue<UnitManager> GetMapEnemies() {
        return mapEnemies;
    }



}
