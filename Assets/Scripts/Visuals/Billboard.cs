using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    //A Quad looking at camera 
    public Vector3 upDirection = Vector3.up;

    Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position, upDirection);
    }
}
