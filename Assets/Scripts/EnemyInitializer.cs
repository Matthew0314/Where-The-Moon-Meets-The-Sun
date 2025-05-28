using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EnemyInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public static Queue<UnitManager> InitEnemies(string[] lines, int startEID, int maxEID, GenerateGrid grid) {
        Type unitType = Type.GetType("EnemyStats");
        Queue<UnitManager> mapEnemies = new Queue<UnitManager>();

        foreach (string line in lines.Skip(1)) // skip header
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Trim().Split(',');

            int index = 0;
            int eID = int.Parse(fields[index++]);

            if (eID < startEID) continue;

            string cName = fields[index++];
            string cDesc = fields[index++];
            string cType = fields[index++];

            int level = int.Parse(fields[index++]);
            int HP = int.Parse(fields[index++]);
            int ATK = int.Parse(fields[index++]);
            int MAG = int.Parse(fields[index++]);
            int DEF = int.Parse(fields[index++]);
            int RES = int.Parse(fields[index++]);
            int SPD = int.Parse(fields[index++]);
            int EVA = int.Parse(fields[index++]);
            int LUCK = int.Parse(fields[index++]);
            int MOVE = int.Parse(fields[index++]);

            bool air = bool.Parse(fields[index++]);
            bool mount = bool.Parse(fields[index++]);
            bool armored = bool.Parse(fields[index++]);
            bool whisp = bool.Parse(fields[index++]);
            int healthBars = int.Parse(fields[index++]);

            string loadPrefab = fields[index++];
            int enemyX = int.Parse(fields[index++]);
            int enemyZ = int.Parse(fields[index++]);
            string AIenemy = fields[index++];

            string[] items = new string[6];
            for (int j = 0; j < 6; j++) items[j] = fields[index++];

            bool boss = bool.Parse(fields[index++]);

            UnitStats eStats = (UnitStats)Activator.CreateInstance(
                unitType, eID, cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars, boss);

            foreach (string item in items)
            {
                if (item == "NULL") break;
                Weapon tempWeapon = WeaponManager.MakeWeapon(item);
                eStats.AddWeapon(tempWeapon);
            }
           
            //Loads the enemies prefab and instantiates it on the grid tile that is specified in the CSV
            GameObject enemyPrefab = Resources.Load("Enemies/" + loadPrefab) as GameObject;
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(grid.GetGridTile(enemyX, enemyZ).GetXPos(), grid.GetGridTile(enemyX, enemyZ).GetYPos(), grid.GetGridTile(enemyX, enemyZ).GetZPos()), Quaternion.identity);

            //Ataches an AI script interface depending on the characterististics of the nemy specified in the CSV file
            Type type = Type.GetType(AIenemy);
            IEnemyAI enemyAI = newEnemy.AddComponent(type) as IEnemyAI;
            
            //Stores the enemy stats in the EnemyUnit object and stores in a queue
            UnitManager enemy = newEnemy.GetComponent<UnitManager>();
            
            enemy.stats = eStats;
            if (enemy.stats.weapons.Count > 0) enemy.primaryWeapon = enemy.stats.weapons[0];

            enemy.InitializeUnitData();
            enemy.XPos = enemyX;
            enemy.ZPos = enemyZ;
            grid.GetGridTile(enemyX, enemyZ).UnitOnTile = enemy;
            mapEnemies.Enqueue(enemy);

            if(eID >= maxEID) break;
                      

            
        }

        return mapEnemies;

    }

    // public static IEnumerator SpawnReinforcements(string[] lines, int startEID, int maxEID, GridControl grid) {

    // }

    // private UnitStats ParseEnemyStats(string[] fields, int eID, Type unitType) {

    //     int index = 1; // Start after eID (already parsed)

    //     string cName = fields[index++];
    //     string cDesc = fields[index++];
    //     string cType = fields[index++];

    //     int level = int.Parse(fields[index++]);
    //     int HP = int.Parse(fields[index++]);
    //     int ATK = int.Parse(fields[index++]);
    //     int MAG = int.Parse(fields[index++]);
    //     int DEF = int.Parse(fields[index++]);
    //     int RES = int.Parse(fields[index++]);
    //     int SPD = int.Parse(fields[index++]);
    //     int EVA = int.Parse(fields[index++]);
    //     int LUCK = int.Parse(fields[index++]);
    //     int MOVE = int.Parse(fields[index++]);

    //     bool air = bool.Parse(fields[index++]);
    //     bool mount = bool.Parse(fields[index++]);
    //     bool armored = bool.Parse(fields[index++]);
    //     bool whisp = bool.Parse(fields[index++]);
    //     int healthBars = int.Parse(fields[index++]);

    //     bool boss = false;

    //     // Create the UnitStats instance
    //     UnitStats eStats = (UnitStats)Activator.CreateInstance(
    //         unitType, eID, cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars, boss
    //     );

    //     return eStats;
    // }



}
