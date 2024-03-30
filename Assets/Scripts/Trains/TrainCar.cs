using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCar : MonoBehaviour
{
    public Transform rearWheels;
    public float carLength;
    float wheelsDistance;

    //Tweening variables
    Vector3 prevPosition;
    Vector3 prevForward;
    Vector3 desiredPosition;
    Vector3 desiredForward;

    void Start()
    {
        wheelsDistance = rearWheels.localPosition.magnitude;
    }

    void Update()
    {
        // Tweening position
        transform.position += (desiredPosition - prevPosition) * Time.deltaTime / Time.fixedDeltaTime;
        transform.forward += (desiredForward - prevForward) * Time.deltaTime / Time.fixedDeltaTime;
    }

    public void UpdateTrainPosition(Vector3 frontPosition)
    {
        prevPosition = desiredPosition;
        prevForward = desiredForward;

        desiredForward = (frontPosition * 2 - rearWheels.position - desiredPosition).normalized;
        desiredPosition = frontPosition;
    }
}
