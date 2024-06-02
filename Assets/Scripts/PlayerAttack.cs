using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class is still being worked on, please ignore for now
public class PlayerAttack : MonoBehaviour
{
    private GenerateGrid grid;
    private bool[,] canAttack;
    private bool[,] visited;
    private int[,] distances;
    private FindPath pathFinder;
    List<GameObject> movementTiles;
    public GameObject attackTile;
    int sX;
    int sZ;
  
    
    //private GridTile gridCell;



    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        movementTiles = new List<GameObject>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public bool[,] CalculateAttack(int startX, int startZ, int attackRange, bool canAttack1, bool canAttack2, bool canAttack3)
    {
        List<GridTile> cellList = new List<GridTile>();
        List<GridTile> processedList = new List<GridTile>();

        sX = startX;
        sZ = startZ;

        cellList.Add(grid.GetGridTile(startX, startZ));
        processedList.Add(grid.GetGridTile(startX, startZ));

        distances = new int[grid.GetWidth(), grid.GetLength()];
        visited = new bool[grid.GetWidth(), grid.GetLength()];
        canAttack = new bool[grid.GetWidth(), grid.GetLength()];

        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetLength(); j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }

        distances[startX, startZ] = 0;
        canAttack[startX, startZ] = true;

        while (cellList.Count > 0)
        {
            GridTile currentCell = cellList[0];
            cellList.RemoveAt(0);

            int currX = currentCell.GetGridX();
            int currZ = currentCell.GetGridZ();

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

                    if (grid.IsValid(nextX, nextZ))
                    {

                        //If implementing flier class later on change this to check that case
                        int currCost = grid.GetGridTile(nextX, nextZ).GetAttackCost();

                        if (currCost == int.MaxValue)
                        {
                            continue;
                        }

                        int newDistance = distances[currX, currZ] + currCost;



                        if (!visited[nextX, nextZ] && newDistance <= attackRange)
                        {   
                            // if (newDistance == attackRange) {
                            //     canAttack[nextX, nextZ] = true;
                            // } 

                            if (attackRange > 3 && newDistance > 3) {
                                canAttack[nextX, nextZ] = true;
                            }

                            if (canAttack1 && newDistance == 1) {
                                canAttack[nextX, nextZ] = true;
                            }
                            if (canAttack2 && newDistance == 2) {
                                canAttack[nextX, nextZ] = true;
                            }
                            if (canAttack3 && newDistance == 3) {
                                canAttack[nextX, nextZ] = true;
                            }
                            
                            distances[nextX, nextZ] = newDistance;
                            if (!processedList.Contains(grid.GetGridTile(nextX, nextZ)))
                            {
                                cellList.Add(grid.GetGridTile(nextX, nextZ));
                            }


                        }

                    }
                }
            }


        }

        return canAttack;
    }

    public void HighlightAttack(bool[,] canAttack)
    {
        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetLength(); j++)
            {
                if (grid.IsValid(i, j) && canAttack[i, j] && (sX != i || sZ != j))
                {

                    GameObject areaTile;
                    areaTile = Instantiate(attackTile, new Vector3(grid.GetGridTile(i, j).GetXPos(), grid.GetGridTile(i, j).GetYPos() + 0.005f, grid.GetGridTile(i, j).GetZPos()), Quaternion.identity);
                    movementTiles.Add(areaTile);
                }
            }
        }
    }

    public void DestroyRange()
    {
         foreach(GameObject areaT in movementTiles)
        {
            Destroy(areaT);
        }
    }

    //Will return list of enemies in attack range
    public void EnemyList()
    {

    }
}
