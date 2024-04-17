using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int cameraMoveSpeed = 1;
    readonly int screenWidth = Screen.width;
    readonly int screenHeight = Screen.height;

    void Start()
    {
        
    }

    void Update()
    {
        // Moving the camera with mouse
        if(Input.mousePosition.x < screenWidth * 0.02f)
        {
            // Moving camera to the left
            Vector3 moveAxis = -transform.right;
            moveAxis.y = 0;
            transform.position += moveAxis.normalized * cameraMoveSpeed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 3 : 1);
        }
        else if (Input.mousePosition.x > screenWidth * 0.98f)
        {
            // Moving camera to the left
            Vector3 moveAxis = transform.right;
            moveAxis.y = 0;
            transform.position += moveAxis.normalized * cameraMoveSpeed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 3 : 1);
        }

        if (Input.mousePosition.y < screenHeight * 0.02f)
        {
            // Moving camera to the left
            Vector3 moveAxis = -transform.forward;
            moveAxis.y = 0;
            transform.position += moveAxis.normalized * cameraMoveSpeed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 3 : 1);
        }
        else if (Input.mousePosition.y > screenHeight * 0.98f)
        {
            // Moving camera to the left
            Vector3 moveAxis = transform.forward;
            moveAxis.y = 0;
            transform.position += moveAxis.normalized * cameraMoveSpeed * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 3 : 1);
        }
    }
}
