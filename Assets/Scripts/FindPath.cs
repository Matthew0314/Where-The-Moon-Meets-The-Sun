using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPath : MonoBehaviour
{

    public bool[,] canMove;
    public bool[,] canAttack;
    public bool charSelected;
    private int attackRangeStat;
    private GenerateGrid gridTraverse;
    private PlayerGridMovement movementVars;
    private CollideWithPlayerUnit playerCollide;
    private GameObject currUnit;
    private GridTile gridCell;
    [SerializeField] GameObject movementAreaTile;
    public GameObject attackTile;
    private GridTile currTile;
    private bool[,] visited;
    private int[,] distances;
    private PlayerAttack attackPath;
    List<GameObject> movementTiles;
    List<GridTile> movableCells;


    // Start is called before the first frame update
    void Start()
    {
        playerCollide = GameObject.Find("PlayerMove").GetComponent<CollideWithPlayerUnit>();
        gridTraverse = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        movementVars = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        canMove = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        movementTiles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //These are eventually going to be moved to a seperate class
        
        // if (Input.GetKeyDown(KeyCode.Space) && !charSelected && playerCollide.collPlayer)
        // {
        //     ResetArea();
        //     currUnit = playerCollide.currPlayer;
        //     //canMove[movementVars.x, movementVars.z] = true;
        //     attackRangeStat = playerCollide.GetPlayerAttack();
        //     CalcAttack(movementVars.x, movementVars.z, attackRangeStat , playerCollide.GetPlayerMove());
        //     PrintArea();
            
        //     charSelected = true;
        // }
        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     DestroyArea();
        //     charSelected = false;
        // }
        // if (Input.GetKeyDown(KeyCode.Space) && charSelected)
        // {
        //     Debug.Log("Move Unit");
        //     //MoveUnit();
        // }
        // if (Input.GetKeyDown(KeyCode.G)) {
        //     currUnit = playerCollide.currPlayer;
        //     attackRangeStat = playerCollide.GetPlayerAttack();
        //     attackPath.CalculateAttack(movementVars.x, movementVars.z, attackRangeStat);
        //     attackPath.HighlightAttack();
        // }
    }

     public void PrintArea()
     {
        for(int i = 0; i < gridTraverse.GetWidth(); i++)
        {
            for (int j = 0; j < gridTraverse.GetLength(); j++)
            {
                if (canMove[i,j] || canAttack[i,j])
                {

                    GameObject areaTile;
                    if (canAttack[i, j] && !canMove[i, j])
                    {
                        areaTile = Instantiate(attackTile, new Vector3(gridTraverse.grid[i, j].GetXPos(), gridTraverse.grid[i, j].GetYPos() + 0.005f, gridTraverse.grid[i, j].GetZPos()), Quaternion.identity);
                    }
                    else
                    {
                        areaTile = Instantiate(movementAreaTile, new Vector3(gridTraverse.grid[i, j].GetXPos(), gridTraverse.grid[i, j].GetYPos() + 0.005f, gridTraverse.grid[i, j].GetZPos()), Quaternion.identity);
                    }

                    
                    movementTiles.Add(areaTile);
                }
            }
        }
    }

   public void DestroyArea()
    {
        foreach(GameObject areaT in movementTiles)
        {
            Destroy(areaT);
        }
    }

    public void ResetArea()
    {
        canMove = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
    }


    private void CalculateMovement(int startX, int startZ, int charMovement)
    {
        //Creates two lists
        //cellList is used to keep track to cells that need to be processed
        List<GridTile> cellList = new List<GridTile>();
        List<GridTile> processedList = new List<GridTile>();
        //movable cells are the ones on the outer rim that will be used when we calculate the Attack area
        movableCells = new List<GridTile>();

        //Adds the starting cell to both cellList and movableCells
        cellList.Add(gridTraverse.grid[startX, startZ]);
        movableCells.Add(gridTraverse.grid[startX, startZ]);
        processedList.Add(gridTraverse.grid[startX, startZ]);

        //creates arrays that store the distance number and which cells have been visited
        distances = new int[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        visited = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];

        canMove[startX, startZ] = true;

        //Initilizes all distances with the max interger
        for (int i = 0; i < gridTraverse.GetWidth(); i++)
        {
            for (int j = 0; j < gridTraverse.GetLength(); j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }

        distances[startX, startZ] = 0;
        canMove[startX, startZ] = true;

        while (cellList.Count > 0)
        {
            GridTile currentCell = cellList[0];
            cellList.RemoveAt(0);

            int currX = currentCell.GetGridX();
            int currZ = currentCell.GetGridZ();

            if (visited[currX, currZ]) { continue; }

            visited[currX, currZ] = true;

            for (int x = -1; x < 2; x++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if ((x == 0 && z == 0) || (x != 0 && z != 0)) { continue; }

                    int nextX = currX + x;
                    int nextZ = currZ + z;

                    if (gridTraverse.IsValid(nextX, nextZ))
                    {

                        //If implementing flier class later on change this to check that case
                        int currCost = gridTraverse.grid[nextX, nextZ].GetMovementCost();

                        if (currCost == int.MaxValue)
                        {
                            //Since its an edge cell it will be added tp movable cells and used to calculate attack area
                            if (!movableCells.Contains(gridTraverse.grid[currX, currZ]))
                            {
                                movableCells.Add(gridTraverse.grid[currX, currZ]);
                            }
                            continue;
                        }

                        int newDistance = distances[currX, currZ] + currCost;

                        if (!visited[nextX, nextZ] && newDistance <= charMovement)
                        {
                            canMove[nextX, nextZ] = true;
                            distances[nextX, nextZ] = newDistance;
                            if (!processedList.Contains(gridTraverse.grid[nextX, nextZ]))
                            {
                                cellList.Add(gridTraverse.grid[nextX, nextZ]);
                            }
                        }
                        else
                        {
                            if (!movableCells.Contains(gridTraverse.grid[currX, currZ]))
                            {
                                movableCells.Add(gridTraverse.grid[currX, currZ]);
                            }
                        }
                    }
                }
            }
        }     
    }
      
    
    public void CalcAttack(int startX, int startZ, int charAttack, int charMovement)
    {
        CalculateMovement(startX, startZ, charMovement);

        List<GridTile> attackCellList = new List<GridTile>(movableCells);
        List<GridTile> processedCells = new List<GridTile>(movableCells);
        bool[,] newVisited = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        int[,] newDistances = new int[gridTraverse.GetWidth(), gridTraverse.GetLength()];

        for (int i = 0; i < gridTraverse.GetWidth(); i++)
        {
            for (int j = 0; j < gridTraverse.GetLength(); j++)
            {
                newDistances[i, j] = int.MaxValue;
            }
        }

        for (int i = 0; i < attackCellList.Count; i++)
        {
            GridTile initAttack = attackCellList[i];
            newDistances[initAttack.GetGridX(), initAttack.GetGridZ()] = 0;
        }

        while (attackCellList.Count > 0)
        {
            GridTile currentCell = attackCellList[0];
            attackCellList.RemoveAt(0);

            int currX = currentCell.GetGridX();
            int currZ = currentCell.GetGridZ();

            if (newVisited[currX, currZ]) { continue; }

            visited[currX, currZ] = true;
            newVisited[currX, currZ] = true;

            for (int x = -1; x < 2; x++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if ((x == 0 && z == 0) || (x != 0 && z != 0)) { continue; }

                    int nextX = currX + x;
                    int nextZ = currZ + z;

                    if (gridTraverse.IsValid(nextX, nextZ))
                    {
                        int currCost = gridTraverse.grid[nextX, nextZ].GetAttackCost();

                        if (currCost == int.MaxValue) { continue; }

                        int newDistance = newDistances[currX, currZ] + gridTraverse.grid[nextX, nextZ].GetAttackCost(); ;

                        if (!newVisited[nextX, nextZ] && newDistance <= charAttack)
                        {
                            canAttack[nextX, nextZ] = true;

                            if (newDistance < newDistances[nextX, nextZ])
                            {
                                newDistances[nextX, nextZ] = newDistance;
                            }

                            if (!processedCells.Contains(gridTraverse.grid[nextX, nextZ]))
                            {
                                attackCellList.Add(gridTraverse.grid[nextX, nextZ]);
                                processedCells.Add(gridTraverse.grid[nextX, nextZ]);
                            }
                        }
                    }
                }
            }
        }
    }

  

}

