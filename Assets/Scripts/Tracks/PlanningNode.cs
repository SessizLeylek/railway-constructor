using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningNode : MonoBehaviour
{
    public bool isFixed = false;    // Fixed nodes cannot be moved, rotated or deleted
    public Vector3 nodeDirection;
    [HideInInspector] public TrackConnectionPoint connectionPoint = null;
    PlanningNode[] connectedNodes;

    public void SetValues(Vector3? worldPosition = null, Vector3? nodeDirection = null, bool fixedPosition = false, TrackConnectionPoint connectionPoint = null)
    {
        if (worldPosition != null) transform.position = worldPosition.Value;
        if(nodeDirection != null) this.nodeDirection = nodeDirection.Value;
        this.isFixed = fixedPosition;
        this.connectionPoint = connectionPoint;
    }
}
