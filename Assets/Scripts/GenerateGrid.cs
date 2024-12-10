using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Purpose of this class is to initlize the information for the grid based on the layer that the tile collides with
public class GenerateGrid : MonoBehaviour
{
    private GridTile[,] grid;
    private PlayerGridMovement playerCursor;
    private FindPath pathFinder;
    private float cellSize = 4;
    private int length;
    private int width;
    // [SerializeField] GameObject tilePrefab;               //Will be deleted, final game wont have visible tiles
    // [SerializeField] GameObject impassableTilePrefab;       //Same with this one
    private LayerMask obstacleLayer;                    //Can't pass if grounded unit but can 
    private LayerMask tallObstacleLayer;
    private LayerMask doubleLayer;




    // Start is called before the first frame update
    private void Start()
    {
        playerCursor = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
        obstacleLayer = LayerMask.GetMask("ImpassableTile");
        tallObstacleLayer = LayerMask.GetMask("TallObstacle");
        doubleLayer = LayerMask.GetMask("SlowLayer(2)");
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

    //Generates the map
    private void GenerateMap()
    {
        int tileNum = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                //Creates variables to make code look cleaner
                float xPosition = transform.position.x + (x * cellSize);
                float yPosition = transform.position.y + (cellSize / 2);
                float zPosition = transform.position.z + (z * cellSize);
                Vector3 WorldPos = new Vector3(transform.position.x + (x * cellSize), transform.position.y, transform.position.z + (z * cellSize));

                //if the layer the tile collides with is an obstacle, set movement to int.MaxValue and attack to 1
                //so the units can't move through it but can attack over it
                if (Physics.CheckBox(WorldPos, Vector3.one / 2 * cellSize, Quaternion.identity, obstacleLayer))
                {
                    grid[x, z] = new GridTile(x, z, false, false, int.MaxValue, 1, tileNum, xPosition, yPosition, zPosition);

                    // Instantiate(impassableTilePrefab, WorldPos, Quaternion.identity);
                }
                //If the layer is a tall obstacle, initlize it so that units can't attack nor move through it
                else if (Physics.CheckBox(WorldPos, Vector3.one / 2 * cellSize, Quaternion.identity, tallObstacleLayer)) {
                    grid[x, z] = new GridTile(x, z, false, true, int.MaxValue, int.MaxValue, tileNum, xPosition, yPosition, zPosition);

                    // Instantiate(impassableTilePrefab, WorldPos, Quaternion.identity);
                }
                //If the layer is a slow layer, initlize movement to 2
                else if(Physics.CheckBox(WorldPos, Vector3.one / 2 * cellSize, Quaternion.identity, doubleLayer))
                {
                    grid[x, z] = new GridTile(x, z, true, false, 2, 1, tileNum, xPosition, yPosition, zPosition);

                    // Instantiate(tilePrefab, WorldPos, Quaternion.identity);
                }
                //If it is a normal movement tile, initlize movement and attack to 1
                else
                {
                    grid[x, z] = new GridTile(x, z, true, false, 1, 1, tileNum, xPosition, yPosition, zPosition);

                    // Instantiate(tilePrefab, WorldPos, Quaternion.identity);
                }

                tileNum++;

            }
        }

    }


    //Initlize to where we want the cursor to be for each map
    public void initlizeCursor()
    {
        int x = playerCursor.getX();
        int z = playerCursor.getZ();
        playerCursor.transform.position = new Vector3(grid[x, z].GetXPos(), grid[x, z].GetYPos() + 0.15f, grid[x, z].GetZPos());
        playerCursor.moveCursor.position = new Vector3(grid[x, z].GetXPos(), grid[x, z].GetYPos() + 0.30f, grid[x, z].GetZPos());
        playerCursor.moveCursor.parent = null;
    }

    //Checks to see if a grid position is within the range
    public bool IsValid(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < length)
        {
            return true;
        }
        return false;
    }


    //Moves units to another tile
    public void MoveUnit(UnitManager unitToMove,int orgX,int orgZ,int curX,int curZ) {
        grid[orgX, orgZ].UnitOnTile = null;
        grid[curX, curZ].UnitOnTile = unitToMove;

        unitToMove.XPos = curX;
        unitToMove.ZPos = curZ;

        if (unitToMove.UnitType == "Enemy" && playerCursor.enemyRangeActive) {
            pathFinder.DestroyEnemyRange();
            pathFinder.EnemyRange();
        }
    }

    //returns the width and length of the grid and size of each cell
    public int GetWidth() { return width; }
    public int GetLength() { return length; }
    public float GetCellSize() { return cellSize; }
    public GridTile GetGridTile(int x, int z) { return grid[x,z]; }
    
}



