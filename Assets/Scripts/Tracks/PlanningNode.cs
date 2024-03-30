using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningNode : MonoBehaviour
{
    [HideInInspector] public Vector3 nodeDirection;
    PlanningNode[] connectedNodes;

    void Start()
    {
    }

    void Update()
    {
        nodeDirection = transform.forward;
    }
}
