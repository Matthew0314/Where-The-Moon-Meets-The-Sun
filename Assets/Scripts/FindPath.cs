using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PathTile {

    public int x;
    public int z;

    public float gValue;
    public float hValue;

    public PathTile parentTile;

    public float fValue {
        get{ return gValue + hValue; }
    }

    public PathTile(int xPos, int zPos) {
        x = xPos;
        z = zPos;
    }
}

public class FindPath : MonoBehaviour
{

    public bool[,] canMove;
    public bool[,] canAttack;
    public bool[,] canOnlyAttack;
    public bool charSelected;
    private int attackRangeStat;
    private GenerateGrid gridTraverse;
    private PlayerGridMovement movementVars;
    private MapManager _currentMap;
    private GameObject currUnit;
    private GridTile gridCell;
    [SerializeField] GameObject movementAreaTile;
    public GameObject moveTile;
    public GameObject attackTile;
    public GameObject enemyTile;
    [SerializeField] GameObject specificEnemyTile;
    private GridTile currTile;
    private bool[,] visited;
    private int[,] distances;
    private int[,] moveDistances;
    private int[,] attackDistances;
    private bool[,] moveVisited;
    private bool[,] attackVisited;
    private bool[,] enemyRange;
    private bool[,] specEnemyRange;

    int sX;
    int sZ;


    List<GameObject> movementTiles;
    List<GameObject> attackTiles;
    List<GameObject> enemyTiles;
    List<GameObject> specEnemyTiles;
    List<GridTile> movableCells;

    public List<UnitManager> selectedEnemies;

    PathTile[,] pathTiles;


