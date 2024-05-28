using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class PrologueMap : MonoBehaviour, IMaps
{
    private UnitRosterManager unitRos;
    private UnitStats stats;
    private PlayerClassManager classRos;
    private PlayerClass uClass;
    private GenerateGrid grid;
    private string[] newUnits = { "YoungFelix", "YoungLilith" };
    private int unitNum = 2;
    private int[] startGridX = { 2, 3 };
    private int[] startGridZ = { 1, 2 };
    private int length = 10;
    private int width = 10;
    private List<UnitStats> mapUnits;
    private EnemyStats eStats;
    public TextAsset enemyTextData;
    private List<EnemyUnit> mapEnemies = new List<EnemyUnit>();
    
    // [SerializeField] GameObject felix; //Remove Later
    // [SerializeField] GameObject lilith; //Remove Later

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        unitRos = GameObject.Find("GridManager").GetComponent<UnitRosterManager>();
        classRos = GameObject.Find("GridManager").GetComponent<PlayerClassManager>();
        unitRos.ReadCSV();
        classRos.Init();
        Init();

    }


    public void Init()
    {
        grid.GenGrid(length, width);
        AddNewPlayers();
        unitRos.InitMapUnit(unitNum);
        PrintCharacters();
        InitEnemies();
    }

    private void InitEnemies() {
        
        string[] data = enemyTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        for (int i = 23; i < data.Length - 1; i += 23)
        {
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
            

            eStats = new EnemyStats(cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars);

            string loadPrefab = data[i + 18];
            int enemyX = int.Parse(data[i + 19]);
            int enemyZ = int.Parse(data[i + 20]);
            string AIenemy = data[i + 21];
            
            GameObject enemyPrefab = Resources.Load("Enemies/" + loadPrefab) as GameObject;
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(grid.grid[enemyX, enemyZ].GetXPos(), grid.grid[enemyX, enemyZ].GetYPos() + 0.005f, grid.grid[enemyX, enemyZ].GetZPos()), Quaternion.identity);
            
            Type type = Type.GetType(AIenemy);
            IEnemyAI enemyAI = newEnemy.AddComponent(type) as IEnemyAI;
            Debug.Log("Hello Enemies");
            EnemyUnit enemy = newEnemy.GetComponent<EnemyUnit>();
            enemy.stats = eStats;
            Debug.Log("Hello Enemies");
            mapEnemies.Add(enemy);

            
        }
    }

    public void AddNewPlayers()
    {
        for (int i = 0; i < newUnits.Length; i++)
        {
            unitRos.AddPlayableUnit(newUnits[i]);
        }
    }

    public void PrintCharacters()
    {
        mapUnits = unitRos.getMapUnits();
        for (int i=0; i < unitNum; i++)
        {
            
            stats = mapUnits[i];
            GameObject unitPrefab = Resources.Load("Units/" + stats.UnitClass + "/" + stats.UnitName + stats.UnitClass) as GameObject;
            Instantiate(unitPrefab, new Vector3(grid.grid[startGridX[i], startGridZ[i]].GetXPos(), grid.grid[startGridX[i], startGridZ[i]].GetYPos() + 0.005f, grid.grid[startGridX[i], startGridZ[i]].GetZPos()), Quaternion.identity);

            /*if (stats.UnitName == "YoungFelix")
            {
                Instantiate(felix, new Vector3(grid.grid[startGridX[i], startGridZ[i]].GetXPos(), grid.grid[startGridX[i], startGridZ[i]].GetYPos() + 0.005f, grid.grid[startGridX[i], startGridZ[i]].GetZPos()), Quaternion.identity);
            }
            else if (stats.UnitName == "YoungLilith")
            {
                Instantiate(lilith, new Vector3(grid.grid[startGridX[i], startGridZ[i]].GetXPos(), grid.grid[startGridX[i], startGridZ[i]].GetYPos() + 0.005f, grid.grid[startGridX[i], startGridZ[i]].GetZPos()), Quaternion.identity);
            }*/
        }
    }


    //The clear condition for the prologue is routing all the enemies 
    public void CheckClearCondition()
    {
        //check to see if all enemies are removed from the list
        //implement later
    }

    //Niether YoungFelix nor YoungLilith can die, check to see if alive
    public void CheckMainChars()
    {
        //Check to see if felix and lilith have been removed after every action
        //Game over if they have been removed
    }



}
