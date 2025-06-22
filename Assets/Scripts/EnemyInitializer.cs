using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EnemyInitializer : MonoBehaviour
{

    public Queue<UnitManager> InitEnemies(string[] lines, int startEID, int maxEID, GenerateGrid grid)
    {
        Type unitType = Type.GetType("EnemyStats");
        Queue<UnitManager> mapEnemies = new Queue<UnitManager>();

        foreach (string line in lines.Skip(1)) // skip header
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Trim().Split(',');

            int eID = int.Parse(fields[0]);
            if (eID < startEID) continue;

            int enemyX = int.Parse(fields[20]);
            int enemyZ = int.Parse(fields[21]);

            UnitStats eStats = ParseEnemyStats(fields, eID, unitType);
            UnitManager enemy = InstantiateEnemyOnGrid(fields, eStats, grid, enemyX, enemyZ);
            mapEnemies.Enqueue(enemy);

            if (eID >= maxEID) break;
        }

        return mapEnemies;

    }
    public IEnumerator SpawnReinforcements(string[] lines, int startEID, int maxEID, GenerateGrid grid, PlayerGridMovement playerGridMovement, FindPath findPath, TurnManager turnManager, Queue<UnitManager> mapEnemies)
    {
        Type unitType = Type.GetType("EnemyStats");
        // v = new Queue<UnitManager>();

        foreach (string line in lines.Skip(1)) // skip header
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] fields = line.Trim().Split(',');

            int eID = int.Parse(fields[0]);
            if (eID <= startEID) continue;

            int enemyX = int.Parse(fields[20]);
            int enemyZ = int.Parse(fields[21]);

            UnitStats eStats = ParseEnemyStats(fields, eID, unitType);

            yield return StartCoroutine(playerGridMovement.MoveCursor(enemyX, enemyZ, 100f));

            UnitManager enemy = InstantiateEnemyOnGrid(fields, eStats, grid, enemyX, enemyZ);
            mapEnemies.Enqueue(enemy);
            turnManager.AddEnemy(enemy);

            if (playerGridMovement.enemyRangeActive)
            {
                findPath.DestroyEnemyRange();
                findPath.EnemyRange();
            }

            if (eID >= maxEID) break;
            yield return new WaitForSeconds(1f);
        }

        yield return null;

    }

    private UnitStats ParseEnemyStats(string[] fields, int eID, Type unitType)
    {

        int index = 23;

        string cName = fields[1];
        string cDesc = fields[2];
        string cType = fields[3];

        int level = int.Parse(fields[4]);
        int HP = int.Parse(fields[5]);
        int ATK = int.Parse(fields[6]);
        int MAG = int.Parse(fields[7]);
        int DEF = int.Parse(fields[8]);
        int RES = int.Parse(fields[9]);
        int SPD = int.Parse(fields[10]);
        int EVA = int.Parse(fields[11]);
        int LUCK = int.Parse(fields[12]);
        int MOVE = int.Parse(fields[13]);

        bool air = bool.Parse(fields[14]);
        bool mount = bool.Parse(fields[15]);
        bool armored = bool.Parse(fields[16]);
        bool whisp = bool.Parse(fields[17]);
        int healthBars = int.Parse(fields[18]);

        bool boss = bool.Parse(fields[29]);


        // Create the UnitStats instance
        UnitStats eStats = (UnitStats)Activator.CreateInstance(
            unitType, eID, cName, cDesc, cType, level, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp, healthBars, boss
        );

        string[] items = new string[6];
        for (int j = 0; j < 6; j++) items[j] = fields[index++];

        foreach (string item in items)
        {
            if (item == "NULL") break;
            Weapon tempWeapon = WeaponManager.MakeWeapon(item);
            eStats.AddWeapon(tempWeapon);
        }

        return eStats;
    }

    private UnitManager InstantiateEnemyOnGrid(string[] fields, UnitStats eStats, GenerateGrid grid, int enemyX, int enemyZ)
    {
        string loadPrefab = fields[19];
        string AIenemy = fields[22];

        //Loads the enemies prefab and instantiates it on the grid tile that is specified in the CSV
        GameObject enemyPrefab = Resources.Load("Enemies/" + loadPrefab) as GameObject;
        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(grid.GetGridTile(enemyX, enemyZ).GetXPos(), grid.GetGridTile(enemyX, enemyZ).GetYPos(), grid.GetGridTile(enemyX, enemyZ).GetZPos()), Quaternion.identity);

        //Ataches an AI script interface depending on the characterististics of the nemy specified in the CSV file
        Type type = Type.GetType(AIenemy);
        IEnemyAI enemyAI = newEnemy.AddComponent(type) as IEnemyAI;

        //Stores the enemy stats in the EnemyUnit object and stores in a queue
        UnitManager enemy = newEnemy.GetComponent<UnitManager>();

        enemy.stats = eStats;
        if (enemy.stats.weapons.Count > 0) enemy.SetPrimaryWeapon(enemy.stats.weapons[0]);

        enemy.InitializeUnitData();
        enemy.XPos = enemyX;
        enemy.ZPos = enemyZ;
        grid.GetGridTile(enemyX, enemyZ).UnitOnTile = enemy;

        return enemy;
    }



}
