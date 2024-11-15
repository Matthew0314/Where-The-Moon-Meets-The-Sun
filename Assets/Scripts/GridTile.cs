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
    public bool GetPassable() { return passable; }
    public int GetMovementCost() { return movementCost; }
    public int GetAttackCost() { return attackCost; }
    public float GetXPos() { return xPos; }
    public float GetYPos() { return yPos; }
    public float GetZPos() { return zPos; }
    public int GetGridX() { return gridX; }
    public int GetGridZ() { return gridZ; }
    public bool GetTallObstacle() { return tallObstacle; }
    public int GetTileNum() { return tileNum; }
    
    public UnitManager UnitOnTile {
        get { return unitOnTile; }
        set { unitOnTile = value; }
    }


}
