using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCameraFollow : MonoBehaviour
{
    public Transform target;
    Vector3 positionDifference;

    void Start()
    {
        positionDifference = transform.position - target.position;
    }

    void Update()
    {
        transform.position = positionDifference + target.position;
    }
}
