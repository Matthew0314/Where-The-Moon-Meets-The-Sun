using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveAI : MonoBehaviour, IEnemyAI
{
    private GenerateGrid grid;
  
 

    void Start() {
        grid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
    }

    public IEnumerator enemyAttack(GameObject enemy) {
        // Transform objTransform = enemy.transform;
        // Vector3 targetPosition = new Vector3(grid.GetGridTile(4, 3).GetXPos(), grid.GetGridTile(4, 3).GetYPos(), grid.GetGridTile(4, 3).GetZPos());
        // objTransform.position = Vector3.MoveTowards(objTransform.position, targetPosition, 20f * Time.deltaTime);
        Debug.Log("Aggressive");

      

        // Vector3 targetPosition = new Vector3(grid.GetGridTile(9, 3).GetXPos(), grid.GetGridTile(9, 3).GetYPos(), grid.GetGridTile(9, 3).GetZPos());
        // float speed = 20f; // Speed of movement

        // // Move the enemy towards the target position
        // while (Vector3.Distance(enemy.transform.position, targetPosition) > 0.01f)
        // {
        //     // Calculate the step based on speed and deltaTime
        //     float step = speed * Time.deltaTime;

        //     // Move the enemy towards the target position gradually
        //     enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, step);

        //     yield return null; // Wait for the next frame
        // }

        // enemy.transform.position = targetPosition; // Ensure exact position when reached

        // Debug.Log("Enemy reached the target position.");

      
        yield return new WaitForSeconds(3.0f);
       
    }
}
