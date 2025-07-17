using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;

public class PrologueMap : MapManager
{
    string[] newUnits = { "YoungLilith", "Janine", "Felix", "YoungFelix", "Meyneth", "Mia", "Noah" };
    private bool calledReinforcements = false;


    // protected override void Awake()
    // {
    //     // length = 16;
    //     // width = 24;
    //     // unitStartNum = 2;

    //     // winCondition = "Defeat the boss.";
    //     // loseCondition = "<color=#3160BC>Felix</color> or <color=#3160BC>Lilith</color> falls in battle.";

    //     // maxEIDNormal = 5;
    //     // maxEIDHard = 5;
    //     // maxEIDEclipse = 8;

    //     // requiredUnits.Add("YoungFelix");

    //     // primaryStart = new Vector2Int(9, 2);

    //     base.Awake();

    // }

    protected override void Start()
    {
        //Reads in data for weapons, all units in the game, and all player classes
        // ! THESE WILL NEVER BE CALLED AGAIN AFTER THE PROLOGUE MAP
        // ! This needs to be moved to another class
        manageWeapons.ReadCSV();
        classRos.Init();
        unitRos.ReadCSV();

        //If there are new players being added to the players roster, this will be called
        // for (int i = 0; i < 10; i++)
        // {
        AddNewPlayers(newUnits);
            
        // }

        //Initilizes the prologue map
        // base.Init();
        Init();
        // InitStartTiles();
    }

    // public override Vector2Int[] GetPlayerStartPositions()
    // {
    //     return new Vector2Int[] {
    //         new Vector2Int(9, 2),
    //         new Vector2Int(10, 1),
    //         new Vector2Int(11, 2),
    //         new Vector2Int(12, 1)
    //     };
    // }

   
    //The clear condition for the prologue is routing all the enemies 
    public override IEnumerator CheckClearCondition()
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
    public override IEnumerator CheckDefeatCondition()
    {
        yield return StartCoroutine(MissingUnitsDefeat(new List<string> { "YoungFelix", "YoungLilith" }));
    }

    public override IEnumerator CheckEvents() {
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



    

    // Co routine to start the map
    protected override IEnumerator StartMap() {
        yield return StartCoroutine(battleStartMenu.StartMenu());
        yield return StartCoroutine(StandardShowBossStartMap(21, 13));
    }

}
