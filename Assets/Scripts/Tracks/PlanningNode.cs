using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningNode : MonoBehaviour
{
    public bool isFixed = false;    // Fixed nodes cannot be moved, rotated or deleted
    public Vector3 nodeDirection;
    [HideInInspector] public TrackConnectionPoint connectionPoint = null;
    [HideInInspector] public List<PlanningTrackMesh> connectedTracks = new List<PlanningTrackMesh>();

    /// <summary>
    /// Set the necessary variables
    /// </summary>
    public void SetValues(Vector3? worldPosition = null, Vector3? nodeDirection = null, bool fixedPosition = false, TrackConnectionPoint connectionPoint = null)
    {
        if (worldPosition != null) transform.position = worldPosition.Value;
        if(nodeDirection != null) this.nodeDirection = nodeDirection.Value;
        this.isFixed = fixedPosition;
        this.connectionPoint = connectionPoint;
    }

    /// <summary>
    /// Removes the planning node with the tracks connected to it
    /// </summary>
    public void RemoveNode()
    {
        while (connectedTracks.Count > 0)
        {
            connectedTracks[0].RemoveTrack();
        }

        Destroy(gameObject);
    }
}
