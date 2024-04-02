using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For creating TrackPlans(Which later will be tracks if they are not cancelled) according to user inputs
public class TrackPlanner : MonoBehaviour
{
    public GameObject trackTipHitbox;
    GameObject[] trackTips;

    [SerializeField] GameObject planningTrackPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask terrainLayer;
    [SerializeField] LayerMask railLayer;

    PlanningNode nodeInUse = null;  // Node in use for extending, moving and rotating
    Vector3 prevNodePosition = Vector3.zero;    // Node's previous position before moving

    enum PlanningState { Inactive, Idle, Extending, Moving, Rotating }
    PlanningState planningState = PlanningState.Inactive;

    void Start()
    {
        
    }

    void Update()
    {
        switch (planningState)
        {
            case PlanningState.Inactive:
                // NO PLANNING //

                planningState = PlanningState.Idle;

                break;
            case PlanningState.Idle:
                // PLAN MODE ACTIVE, NO ACTION IS SELECTED //
                {
                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, railLayer))
                    {
                        // Raycasting tracks
                        SingleTrack track = hit.transform.GetComponent<SingleTrack>();

                        int rectCount = (hit.collider as MeshCollider).sharedMesh.triangles.Length / 6 - 1;
                        float raycastTValue = (hit.triangleIndex / 2) / (float)rectCount;

                        if (hit.triangleIndex < 2)
                        {
                            // If the head of the track is raycasted, ...

                            if (Input.GetMouseButtonDown(0))
                            {
                                //To start extending a track from the head, create two planning nodes and a planning track mesh

                                PlanningTrackMesh planTrack = Instantiate(planningTrackPrefab).GetComponent<PlanningTrackMesh>();
                                planTrack.headNode.transform.position = track.ReturnPointWorldPosition(0);
                                planTrack.headNode.fixedPosition = true;
                                planTrack.headNode.nodeDirection = track.arc.ReturnTangentVector(0);
                                planTrack.headNode.connectionPoint = track.headConnection;
                                prevNodePosition = planTrack.headNode.transform.position;
                                nodeInUse = planTrack.tailNode;

                                planningState = PlanningState.Extending;
                            }
                        }
                        else if (hit.triangleIndex / 2 == rectCount)
                        {
                            // If the tail of the track is raycasted, ...

                            if (Input.GetMouseButtonDown(0))
                            {
                                //To start extending a track from the tail, create two planning nodes and a planning track mesh

                                PlanningTrackMesh planTrack = Instantiate(planningTrackPrefab).GetComponent<PlanningTrackMesh>();
                                planTrack.headNode.transform.position = track.ReturnPointWorldPosition(1);
                                planTrack.headNode.fixedPosition = true;
                                planTrack.headNode.nodeDirection = track.arc.ReturnTangentVector(1);
                                planTrack.headNode.connectionPoint = track.tailConnection;
                                prevNodePosition = planTrack.headNode.transform.position;
                                nodeInUse = planTrack.tailNode;

                                planningState = PlanningState.Extending;
                            }
                        }
                        else
                        {
                            // If another section of the track is raycasted, ...

                        }

                        Debug.DrawRay(track.ReturnPointWorldPosition(raycastTValue), Vector3.up * 5, Color.blue);
                    }
                }
                break;
            case PlanningState.Extending:
                // EXTENDING AN TRACK //
                {
                    if (nodeInUse != null && Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, terrainLayer))
                    {
                        nodeInUse.transform.position = hit.point;
                        nodeInUse.nodeDirection = (nodeInUse.transform.position - prevNodePosition).normalized; // This method does not form an arc, CHANGE THIS

                        if (Input.GetMouseButtonDown(0))
                        {
                            // Left clicking completes extending

                            nodeInUse = null;

                            planningState = PlanningState.Idle;
                        }
                    }
                }
                break;
            case PlanningState.Moving:
                // MOVING A PLANNING NODE //
                break;
            case PlanningState.Rotating:
                // CHANGING THE DIRECTION OF A PLANNING NODE //
                break;
        }
        
    }
}
