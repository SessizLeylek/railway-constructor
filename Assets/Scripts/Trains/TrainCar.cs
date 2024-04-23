using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCar : MonoBehaviour
{
    public Transform rearWheels;
    public float carLength;
    public float wheelsWidth;

    //Tweening variables
    Vector3 frontWheelLerpDir;
    Vector3 backWheelLerpDir;
    Vector3 backWheelsPos;

    void Start()
    {

    }

    void Update()
    {
        // Tweening position
        transform.position += frontWheelLerpDir * Time.deltaTime;
        backWheelsPos += backWheelLerpDir * Time.deltaTime;
        transform.forward = (transform.position - backWheelsPos).normalized;
    }

    public void UpdateTrainPosition(Vector3 frontPosition, Vector3 rearPosition)
    {
        // transform.position = desiredFrontPos;
        // backWheelsPos = desiredRearPos;

        frontWheelLerpDir = (frontPosition - transform.position) / Time.fixedDeltaTime;
        backWheelLerpDir = (rearPosition - backWheelsPos) / Time.fixedDeltaTime;
    }
}
