using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //private bool tileOccupied; //Set up Getter and Setters later
    //private string tileType; //Set up Getter and Setters later



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

        //tileOccupied = false;
        //tileType = "Hello World";

    }

    //Only have getters because these variables are not meant to be changed
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

}
