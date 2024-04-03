using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For creating TrackPlans(Which later will be tracks if they are not cancelled) according to user inputs
public class TrackPlanner : MonoBehaviour
{
    public GameObject trackTipHitbox;
    GameObject[] trackTips;

    [SerializeField] GameObject planningTrackPrefab;
    [SerializeField] GameObject planningNodePrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask terrainLayer;
    [SerializeField] LayerMask interactionLayer;

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
                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, interactionLayer))
                    {
                        if (hit.transform.CompareTag("rail"))
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
                                    PlanningNode planNode0 = Instantiate(planningNodePrefab).GetComponent<PlanningNode>();
                                    PlanningNode planNode1 = Instantiate(planningNodePrefab).GetComponent<PlanningNode>();
                                    planTrack.SetNodes(planNode0, planNode1);
                                    planNode0.SetValues(track.ReturnPointWorldPosition(0), track.arc.ReturnTangentVector(0), true, track.headConnection);
                                    prevNodePosition = planNode0.transform.position;
                                    nodeInUse = planNode1;

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
                                    PlanningNode planNode0 = Instantiate(planningNodePrefab).GetComponent<PlanningNode>();
                                    PlanningNode planNode1 = Instantiate(planningNodePrefab).GetComponent<PlanningNode>();
                                    planTrack.SetNodes(planNode0, planNode1);
                                    planNode0.SetValues(track.ReturnPointWorldPosition(1), track.arc.ReturnTangentVector(1), true, track.tailConnection);
                                    prevNodePosition = planNode0.transform.position;
                                    nodeInUse = planNode1;

                                    planningState = PlanningState.Extending;
                                }
                            }
                            else
                            {
                                // If another section of the track is raycasted, ...

                            }

                            Debug.DrawRay(track.ReturnPointWorldPosition(raycastTValue), Vector3.up * 5, Color.blue);

                        }   // TRACK RAYCASTING
                        else if (hit.transform.CompareTag("node"))
                        {
                            // Raycasting planning nodes



                        }   // PLANNING NODE RAYCASTING
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
