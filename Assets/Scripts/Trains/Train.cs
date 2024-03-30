using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public TrainRoute route;
    public float maxSpeed = 20;
    public float acceleration = 10;
    public TrainCar[] cars;
    float currentSpeed = 0;
    float metersTravelled = 0;

    void Start()
    {
    }

    void FixedUpdate()
    {
        float futureMaxSpeed = route.MaxSpeedFromDistance(metersTravelled + (currentSpeed * currentSpeed * 0.5f / acceleration)) * 5;

        if (futureMaxSpeed > currentSpeed)
            currentSpeed += acceleration * Time.fixedDeltaTime;
        else if(futureMaxSpeed < currentSpeed)
            currentSpeed -= acceleration * Time.fixedDeltaTime;

        if(currentSpeed > maxSpeed)
            currentSpeed = maxSpeed;
        else if(currentSpeed < 0)
            currentSpeed = 0;

        if(metersTravelled + Time.fixedDeltaTime * currentSpeed < route.routeLength)
            metersTravelled += Time.fixedDeltaTime * currentSpeed;

        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].UpdateTrainPosition(route.PositionFromDistance(metersTravelled - i * cars[0].carLength));
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

            maxSpeeds[i] = tracks[i].arc.Radius * 0.04f + 0.6f;
        }

        routeLength = distanceTraveled;
    }

    // Track's index according to the distance from the start
    int trackPosition(float distanceFromStart)
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            float d = distanceFromStart - trackDistances[i];
            if (d <= tracks[i].arc.Length && d >= 0)
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

    public float MaxSpeedFromDistance(float distanceFromStart)
    {
        int tr = trackPosition(distanceFromStart);
        if (tr == -1)
            return 0;
        else
            return maxSpeeds[tr];
    }

    public SingleTrack TrackFromDistance(float distanceFromStart)
    {
        int tr = trackPosition(distanceFromStart);
        if (tr == -1)
            return null;
        else
            return tracks[tr];
    }
}