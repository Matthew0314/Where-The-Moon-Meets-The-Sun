using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float rotationSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //PlayerPhaseCamera();
    }

    private void PlayerPhaseCamera()
    {
        float verticalInput = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, verticalInput * rotationSpeed * Time.deltaTime);
    }
}
