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
    [SerializeField] GameObject felix; //Remove Later
    [SerializeField] GameObject lilith; //Remove Later

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
