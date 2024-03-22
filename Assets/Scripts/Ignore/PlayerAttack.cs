using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class is still being worked on, please ignore for now
public class PlayerAttack : MonoBehaviour
{
    private GenerateGrid grid;
    public bool[,] canAttack;
    private bool[,] visited;
    private int[,] distances;
    private FindPath pathFinder;
    List<GameObject> movementTiles;
    //private GridTile gridCell;



    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        canAttack = new bool[grid.GetWidth(), grid.GetLength()];
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public void CalculateAttack(int startX, int startZ, int attackRange)
    {
        List<GridTile> cellList = new List<GridTile>();
        List<GridTile> processedList = new List<GridTile>();

        cellList.Add(grid.grid[startX, startZ]);
        processedList.Add(grid.grid[startX, startZ]);

        distances = new int[grid.GetWidth(), grid.GetLength()];
        visited = new bool[grid.GetWidth(), grid.GetLength()];

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
                        int currCost = grid.grid[nextX, nextZ].GetAttackCost();

                        if (currCost == int.MaxValue)
                        {
                            continue;
                        }

                        int newDistance = distances[currX, currZ] + currCost;



                        if (!visited[nextX, nextZ] && newDistance <= attackRange)
                        {
                            canAttack[nextX, nextZ] = true;
                            distances[nextX, nextZ] = newDistance;
                            if (!processedList.Contains(grid.grid[nextX, nextZ]))
                            {
                                cellList.Add(grid.grid[nextX, nextZ]);
                            }


                        }

                    }
                }
            }


        }
    }

    public void HighlightAttack()
    {
        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetLength(); j++)
            {
                if (canAttack[i, j])
                {

                    GameObject areaTile;
                    areaTile = Instantiate(pathFinder.attackTile, new Vector3(grid.grid[i, j].GetXPos(), grid.grid[i, j].GetYPos() + 0.005f, grid.grid[i, j].GetZPos()), Quaternion.identity);
                    movementTiles.Add(areaTile);
                }
            }
        }
    }

    public void DestroyRange()
    {

    }

    //Will return list of enemies in attack range
    public void EnemyList()
    {

    }
}