//Below is old code, please ignore


/* private void CalculateAttack(int startX, int startZ, int charAttack, int charMovement)
 {
     List<GridTile> cellList = new List<GridTile>();
     cellList.Add(gridTraverse.grid[startX, startZ]);
     charAttack += charMovement;

     int[,] distances = new int[gridTraverse.width, gridTraverse.length];
     visited = new bool[gridTraverse.width, gridTraverse.length];
     bool[,] impassable = new bool[gridTraverse.width, gridTraverse.length];

     for (int i = 0; i < gridTraverse.width; i++)
     {
         for (int j = 0; j < gridTraverse.length; j++)
         {
             distances[i, j] = int.MaxValue;
         }
     }

     distances[startX, startZ] = gridTraverse.grid[startX, startZ].attackCost;


     while (cellList.Count > 0)
     {
         GridTile currentCell = cellList[0];
         cellList.RemoveAt(0);

         int currX = currentCell.gridX;
         int currZ = currentCell.gridZ;

         if (visited[currX, currZ])
         {
             continue;
         }

         visited[currX, currZ] = true;

         for (int x = -1; x < 2; x++)
         {
             for (int z = -1; z < 2; z++)
             {
                 if ((x == 0 && z == 0) || (x != 0 && z != 0))
                 {
                     continue;
                 }

                 int nextX = currX + x;
                 int nextZ = currZ + z;

                 if (gridTraverse.IsValid(nextX, nextZ))
                 {
                     int currCost = gridTraverse.grid[nextX, nextZ].attackCost;

                     if (currCost == int.MaxValue)
                     {
                         continue;
                     }

                     int newDistance = distances[currX, currZ] + currCost;

                     if(!gridTraverse.grid[nextX, nextZ].passable)
                     {

                         newDistance = charMovement;


                     }

                     if (!visited[nextX, nextZ] && newDistance <= charAttack + 1)
                     {
                         canAttack[nextX, nextZ] = true;
                         distances[nextX, nextZ] = newDistance;
                         cellList.Add(gridTraverse.grid[nextX, nextZ]);
                     }
                 }
             }
         }
     }

 }

}*/