    // Start is called before the first frame update
    void Start()
    {
        gridTraverse = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        movementVars = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        _currentMap = GameObject.Find("GridManager").GetComponent<MapManager>();

        canMove = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];
        canAttack = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];

        movementTiles = new List<GameObject>();
        attackTiles = new List<GameObject>();
        enemyTiles = new List<GameObject>();
        specEnemyTiles = new List<GameObject>();

        selectedEnemies = new List<UnitManager>();

        InitPathGrid();
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
                        areaTile = Instantiate(attackTile, new Vector3(gridTraverse.GetGridTile(i, j).GetXPos(), gridTraverse.GetGridTile(i, j).GetYPos() + 0.005f, gridTraverse.GetGridTile(i, j).GetZPos()), Quaternion.identity);
                    }
                    else
                    {
                        areaTile = Instantiate(movementAreaTile, new Vector3(gridTraverse.GetGridTile(i, j).GetXPos(), gridTraverse.GetGridTile(i, j).GetYPos() + 0.005f, gridTraverse.GetGridTile(i, j).GetZPos()), Quaternion.identity);
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

    public void calculateMovement(int startX, int startZ, int charMovement, UnitManager unit)
    {
        //Creates two lists
        //cellList is used to keep track to cells that need to be processed
        List<GridTile> cellList = new List<GridTile>();
        List<GridTile> processedList = new List<GridTile>();
        //movable cells are the ones on the outer rim that will be used when we calculate the Attack area
        // movableCells = new List<GridTile>();

        bool[,] discard;

        //Adds the starting cell to both cellList and movableCells
        cellList.Add(gridTraverse.GetGridTile(startX, startZ));
        // movableCells.Add(gridTraverse.GetGridTile(startX, startZ));
        processedList.Add(gridTraverse.GetGridTile(startX, startZ));

        //creates arrays that store the distance number and which cells have been visited
        moveDistances = new int[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        moveVisited = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canMove = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        Debug.Log(startX + " " + startZ);
        canMove[startX, startZ] = true;

        //Initilizes all distances with the max interger
        for (int i = 0; i < gridTraverse.GetWidth(); i++)
        {
            for (int j = 0; j < gridTraverse.GetLength(); j++)
            {
                moveDistances[i, j] = int.MaxValue;
            }
        }

        moveDistances[startX, startZ] = 0;
        canMove[startX, startZ] = true;

        while (cellList.Count > 0)
        {
            GridTile currentCell = cellList[0];
            cellList.RemoveAt(0);

            int currX = currentCell.GetGridX();
            int currZ = currentCell.GetGridZ();

            if (moveVisited[currX, currZ]) { continue; }

            moveVisited[currX, currZ] = true;
            if (unit.GetPrimaryWeapon() != null)
            {
                discard = CalculateAttack(currX, currZ, unit.GetPrimaryWeapon().Range, unit.GetPrimaryWeapon().Range1, unit.GetPrimaryWeapon().Range2, unit.GetPrimaryWeapon().Range3);
            }
            else
            {
                discard = CalculateAttack(currX, currZ, 0, false, false, false);
            }


            for (int x = -1; x < 2; x++)
            {
                for (int z = -1; z < 2; z++)
                {
                    // Skips Diagonal tiles
                    if ((x == 0 && z == 0) || (x != 0 && z != 0)) { continue; }


                    // Get the x and z coordinates of the next tile
                    int nextX = currX + x;
                    int nextZ = currZ + z;

                    // Checks if in range
                    if (!gridTraverse.IsValid(nextX, nextZ)) continue;

                    //If implementing flier class later on change this to check that case
                    int currCost = gridTraverse.GetGridTile(nextX, nextZ).GetMovementCost();

                    // Debug.Log(gridTraverse.GetGridTile(nextX, nextZ).UnitOnTile.currentHealth );


                    if (currCost == int.MaxValue || ((gridTraverse.GetGridTile(nextX, nextZ).UnitOnTile != null && gridTraverse.GetGridTile(nextX, nextZ).UnitOnTile.GetUnitType() != unit.GetUnitType())))
                    {
                        //Since its an edge cell it will be added tp movable cells and used to calculate attack area
                        // discard = CalculateAttack(startX, startZ, unit.primaryWeapon.Range, unit.primaryWeapon.Range1, unit.primaryWeapon.Range2, unit.primaryWeapon.Range3);

                        continue;
                    }

                    int newDistance = moveDistances[currX, currZ] + currCost;

                    if ((!moveVisited[nextX, nextZ] && newDistance <= charMovement) || (moveVisited[nextX, nextZ] && newDistance <= charMovement && newDistance < moveDistances[nextX, nextZ]))
                    {
                        canMove[nextX, nextZ] = true;
                        moveDistances[nextX, nextZ] = newDistance;
                        if (!processedList.Contains(gridTraverse.GetGridTile(nextX, nextZ)))
                        {
                            cellList.Add(gridTraverse.GetGridTile(nextX, nextZ));
                        }
                    }


                }
            }
        }

        string output = "";
        int rows = moveDistances.GetLength(0);
        int cols = moveDistances.GetLength(1);

        for (int y = cols - 1; y >= 0; y--) // Columns become rows (reversed)
        {
            output += "[ ";
            for (int x = 0; x < rows; x++) // Rows become columns
            {
                int value = moveDistances[x, y];
                output += (value == int.MaxValue ? "-" : value.ToString()) +
                        (x < rows - 1 ? ", " : "");
            }
            output += " ]\n";
        }
        Debug.Log(output);

    }


    public bool[,] CalculateAttack(int startX, int startZ, int attackRange, bool canAttack1, bool canAttack2, bool canAttack3)
    {
        List<GridTile> cellList = new List<GridTile>();
        List<GridTile> processedList = new List<GridTile>();

        sX = startX;
        sZ = startZ;

        cellList.Add(gridTraverse.GetGridTile(startX, startZ));
        processedList.Add(gridTraverse.GetGridTile(startX, startZ));

        attackDistances = new int[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        attackVisited = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canOnlyAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];


        for (int i = 0; i < gridTraverse.GetWidth(); i++)
        {
            for (int j = 0; j < gridTraverse.GetLength(); j++)
            {
                attackDistances[i, j] = int.MaxValue;
            }
        }

        attackDistances[startX, startZ] = 0;
        canAttack[startX, startZ] = true;

        while (cellList.Count > 0)
        {
            GridTile currentCell = cellList[0];
            cellList.RemoveAt(0);

            int currX = currentCell.GetGridX();
            int currZ = currentCell.GetGridZ();

            if (attackVisited[currX, currZ])
            {
                continue;
            }

            attackVisited[currX, currZ] = true;

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

                        //If implementing flier class later on change this to check that case
                        int currCost = gridTraverse.GetGridTile(nextX, nextZ).GetAttackCost();

                        if (currCost == int.MaxValue)
                        {
                            continue;
                        }

                        int newDistance = attackDistances[currX, currZ] + currCost;



                        if (!attackVisited[nextX, nextZ] && newDistance <= attackRange)
                        {
                            // if (newDistance == attackRange) {
                            //     canAttack[nextX, nextZ] = true;
                            // } 

                            if (attackRange > 3 && newDistance > 3)
                            {
                                canAttack[nextX, nextZ] = true;
                                canOnlyAttack[nextX, nextZ] = true;
                            }

                            if (canAttack1 && newDistance == 1)
                            {
                                canAttack[nextX, nextZ] = true;
                                canOnlyAttack[nextX, nextZ] = true;

                            }
                            if (canAttack2 && newDistance == 2)
                            {
                                canAttack[nextX, nextZ] = true;
                                canOnlyAttack[nextX, nextZ] = true;
                            }
                            if (canAttack3 && newDistance == 3)
                            {
                                canAttack[nextX, nextZ] = true;
                                canOnlyAttack[nextX, nextZ] = true;
                            }

                            attackDistances[nextX, nextZ] = newDistance;
                            if (!processedList.Contains(gridTraverse.GetGridTile(nextX, nextZ)))
                            {
                                cellList.Add(gridTraverse.GetGridTile(nextX, nextZ));
                            }


                        }

                    }
                }
            }


        }

        return canOnlyAttack;
    }





    public void HighlightAttack(bool[,] canAttack)
    {
        for (int i = 0; i < gridTraverse.GetWidth(); i++)
        {
            for (int j = 0; j < gridTraverse.GetLength(); j++)
            {
                if (gridTraverse.IsValid(i, j) && canAttack[i, j] && (sX != i || sZ != j))
                {

                    GameObject areaTile;
                    areaTile = Instantiate(attackTile, new Vector3(gridTraverse.GetGridTile(i, j).GetXPos(), gridTraverse.GetGridTile(i, j).GetYPos() + 0.005f, gridTraverse.GetGridTile(i, j).GetZPos()), Quaternion.identity);
                    attackTiles.Add(areaTile);
                }
            }
        }
    }

    public void DestroyRange()
    {
         foreach(GameObject areaT in attackTiles)
        {
            Destroy(areaT);
        }
    }


    // Initializes enemy range tiles for the player to see
    public void EnemyRange() {
        bool [,] moveTemp = canMove;
        bool [,] attackTemp = canAttack;
        enemyRange = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canMove = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];

        Queue<UnitManager> currEnemies = new Queue<UnitManager>();

        Queue<UnitManager> temp = new Queue<UnitManager>(
            (_currentMap.GetMapEnemies1() ?? Enumerable.Empty<UnitManager>())
                .Concat(_currentMap.GetMapEnemies2() ?? Enumerable.Empty<UnitManager>())
        );



        foreach(UnitManager element in temp) {
            currEnemies.Enqueue(element);
        }

        int queueCount = currEnemies.Count;



        for (int k = 0; k < queueCount; k++) {
            UnitManager tempEne = currEnemies.Dequeue();
            calculateMovement(tempEne.XPos, tempEne.ZPos, tempEne.GetMove(), tempEne);

            for (int i = 0; i < gridTraverse.GetWidth(); i++)
            {
                for (int j = 0; j < gridTraverse.GetLength(); j++)
                {
                    if (gridTraverse.IsValid(i, j) && (canAttack[i, j] || canMove[i,j]) && !enemyRange[i,j])
                    {
                        enemyRange[i,j] = true;
                        GameObject areaTile;
                        areaTile = Instantiate(enemyTile, new Vector3(gridTraverse.GetGridTile(i, j).GetXPos(), gridTraverse.GetGridTile(i, j).GetYPos() + 0.003f, gridTraverse.GetGridTile(i, j).GetZPos()), Quaternion.identity);
                        enemyTiles.Add(areaTile);
                    }
                }
            }
        }
        canAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canMove = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()]; 

        canMove = moveTemp;
        canAttack = attackTemp;

        
    }

    //Destroys all enemy range tiles
    public void DestroyEnemyRange()
    {
        foreach(GameObject areaT in enemyTiles)
        {
            Destroy(areaT);
        }
    }

    //Initializes a specific enemy range for the player to see
    public void SpecificEnemyRange(UnitManager enemy) {
        bool [,] moveTemp = canMove;
        bool [,] attackTemp = canAttack;
        specEnemyRange = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];
        canAttack = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];
        canMove = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];



    
          
        calculateMovement(enemy.XPos, enemy.ZPos, enemy.GetMove(), enemy);

        for (int i = 0; i < gridTraverse.GetWidth(); i++)
        {
            for (int j = 0; j < gridTraverse.GetLength(); j++)
            {
                if (gridTraverse.IsValid(i, j) && (canAttack[i, j] || canMove[i,j]) && !specEnemyRange[i,j])
                {
                    specEnemyRange[i,j] = true;
                    GameObject areaTile;
                    areaTile = Instantiate(specificEnemyTile, new Vector3(gridTraverse.GetGridTile(i, j).GetXPos(), gridTraverse.GetGridTile(i, j).GetYPos() + 0.004f, gridTraverse.GetGridTile(i, j).GetZPos()), Quaternion.identity);
                    specEnemyTiles.Add(areaTile);
                }
            }
        }
        
        canAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canMove = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()]; 

        canMove = moveTemp;
        canAttack = attackTemp;

        selectedEnemies.Add(enemy);

        
    }

    // Unselect an enemy in the case that they die or have moved
    public void UnSelectEnemies(UnitManager enemy) {

        foreach(GameObject areaT in specEnemyTiles)
        {
            Destroy(areaT);
        }  

        bool [,] moveTemp = canMove;
        bool [,] attackTemp = canAttack;
        specEnemyRange = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];
        canAttack = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];
        canMove = new bool[_currentMap.GetWidth(), _currentMap.GetLength()];

        if (selectedEnemies.Contains(enemy)) {
            selectedEnemies.Remove(enemy);
        }
        
        int eneCou = selectedEnemies.Count;

        if (eneCou == 0) {return;}

        for (int k = 0; k < eneCou; k++) {
            UnitManager tempEne = selectedEnemies[k];
            calculateMovement(tempEne.XPos, tempEne.ZPos, tempEne.GetMove(), tempEne);

            for (int i = 0; i < gridTraverse.GetWidth(); i++)
            {
                for (int j = 0; j < gridTraverse.GetLength(); j++)
                {
                    if (gridTraverse.IsValid(i, j) && (canAttack[i, j] || canMove[i,j]) && !specEnemyRange[i,j])
                    {
                        specEnemyRange[i,j] = true;
                        GameObject areaTile;
                        areaTile = Instantiate(specificEnemyTile, new Vector3(gridTraverse.GetGridTile(i, j).GetXPos(), gridTraverse.GetGridTile(i, j).GetYPos() + 0.004f, gridTraverse.GetGridTile(i, j).GetZPos()), Quaternion.identity);
                        specEnemyTiles.Add(areaTile);
                    }
                }
            }
        }
        canAttack = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()];
        canMove = new bool[gridTraverse.GetWidth(), gridTraverse.GetLength()]; 

        canMove = moveTemp;
        canAttack = attackTemp;
    }



   // Next four methods are used to find the shortest path from one tile to another
    private void InitPathGrid() {
        Debug.LogWarning(_currentMap.GetWidth() + " " + _currentMap.GetLength());

        pathTiles = new PathTile[_currentMap.GetWidth(), _currentMap.GetLength()];
        
        for (int x = 0; x < _currentMap.GetWidth(); x++) {
            for (int y = 0; y < _currentMap.GetLength(); y++) { 
                pathTiles[x,y] = new PathTile(x,y);
            }
        }
    }

    
    public List<PathTile> FindShortestPath(int startX, int startZ, int endX, int endZ) {

        PathTile startTile = pathTiles[startX, startZ];
        PathTile endTile = pathTiles[endX, endZ];

        List<PathTile> openList = new List<PathTile>();
        List<PathTile> closedList = new List<PathTile>();

        openList.Add(startTile);

        while (openList.Count > 0) {
            PathTile currentTile = openList[0];

            for (int i = 0; i < openList.Count; i++) {
                if (currentTile.fValue > openList[i].fValue) {
                    currentTile = openList[i];
                }

                if (currentTile.fValue == openList[i].fValue && currentTile.hValue > openList[i].hValue) {
                    currentTile = openList[i];
                }
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == endTile) {
                return RetracePath(startTile, endTile);
            }

            List<PathTile> neighborTiles = new List<PathTile>();
            for (int x = -1; x < 2; x++) {
                for (int z = -1; z < 2; z++) {
                    if ((x == 0 && z == 0) || (x != 0 && z != 0)) {continue;}
                    if (!gridTraverse.IsValid(currentTile.x + x, currentTile.z + z)) { continue; } //?
                    
                    neighborTiles.Add(pathTiles[currentTile.x + x, currentTile.z + z]);
                }
            }

            for (int i = 0; i < neighborTiles.Count; i++) {
                if (closedList.Contains(neighborTiles[i])) { continue; }
                if (!gridTraverse.GetGridTile(neighborTiles[i].x, neighborTiles[i].z).GetPassable()) { continue; }

                float moveCost = currentTile.gValue + CalculateDistance(currentTile , neighborTiles[i]);

                if (!openList.Contains(neighborTiles[i]) || moveCost < neighborTiles[i].gValue) {
                    neighborTiles[i].gValue = moveCost;
                    neighborTiles[i].hValue = CalculateDistance(neighborTiles[i], endTile);
                    neighborTiles[i].parentTile = currentTile;

                    if(!openList.Contains(neighborTiles[i])) {
                        openList.Add(neighborTiles[i]);
                    }
                }
            }
        }

        return null;
    }


    private int CalculateDistance(PathTile currentTile, PathTile targetTile) {
        int distX = Mathf.Abs(currentTile.x - targetTile.x);
        int distZ = Mathf.Abs(currentTile.z - targetTile.z);

        if (distX > distZ) { return 14 * distZ + 10 * (distX - distZ); }
        return 14 * distX + 10 * (distZ - distX);
    }

    private List<PathTile> RetracePath(PathTile startTile, PathTile endTile) {
        List<PathTile> path = new List<PathTile>();

        PathTile currentTile = endTile;

        while (currentTile != startTile) {
            path.Add(currentTile);
            currentTile = currentTile.parentTile;
        }
        path.Reverse();
        return path;
    }









}
