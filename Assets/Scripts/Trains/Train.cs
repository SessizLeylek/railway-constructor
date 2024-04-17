using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public TrainRoute route;
    public TrainCar headCar;
    public TrainCar middleCar;
    public TrainCar tailCar;
    public int carCount;
    public float maxSpeed = 20;
    public float acceleration = 10;
    public bool isMoving = true;

    TrainCar[] cars;
    float currentSpeed = 0;
    float metersTravelled = 0;

    void Start()
    {
        // Adjusting the train size
        cars = new TrainCar[carCount];
        if(headCar != null) cars[0] = headCar;
        if (tailCar != null) cars[carCount - 1] = tailCar;
        for (int i = 0; i < carCount; i++)
        {
            if (cars[i] == null) 
            {
                cars[i] = Instantiate(middleCar, transform);
                cars[i].gameObject.SetActive(true);
            } 
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            float futureMaxSpeed = route.MaxSpeedFromDistance(metersTravelled + (currentSpeed * currentSpeed * 0.5f / acceleration)) * 5;

            if (futureMaxSpeed > currentSpeed)
                currentSpeed += acceleration * Time.fixedDeltaTime;
            else if (futureMaxSpeed < currentSpeed)
                currentSpeed -= acceleration * Time.fixedDeltaTime;

            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
            else if (currentSpeed < 0)
                currentSpeed = 0;

            if (metersTravelled + Time.fixedDeltaTime * currentSpeed < route.routeLength)
                metersTravelled += Time.fixedDeltaTime * currentSpeed;

            for (int i = 0; i < carCount; i++)
            {
                float frontPos = metersTravelled - i * (i == 0 ? 0 : cars[i - 1].carLength);
                cars[i].UpdateTrainPosition(route.PositionFromDistance(frontPos), route.PositionFromDistance(frontPos - cars[i].wheelsWidth));
            }
        }
    }
}

public class TrainRoute
{
    public float routeLength;

    SingleTrack[] tracks;
    bool[] invertedTrackDirection;    // train goes from tail to head if set to true
    float[] trackDistances;
    float[] maxSpeeds;

    public TrainRoute(SingleTrack[] _tracks)
    {
        tracks = _tracks;

        float distanceTraveled = 0;
        invertedTrackDirection = new bool[tracks.Length];
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

        float positionValue = (distanceFromStart - trackDistances[trackPos]) / tracks[trackPos].arc.Length;
        return tracks[trackPos].transform.position + tracks[trackPos].arc.ReturnPoint(invertedTrackDirection[trackPos] ? 1 - positionValue : positionValue);
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