/*private void CalcAttack(int startX, int startZ, int charAttack, int charMovement)
{
    CalculateMovement(startX, startZ, charMovement);

    List<GridTile> attackCellList = new List<GridTile>(movableCells);
    List<GridTile> processedCells = new List<GridTile>(movableCells);
    bool[,] newVisited = new bool[gridTraverse.width, gridTraverse.length];



    //int[,] distances = new int[gridTraverse.width, gridTraverse.length];

    /*for (int i = 0; i < gridTraverse.width; i++)
    {
        for (int j = 0; j < gridTraverse.length; j++)
        {
            distances[i, j] = int.MaxValue;
        }
    }

    for (int i = 0; i < attackCellList.Count; i++)
    {
        GridTile initAttack = attackCellList[i];
        distances[initAttack.gridX, initAttack.gridZ] = initAttack.attackCost;
    }*/

//distances[startX, startZ] = gridTraverse.grid[startX, startZ].attackCost;


/*     while (attackCellList.Count > 0)
     {
         GridTile currentCell = attackCellList[0];
         attackCellList.RemoveAt(0);

         int currX = currentCell.gridX;
         int currZ = currentCell.gridZ;

         /*if (newVisited[currX, currZ])
         {
             continue;
         }

         visited[currX, currZ] = true;
         newVisited[currX, currZ] = true;

         for (int x = -1; x < 2; x++)
         {
             for (int z = -1; z < 2; z++)
             {
                 if ((x == 0 && z == 0) || (x != 0 && z != 0))
                 {
                     continue;
                 }

                 int nextX = currX + x;
                 int nextZ = currZ + z;

                 if (gridTraverse.IsValid(nextX, nextZ))
                 {
                     int currCost = gridTraverse.grid[nextX, nextZ].attackCost;

                     if (currCost == int.MaxValue)
                     {
                         continue;
                     }
                     Debug.Log("AttackingCell: " + currX + " " + currZ + " CurrCost: " + currCost);
                     Debug.Log("AttackingCell: " + currX + " " + currZ + " Dist: " + distances[currX, currZ]);
                     int newDistance = distances[currX, currZ] + gridTraverse.grid[nextX, nextZ].attackCost; ;
                     Debug.Log("AttackingCell: " + currX + " " + currZ + " Cost: " + newDistance);


                     if (!visited[nextX, nextZ] && newDistance <= charAttack + charMovement + 1)
                     {
                         canAttack[nextX, nextZ] = true;
                         if (newDistance < distances[nextX, nextZ])
                         {
                             distances[nextX, nextZ] = newDistance;
                         }

                         //attackCellList.Add(gridTraverse.grid[nextX, nextZ]);


                         if (!processedCells.Contains(gridTraverse.grid[nextX, nextZ]))
                         {
                             attackCellList.Add(gridTraverse.grid[nextX, nextZ]);
                             processedCells.Add(gridTraverse.grid[nextX, nextZ]);

                             Debug.Log("AttackCell: " + nextX + " " + nextZ + " added");
                         }

                         //newVisited[nextX, nextZ] = true;
                     }
                 }
             }
         }
     }

 }*/

