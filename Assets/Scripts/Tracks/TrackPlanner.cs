using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
    Vector3 firstNodeDirection = Vector3.forward;   // Direction of the first node while extending

    [HideInInspector] public List<PlanningNode> planningNodes = new List<PlanningNode>();
    [HideInInspector] public List<PlanningTrackMesh> planningTracks = new List<PlanningTrackMesh>();

    enum PlanningState { Inactive, Idle, Extending, Moving, Rotating }
    PlanningState planningState = PlanningState.Inactive;

    void Awake()
    {
        TrackManager.instance.planner = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        switch (planningState)
        {
            case PlanningState.Inactive:
                // NO PLANNING //
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        // Activate all planning nodes and tracks then switch to idle

                        foreach (PlanningNode planNode in planningNodes)
                        {
                            planNode.gameObject.SetActive(true);
                        }

                        foreach (PlanningTrackMesh planTrack in planningTracks)
                        {
                            planTrack.gameObject.SetActive(true);
                        }

                        planningState = PlanningState.Idle;
                    }

                    if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, interactionLayer))
                    {
                        if (hit.transform.CompareTag("rail"))
                        {
                            //Raycasting rails while not in planning

                            if (Input.GetKeyDown(KeyCode.Backspace))
                            {
                                //Pressing backspace while hovering a rail, removes it

                                hit.transform.GetComponent<SingleTrack>().RemoveTrack();
                            }
                        }
                    }
                }
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
                                    nodeInUse = planNode1;

                                    prevNodePosition = planNode0.transform.position;
                                    firstNodeDirection = planNode0.nodeDirection;

                                    planNode1.GetComponent<SphereCollider>().enabled = false;   // Temporarily disable the collider to avoid issues

                                    planningTracks.Add(planTrack);
                                    planningNodes.Add(planNode0);
                                    planningNodes.Add(planNode1);

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
                                    nodeInUse = planNode1;

                                    prevNodePosition = planNode0.transform.position;
                                    firstNodeDirection = planNode0.nodeDirection;

                                    planNode1.GetComponent<SphereCollider>().enabled = false;   // Temporarily disable the collider to avoid issues

                                    planningTracks.Add(planTrack);
                                    planningNodes.Add(planNode0);
                                    planningNodes.Add(planNode1);

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

                            if (Input.GetMouseButtonDown(0))
                            {
                                // To extend an existing planning node, create another planning node and a planning track

                                PlanningNode currentPlanNode = hit.transform.GetComponent<PlanningNode>();

                                PlanningTrackMesh planTrack = Instantiate(planningTrackPrefab).GetComponent<PlanningTrackMesh>();
                                PlanningNode newPlanNode = Instantiate(planningNodePrefab).GetComponent<PlanningNode>();
                                planTrack.SetNodes(currentPlanNode, newPlanNode);
                                nodeInUse = newPlanNode;

                                prevNodePosition = currentPlanNode.transform.position;
                                firstNodeDirection = currentPlanNode.nodeDirection;

                                newPlanNode.GetComponent<SphereCollider>().enabled = false;

                                planningTracks.Add(planTrack);
                                planningNodes.Add(newPlanNode);

                                planningState = PlanningState.Extending;
                            }
                            else if (Input.GetKeyDown(KeyCode.Backspace))
                            {
                                // Removing a planning node

                                hit.transform.GetComponent<PlanningNode>().RemoveNode();
                            }
                            else if (Input.GetKeyDown(KeyCode.T))
                            {
                                // Moving a planning node

                                nodeInUse = hit.transform.GetComponent<PlanningNode>();

                                if(!nodeInUse.isFixed)
                                    planningState = PlanningState.Moving;
                            }
                            else if (Input.GetKeyDown(KeyCode.R))
                            {
                                // Rotating a planning node

                                nodeInUse = hit.transform.GetComponent<PlanningNode>();

                                if (!nodeInUse.isFixed)
                                    planningState = PlanningState.Rotating;
                            }

                        }   // PLANNING NODE RAYCASTING
                    }

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        // Saving the plan

                        foreach (PlanningNode planNode in planningNodes)
                        {
                            if (planNode.connectionPoint == null)
                                planNode.connectionPoint = new TrackConnectionPoint(null, planNode.transform.position);
                        }

                        while (planningTracks.Count > 0)
                        {
                            planningTracks[0].ConstruckTracks();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        //Hide the plan and switch state to inactive

                        foreach (PlanningNode planNode in planningNodes)
                        {
                            planNode.gameObject.SetActive(false);
                        }

                        foreach (PlanningTrackMesh planTrack in planningTracks)
                        {
                            planTrack.gameObject.SetActive(false);
                        }

                        planningState = PlanningState.Inactive;
                    }
                    else if (Input.GetKeyDown(KeyCode.X))
                    {
                        // Delete the plan

                        while (planningTracks.Count > 0)
                        {
                            planningTracks[0].RemoveTrack();
                        }
                    }
                }
                break;
            case PlanningState.Extending:
                // EXTENDING AN TRACK //
                {
                    // Positioning the node in use to set the end point of the track
                    if(nodeInUse != null)
                    {
                        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hit, 500, interactionLayer))
                        {
                            // Connecting two nodes together to form a track

                            if (_hit.transform.CompareTag("node"))
                            {
                                PlanningNode nodeToConnect = _hit.transform.GetComponent<PlanningNode>();

                                nodeInUse.transform.position = nodeToConnect.transform.position;
                                nodeInUse.nodeDirection = nodeToConnect.nodeDirection;

                                if (Input.GetMouseButtonDown(0))
                                {
                                    // Left clicking connects the two nodes

                                    PlanningTrackMesh planTrack = Instantiate(planningTrackPrefab).GetComponent<PlanningTrackMesh>();
                                    planTrack.SetNodes(nodeInUse.connectedTracks[0].headNode, nodeToConnect);

                                    nodeInUse.RemoveNode(); // This section removes the created track and recreates it. I am too lazy to implement a better solution

                                    planningTracks.Add(planTrack);

                                    planningState = PlanningState.Idle;
                                }
                            }

                            //// DO NOT FORGET TO ADD THE SECTION ABOUT CONNECTING TO A TRACK ////
                        }
                        else if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, terrainLayer))
                        {
                            // Extending a new track

                            nodeInUse.transform.position = hit.point;
                            nodeInUse.nodeDirection = (firstNodeDirection - Vector3.ProjectOnPlane(firstNodeDirection, prevNodePosition - nodeInUse.transform.position) * 2).normalized;

                            Debug.DrawRay(prevNodePosition, firstNodeDirection * 3, Color.blue);
                            Debug.DrawRay(hit.point, nodeInUse.nodeDirection * 3, Color.blue);
                            Debug.DrawLine(prevNodePosition, hit.point, Color.yellow);

                            if (Input.GetMouseButtonDown(0))
                            {
                                // Left clicking completes extending

                                nodeInUse.GetComponent<SphereCollider>().enabled = true;    // Renabling the collider of the node
                                nodeInUse = null;

                                planningState = PlanningState.Idle;
                            }
                        }
                    }
                }
                break;
            case PlanningState.Moving:
                // MOVING A PLANNING NODE //
                {
                    if(nodeInUse != null && Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, terrainLayer))
                    {
                        nodeInUse.transform.position = hit.point;

                        if (Input.GetMouseButtonDown(0))
                        {
                            // Left clicking confirms the node positioning
                            planningState = PlanningState.Idle;
                        }
                    }
                }
                break;
            case PlanningState.Rotating:
                // CHANGING THE DIRECTION OF A PLANNING NODE //
                {
                    if (nodeInUse != null && Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500, terrainLayer))
                    {
                        nodeInUse.nodeDirection = (hit.point - nodeInUse.transform.position).normalized;

                        if (Input.GetMouseButtonDown(0))
                        {
                            // Left clicking confirms the node rotating
                            planningState = PlanningState.Idle;
                        }
                    }
                }
                break;
        }
        
    }
}
