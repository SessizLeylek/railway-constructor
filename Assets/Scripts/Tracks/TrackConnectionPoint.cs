using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackConnectionPoint
{
    public SingleTrack[] connectedTracks;
    public Vector3 worldPosition;
    public bool vehicleOnTop = false;   // True if there is a train on top

    /// <summary>
    /// Stores the all tracks connected at one point
    /// </summary>
    /// <param name="track">First track conneceted</param>
    public TrackConnectionPoint(SingleTrack track, Vector3 worldPosition)
    {
        if(track != null)
        {
            connectedTracks = new SingleTrack[1];
            connectedTracks[0] = track;
        }
        else
        {
            connectedTracks = new SingleTrack[0];
        }

        TrackManager.instance.connectionPoints.Add(this);
        this.worldPosition = worldPosition;
    }

    public void ConnectTrack(SingleTrack track)
    {
        SingleTrack[] newArray = new SingleTrack[connectedTracks.Length + 1];
        connectedTracks.CopyTo( newArray, 0 );
        newArray[connectedTracks.Length] = track;

        connectedTracks = newArray;
    }

    public void DisconnectTrack(SingleTrack track)
    {
        if(connectedTracks.Length < 2)
        {
            TrackManager.instance.connectionPoints.Remove(this);
            return;
        }

        SingleTrack[] newArray = new SingleTrack[connectedTracks.Length - 1];
        for (int i = 0, j = 0; i < connectedTracks.Length; i++)
        {
            if (connectedTracks[i] != track)
            {
                newArray[j] = connectedTracks[i];
                j++;
            }
        }

        connectedTracks = newArray;
    }
}
