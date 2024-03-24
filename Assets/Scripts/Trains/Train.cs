using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public TrainRoute route;
    public GameObject singleCarObject;
    public float speed = 20;
    public int carCount = 1;

    TrainCar[] cars;
    float metersTravelled = 0;

    void Start()
    {
        cars = new TrainCar[carCount];
        cars[0] = singleCarObject.GetComponent<TrainCar>();
        for (int i = 1; i < carCount; i++)
        {
            cars[i] = Instantiate(singleCarObject, transform).GetComponent<TrainCar>();
        }
    }

    void Update()
    {
        if(metersTravelled + Time.deltaTime * speed < route.routeLength)
            metersTravelled += Time.deltaTime * speed;

        for (int i = 0; i < carCount; i++)
        {
            cars[i].UpdateTrainPosition(route.PositionFromDistance(metersTravelled - i * 8));
        }

    }
}

public class TrainRoute
{
    public float routeLength;

    SingleTrack[] tracks;
    float[] trackDistances;
    float[] maxSpeeds;

    public TrainRoute(SingleTrack[] _tracks)
    {
        tracks = _tracks;

        float distanceTraveled = 0;
        trackDistances = new float[tracks.Length];
        maxSpeeds = new float[tracks.Length];
        for(int i = 0; i < tracks.Length; i++)
        {
            trackDistances[i] = distanceTraveled;
            distanceTraveled += tracks[i].arc.Length;

            maxSpeeds[i] = tracks[i].arc.Radius * 0.1f + 1.5f;
        }

        routeLength = distanceTraveled;
    }

    int trackPosition(float distanceFromStart)
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            float d = distanceFromStart - trackDistances[i];
            if (d < tracks[i].arc.Length && d > 0)
            {
                return i;
            }
        }

        return -1;
    }

    public Vector3 PositionFromDistance(float distanceFromStart)
    {
        int trackPos = trackPosition(distanceFromStart);

        if(trackPos == -1) return Vector3.zero;

        return tracks[trackPos].transform.position + tracks[trackPos].arc.ReturnPoint((distanceFromStart - trackDistances[trackPos]) / tracks[trackPos].arc.Length);
    }
}