using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public TrainCar[] cars;
    public float speed = 20;

    [HideInInspector] public SingleTrack[] trainRoute;
    int routePosition = 0;
    float metersTravelled = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if(routePosition < trainRoute.Length)
        {
            cars[0].UpdateTrainPosition(trainRoute[routePosition].transform.position + trainRoute[routePosition].arc.ReturnPoint(metersTravelled / trainRoute[routePosition].arc.Length));

            metersTravelled += speed * Time.deltaTime;
            if(metersTravelled > trainRoute[routePosition].arc.Length)
            {
                metersTravelled -= trainRoute[routePosition].arc.Length;
                routePosition++;
            }
        }
    }
}
