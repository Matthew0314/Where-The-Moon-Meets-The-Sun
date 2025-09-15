using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Purpose of this class is to initlize the information for the grid based on the layer that the tile collides with
public class GenerateGrid : MonoBehaviour
{
    private GridTile[,] grid;
    private const float cellSize = 4; // Size of each grid cell, keep at 4
    private int length;
    private int width;
    private PlayerGridMovement playerGridMovement;
    private FindPath findPath;
  
    private void Awake()
    {
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        findPath = GameObject.Find("Player").GetComponent<FindPath>();
    }

    //Generates grid using parameters from the map data
    public void GenGrid(int len, int wid)
    {  
        length = len;
        width = wid;
        grid = new GridTile[width, length];
        GenerateMap();
        initlizeCursor();
    }

    
    private void GenerateMap()
    {
        int tileNum = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                float xPosition = transform.position.x + (x * cellSize);
                float yPosition = transform.position.y + (cellSize / 2);
                float zPosition = transform.position.z + (z * cellSize);
                Vector3 WorldPos = new Vector3(xPosition, transform.position.y, zPosition);


                // Use later if you have ground that is not flat
                // // Raycast down to find ground height
                // Vector3 rayOrigin = new Vector3(xPosition, transform.position.y + 10f, zPosition);
                // if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 20f))
                // {
                //     if (hit.collider.CompareTag("Ground"))
                //     {
                //         yPosition = hit.point.y + (cellSize / 2); // Adjust so tile centers above ground
                //     }
                // }

                Collider[] hitColliders = Physics.OverlapBox(WorldPos, Vector3.one / 2 * cellSize, Quaternion.identity);
                bool isSet = false;

                foreach (Collider col in hitColliders)
                {
                    if (col.CompareTag("TallObstacle"))
                    {
                        grid[x, z] = new GridTile(x, z, false, true, int.MaxValue, int.MaxValue, tileNum, xPosition, yPosition, zPosition);
                        isSet = true;
                        break;
                    }
                    else if (col.CompareTag("Obstacle"))
                    {
                        grid[x, z] = new GridTile(x, z, false, false, int.MaxValue, 1, tileNum, xPosition, yPosition, zPosition);
                        isSet = true;
                        break;
                    }
                    else if (col.CompareTag("Slow"))
                    {
                        grid[x, z] = new GridTile(x, z, true, false, 2, 1, tileNum, xPosition, yPosition, zPosition);
                        isSet = true;
                        break;
                    }
                }

                if (!isSet)
                {
                    grid[x, z] = new GridTile(x, z, true, false, 1, 1, tileNum, xPosition, yPosition, zPosition);
                }

                tileNum++;
            }
        }
    }

    //Initlize to where we want the cursor to be for each map
    private void initlizeCursor()
    {
        int x = playerGridMovement.getX();
        int z = playerGridMovement.getZ();
        playerGridMovement.transform.position = new Vector3(grid[x, z].GetXPos(), grid[x, z].GetYPos() + 0.15f, grid[x, z].GetZPos());

        Transform cursor = GameObject.Find("PlayerMove").transform;
        cursor.position = new Vector3(grid[x, z].GetXPos(), grid[x, z].GetYPos() + 0.30f, grid[x, z].GetZPos());
        cursor.parent = null;
    }

    //Checks to see if a grid position is within the range
    public bool IsValid(int x, int z) => x >= 0 && x < width && z >= 0 && z < length;

    //Moves units to another tile
    public void MoveUnit(UnitManager unitToMove,int orgX,int orgZ,int curX,int curZ) {

        if (IsValid(orgX, orgZ)) grid[orgX, orgZ].UnitOnTile = null;
        
        grid[curX, curZ].UnitOnTile = unitToMove;

        unitToMove.XPos = curX;
        unitToMove.ZPos = curZ;

        if (unitToMove.UnitType == "Enemy" && playerGridMovement.enemyRangeActive) {
            findPath.DestroyEnemyRange();
            findPath.EnemyRange();
        }
    }

    //returns the width and length of the grid and size of each cell
    public int GetWidth() => width; 
    public int GetLength() => length; 
    public float GetCellSize() => cellSize; 
    public GridTile GetGridTile(int x, int z) => grid[x,z]; 
    
}



