using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCar : MonoBehaviour
{
    public Transform rearWheels;
    float wheelsDistance;

    void Start()
    {
        wheelsDistance = rearWheels.localPosition.magnitude;
    }

    void Update()
    {
        
    }

    public void UpdateTrainPosition(Vector3 frontPosition)
    {
        transform.forward = (frontPosition * 2 - rearWheels.position - transform.position).normalized;
        transform.position = frontPosition;
    }
}
