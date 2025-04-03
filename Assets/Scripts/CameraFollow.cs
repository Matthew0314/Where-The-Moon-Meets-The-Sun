using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Camera follows the cursor
public class CameraFollow : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] Transform target; 
    [SerializeField] Vector3 offset; 

    void Start() {
        if (offset == Vector3.zero) offset = transform.position - target.position;
    }

    void LateUpdate() {
        transform.position = target.position + offset;
    }

    private void PlayerPhaseCamera() {
        float verticalInput = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, verticalInput * rotationSpeed * Time.deltaTime);
    }
}
