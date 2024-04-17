using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCar : MonoBehaviour
{
    public Transform rearWheels;
    public float carLength;
    public float wheelsWidth;

    //Tweening variables
    Vector3 prevFrontPos;
    Vector3 prevRearPos;
    Vector3 desiredFrontPos;
    Vector3 desiredRearPos;
    Vector3 backWheelsPos;

    void Start()
    {

    }

    void Update()
    {
        // Tweening position
        transform.position += (desiredFrontPos - prevFrontPos) * Time.deltaTime / Time.fixedDeltaTime;
        backWheelsPos += (desiredRearPos - prevRearPos) * Time.deltaTime / Time.fixedDeltaTime;
        transform.forward = (transform.position - backWheelsPos).normalized;
    }

    public void UpdateTrainPosition(Vector3 frontPosition, Vector3 rearPosition)
    {
        // transform.position = desiredFrontPos;
        // backWheelsPos = desiredRearPos;

        prevFrontPos = desiredFrontPos;
        prevRearPos = desiredRearPos;

        desiredRearPos = rearPosition;
        desiredFrontPos = frontPosition;
    }
}
