using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores information for each tile on the grid
public class GridTile
{
    private int gridX;
    private int gridZ;
    private bool passable;
    private bool tallObstacle;
    private int movementCost;
    private int attackCost;
    private float xPos;
    private float yPos;
    private float zPos;
    private int tileNum;
    private UnitManager unitOnTile;


    //Constructor
    public GridTile(int x, int z, bool pass, bool tall, int moveCost, int atkCost, int tileNumber, float xPoss, float yPoss, float zPoss)
    {
        gridX = x;
        gridZ = z;
        passable = pass;
        tallObstacle = tall;
        movementCost = moveCost;
        attackCost = atkCost;
        tileNum = tileNumber;
        xPos = xPoss;
        yPos = yPoss;
        zPos = zPoss;
        unitOnTile = null;
    }

    //Returns the values of all variables for this class
    public bool GetPassable() => passable;
    public int GetMovementCost() => movementCost; 
    public int GetAttackCost() => attackCost;
    public float GetXPos() => xPos;
    public float GetYPos() => yPos; 
    public float GetZPos() => zPos;
    public int GetGridX() => gridX;
    public int GetGridZ() => gridZ;
    public bool GetTallObstacle() => tallObstacle;
    public int GetTileNum() => tileNum;

    public Vector3 GetGridPos => new Vector3(xPos, yPos, zPos);
    
    public UnitManager UnitOnTile
    {
        get { return unitOnTile; }
        set { unitOnTile = value; }
    }


}