/*  private void TraverseGrid(int MovementRad, int x, int z)
{
   //Recursive algorithm that will add each traversable cell to an array until movementcost is less than 0
   //Perhaps this algorithm can be improved by using a list/hashmap instead of an array
   if (x - 1 >= 0 && MovementRad - gridTraverse.grid[x - 1,z].movementCost >= 0)
   {
       canMove[x - 1, z] = true;
       if (MovementRad - gridTraverse.grid[x - 1, z].movementCost == 0)
       {
           AttackRange(x - 1, z, attackRangeStat);
       }
       else
       {
           TraverseGrid(MovementRad - gridTraverse.grid[x - 1, z].movementCost, x - 1, z);
       }

   }
   else if (x - 1 >= 0 && !gridTraverse.grid[x-1, z].passable)
   {
       AttackRange(x, z, attackRangeStat);
   }

   if (x + 1 < gridTraverse.width && MovementRad - gridTraverse.grid[x + 1, z].movementCost >= 0)
   {
       canMove[x + 1, z] = true;
       if (MovementRad - gridTraverse.grid[x + 1, z].movementCost == 0)
       {
           AttackRange(x + 1, z, attackRangeStat);
       }
       else
       {
           TraverseGrid(MovementRad - gridTraverse.grid[x + 1, z].movementCost, x + 1, z);
       }

   }
   else if (x + 1 < gridTraverse.width && !gridTraverse.grid[x + 1, z].passable)
   {
       AttackRange(x, z, attackRangeStat);
   }

   if (z - 1 >= 0 && MovementRad - gridTraverse.grid[x, z - 1].movementCost >= 0)
   {
       canMove[x, z - 1] = true;
       if (MovementRad - gridTraverse.grid[x, z - 1].movementCost == 0)
       {
           AttackRange(x, z - 1, attackRangeStat);
       }
       else
       {
           TraverseGrid(MovementRad - gridTraverse.grid[x, z - 1].movementCost, x, z - 1);
       }

   }
   else if (z - 1 >= 0 && !gridTraverse.grid[x, z - 1].passable)
   {
       AttackRange(x, z, attackRangeStat);
   }

   if (z + 1 < gridTraverse.length && MovementRad - gridTraverse.grid[x, z + 1].movementCost >= 0)
   {
       canMove[x, z + 1] = true;
       if (MovementRad - gridTraverse.grid[x, z + 1].movementCost == 0)
       {
           AttackRange(x, z + 1, attackRangeStat);
       }
       else
       {
           TraverseGrid(MovementRad - gridTraverse.grid[x, z + 1].movementCost, x, z + 1);
       } 
   }
   else if (z + 1 < gridTraverse.length && !gridTraverse.grid[x, z + 1].passable)
   {
       AttackRange(x, z, attackRangeStat);
   }
}


public void AttackRange(int x, int z, int attackRangeST)
{

   attackRangeST -= 1;

   if (x - 1 >= 0 && !gridTraverse.grid[x - 1, z].tallObstacle && attackRangeST >= 0)
   {
       if (gridTraverse.grid[x - 1, z].passable)
       {
           attackRange[x - 1, z] = true;
       }

       AttackRange(x - 1, z, attackRangeST);
   }

   if (x + 1 < gridTraverse.width && !gridTraverse.grid[x + 1, z].tallObstacle && attackRangeST >= 0)
   {
       if (gridTraverse.grid[x + 1, z].passable)
       {
           attackRange[x + 1, z] = true;
       }

       AttackRange(x + 1, z, attackRangeST);
   }

   if (z - 1 >= 0 && !gridTraverse.grid[x, z - 1].tallObstacle && attackRangeST >= 0)
   {
       if (gridTraverse.grid[x, z - 1].passable)
       {
           attackRange[x, z - 1] = true;
       }

       AttackRange(x, z - 1, attackRangeST);
   }

   if (z + 1 < gridTraverse.length && !gridTraverse.grid[x, z + 1].tallObstacle && attackRangeST >= 0)
   {
       if (gridTraverse.grid[x, z + 1].passable)
       {
           attackRange[x, z + 1] = true;
       }
       AttackRange(x, z + 1, attackRangeST);
   }
}*/
