using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class Has not been implemented yet please ignore
public class CameraFollow : MonoBehaviour
{
    public float rotationSpeed = 10f;
     public Transform target; 
    public Vector3 offset; 
    // Start is called before the first frame update
    void Start()
    {
         if (offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // PlayerPhaseCamera();
        transform.position = target.position + offset;
    }

    private void PlayerPhaseCamera()
    {
        float verticalInput = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, verticalInput * rotationSpeed * Time.deltaTime);
    }
}

//Note

/*
For enemy phase, the camera will have to change its target to each enemy, going to have to use IEnumerator for a slow transition
and then after its either have the cursor move to where the gamera is or the camera back to player 1 along with the cursor
*/
