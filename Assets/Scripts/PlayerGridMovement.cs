using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Purpose of this class is to provide movement capabilities for the cursors on the grid
public class PlayerGridMovement : MonoBehaviour
{
    public int x = 0;
    public int z = 0;
    public Transform moveCursor;
    [SerializeField] float speed = 20f;
    [SerializeField] LayerMask obstacleLayer;
    private static float cursorSen = .35f;
    private GenerateGrid gridControl;
    private FindPath pathFinder;


    void Start()
    {
        //creates an object reference
        gridControl = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        pathFinder = GameObject.Find("Player").GetComponent<FindPath>();
    }

    // Update is called once per frame
    void Update()
    {
        cursorMovement();
    }



    void cursorMovement()
    {
        //updates "player" position to where "PlayerMove" is on the grid
        transform.position = Vector3.MoveTowards(transform.position, moveCursor.position, speed * Time.deltaTime);


        if (Vector3.Distance(transform.position, moveCursor.position) <= cursorSen)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= cursorSen)
            {
                //Checks if the next tile is not a TallObstacle or if a chracter is not selected
                if (gridControl.IsValid(x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z) && !gridControl.grid[x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z].GetTallObstacle() && !pathFinder.charSelected)
                {
                    moveCursor.position += new Vector3(Mathf.Sign(Input.GetAxis("Horizontal")) * gridControl.GetCellSize(), 0f, 0f);

                    x += (int)Mathf.Sign(Input.GetAxis("Horizontal"));

                    Debug.Log(x + " " + z);
                }
                //If Character is selected, check to see if next tile is whithin the characters range
                else if (gridControl.IsValid(x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z) && pathFinder.charSelected && pathFinder.canMove[x + (int)Mathf.Sign(Input.GetAxis("Horizontal")), z])
                {
                    moveCursor.position += new Vector3(Mathf.Sign(Input.GetAxis("Horizontal")) * gridControl.GetCellSize(), 0f, 0f);

                    x += (int)Mathf.Sign(Input.GetAxis("Horizontal"));

                    Debug.Log(x + " " + z);
                }
            }

            if (Mathf.Abs(Input.GetAxis("Vertical")) >= cursorSen)
            {
                if (gridControl.IsValid(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))) && !gridControl.grid[x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))].GetTallObstacle() && !pathFinder.charSelected)
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(Input.GetAxis("Vertical")) * gridControl.GetCellSize());

                    z += (int)Mathf.Sign(Input.GetAxis("Vertical"));

                    Debug.Log(x + " " + z);
                }
                else if (gridControl.IsValid(x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))) && pathFinder.charSelected && pathFinder.canMove[x, z + (int)Mathf.Sign(Input.GetAxis("Vertical"))])
                {
                    moveCursor.position += new Vector3(0f, 0f, Mathf.Sign(Input.GetAxis("Vertical")) * gridControl.GetCellSize());

                    z += (int)Mathf.Sign(Input.GetAxis("Vertical"));

                    Debug.Log(x + " " + z);
                }

            }
        }
    }
}
