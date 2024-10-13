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

    Vector3 newFrontPos;
    Vector3 newBackPos;
    Vector3 oldFrontPos;
    Vector3 oldBackPos;

    void Start()
    {

    }

    void Update()
    {
        // Tweening position (solution 1)
        /*transform.position += frontWheelLerpDir * Time.deltaTime;
        backWheelsPos += backWheelLerpDir * Time.deltaTime;
        transform.forward = (transform.position - backWheelsPos).normalized;*/

        //solution 2
        float interpolationAlpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        transform.position = Vector3.Lerp(oldFrontPos, newFrontPos, interpolationAlpha);
        backWheelsPos = Vector3.Lerp(oldBackPos, newBackPos, interpolationAlpha);
        transform.forward = (transform.position - backWheelsPos);

        Debug.Log($"{transform.position}, {backWheelsPos}");
    }

    public void UpdateTrainPosition(Vector3 frontPosition, Vector3 rearPosition)
    {
        oldFrontPos = transform.position;
        oldBackPos = backWheelsPos;
        newFrontPos = frontPosition;
        newBackPos = rearPosition;

        frontWheelLerpDir = (frontPosition - transform.position) / Time.fixedDeltaTime;
        backWheelLerpDir = (rearPosition - backWheelsPos) / Time.fixedDeltaTime;
    }
}
