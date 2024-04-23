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
    float totalCarLength;
    float currentSpeed = 0;
    float metersTravelled = 0;
    float currentMaxSpeed = 0;
    int nextTrackP = 0;
    float nextTrackMaxSpeed = 0;
    bool decelerate = false;

    void Start()
    {
        // Adjusting the train size
        cars = new TrainCar[carCount];
        if(headCar != null) cars[0] = headCar;
        if (tailCar != null) cars[carCount - 1] = tailCar;
        for (int i = 0; i < carCount; i++)
        {
            // Create new cars
            if (cars[i] == null) 
            {
                cars[i] = Instantiate(middleCar, transform);
                cars[i].gameObject.SetActive(true);
            }

            //Calculate total length by adding car lengths
            totalCarLength += cars[i].carLength;
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            currentMaxSpeed = route.MaxSpeedBetweenRange(metersTravelled - totalCarLength, metersTravelled);

            // Check if next track's max speed is lower or next connection point is occupied
            nextTrackP = route.trackPosition(metersTravelled) + 1;
            if (nextTrackP >= route.tracks.Length) { metersTravelled = 0; return; }
            nextTrackMaxSpeed = route.maxSpeeds[nextTrackP];

            float decPos = (currentSpeed * currentSpeed - nextTrackMaxSpeed * nextTrackMaxSpeed) / acceleration * 0.5f;
            decelerate = (decPos >= route.trackDistances[nextTrackP] - metersTravelled) || 
                (!route.invertedTrackDirection[nextTrackP] && route.tracks[nextTrackP].headConnection.vehicleOnTop) ||
                (route.invertedTrackDirection[nextTrackP] && route.tracks[nextTrackP].tailConnection.vehicleOnTop);

            // Accelerate if needed
            if (decelerate || currentMaxSpeed < currentSpeed) currentSpeed -= acceleration * Time.fixedDeltaTime;
            else if (currentMaxSpeed > currentSpeed) currentSpeed += acceleration * Time.fixedDeltaTime;

            Debug.DrawRay(route.PositionFromDistance(route.trackDistances[nextTrackP] - decPos), Vector3.up *5, decelerate ? Color.red : Color.green, 0.25f);

            // Clamp the speed
            if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;
            else if (currentSpeed < 0) currentSpeed = 0;

            if (metersTravelled + Time.fixedDeltaTime * currentSpeed < route.routeLength)
                metersTravelled += Time.fixedDeltaTime * currentSpeed;

            // Update positions of train cars
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

    public SingleTrack[] tracks;
    public bool[] invertedTrackDirection;    // train goes from tail to head if set to true
    public float[] trackDistances;
    public float[] maxSpeeds;

    /// <summary>
    /// Stores the route information consisting of single tracks
    /// </summary>
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

            maxSpeeds[i] = Mathf.Pow(tracks[i].arc.Radius * 0.04f + 0.6f, 2) * 3;

            
        }

        routeLength = distanceTraveled;
    }

    // Track's index according to the distance from the start
    public int trackPosition(float distanceFromStart)
    {
        if (distanceFromStart < 0)
            return 0;

        for (int i = 0; i < tracks.Length; i++)
        {
            float d = distanceFromStart - trackDistances[i];
            if (d <= tracks[i].arc.Length && d >= 0)
            {
                return i;
            }
        }

        return tracks.Length - 1;
    }

    /// <summary>
    /// Returns the world position, according to the relative position to the start point
    /// </summary>
    public Vector3 PositionFromDistance(float distanceFromStart)
    {
        int trackPos = trackPosition(distanceFromStart);

        float positionValue = (distanceFromStart - trackDistances[trackPos]) / tracks[trackPos].arc.Length;
        return tracks[trackPos].transform.position + tracks[trackPos].arc.ReturnPoint(invertedTrackDirection[trackPos] ? 1 - positionValue : positionValue);
    }

    /// <summary>
    /// Returns the max speed, according to the relative position to the start point
    /// </summary>
    public float MaxSpeedFromDistance(float distanceFromStart)
    {
        int tr = trackPosition(distanceFromStart);
        return maxSpeeds[tr];
    }

    /// <summary>
    /// Returns the max speed between the range, according to the relative positions to the start point
    /// </summary>
    public float MaxSpeedBetweenRange(float rangeStart, float rangeEnd)
    {
        int tr0 = trackPosition(rangeStart);
        int tr1 = trackPosition(rangeEnd);

        float maxSpeed = 360;

        for (int i = tr0; i <= tr1; i++)
        {
            if (maxSpeeds[i] < maxSpeed)
                maxSpeed = maxSpeeds[i];
        }

        return maxSpeed;
    }

    /// <summary>
    /// Returns the track, according to the relative position to the start point
    /// </summary>
    public SingleTrack TrackFromDistance(float distanceFromStart)
    {
        int tr = trackPosition(distanceFromStart);
        return tracks[tr];
    }
}