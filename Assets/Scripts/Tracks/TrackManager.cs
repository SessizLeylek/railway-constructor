using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores all track information
public class TrackManager : MonoBehaviour
{
    [HideInInspector] public TrackPlanner planner;
    [HideInInspector] public List<SingleTrack> tracks = new List<SingleTrack>();
    [HideInInspector] public List<TrackConnectionPoint> connectionPoints = new List<TrackConnectionPoint>();

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void AddTrack(SingleTrack track)
    {
        tracks.Add(track);
    }

    public void RemoveTrack(SingleTrack track)
    {
        tracks.Remove(track);
    }

    // Singleton Instance
    public static TrackManager instance;
    public TrackManager()
    {
        if (instance == null)
            instance = this;
    }

    private void OnDrawGizmos()
    {
        foreach(TrackConnectionPoint con in connectionPoints)
        {
            Gizmos.DrawSphere(con.worldPosition, 0.1f);
        }
    }
}
