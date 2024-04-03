using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.Intrinsics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlanningTrackMesh : MonoBehaviour
{
    public PlanningNode headNode;
    public int headDirectionMultiplier = 1;    // Multiplies the direction vector of head node
    public PlanningNode tailNode;
    public int tailDirectionMultiplier = 1;    // Multiplies the direction vector of tail node

    Mesh mesh;
    MeshFilter meshFilter;

    bool formedBySingleArc = false;
    Vector3 arc1Position;
    Arc arc1;
    Vector3 arc2Position;
    Arc arc2;

    bool isLine = false;    // is true if the track is a straight line
    Vector3 lineHeadPosition;
    Vector3 lineTailPosition;

    public bool dbgCreateTracks = false;
    public GameObject trackPrefab;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void FixedUpdate()
    {
        UpdateMesh();
    }

    public void SetNodes(PlanningNode headNode, PlanningNode tailNode, int headDirectionMultiplier = 1, int tailDirectionMultiplier = 1)
    {
        this.headNode = headNode;
        this.tailNode = tailNode;
        this.headDirectionMultiplier = headDirectionMultiplier;
        this.tailDirectionMultiplier = tailDirectionMultiplier;
    }

    void UpdateMesh()
    {
        mesh = new Mesh();

        Vector3 upVector = Vector3.up;  // I am not sure will this be needed to be changed but...

        Vector3 v = tailNode.transform.position - headNode.transform.position;
        Vector3 t1 = headNode.nodeDirection * headDirectionMultiplier;
        Vector3 t2 = tailNode.nodeDirection * tailDirectionMultiplier;

        // First, check if the track is a line
        isLine = Vector3.Cross(t1, v).sqrMagnitude < 0.00001f && Vector3.Cross(t2, v).sqrMagnitude < 0.00001f;

        if (isLine)
        {
            mesh.vertices = new Vector3[] { headNode.transform.position - Vector3.Cross(v, upVector).normalized * 0.5f,
                                            headNode.transform.position + Vector3.Cross(v, upVector).normalized * 0.5f,
                                            tailNode.transform.position - Vector3.Cross(v, upVector).normalized * 0.5f,
                                            tailNode.transform.position + Vector3.Cross(v, upVector).normalized * 0.5f};
            mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2};
        }  // CREATING THE MESH WITH A LINE
        else
        {
            //If it is not a line, then recalculate positions and angles of the new arcs, then generate the meshes for these arcs
            ////// CALCULATING ARCS ////// thanks to ryanjuckett for the formulas

            // Choosing d1 section
            float _vt = Vector3.Dot(v, t1 + t2);
            float _tt = 2 * (1 - Vector3.Dot(t1, t2));

            float d;
            if (_tt == 0)
                d = v.sqrMagnitude / Vector3.Dot(v, t2) * 0.25f;
            else
                d = (Mathf.Sqrt(_vt * _vt + _tt * v.sqrMagnitude) - _vt) / _tt;

            // Edge cases section
            Vector3 pm = 0.5f * (headNode.transform.position + tailNode.transform.position + d * t1 - d * t2);    // Connection point

            // Finding the center section
            Vector3 n1 = Vector3.Cross(t1, upVector);
            Vector3 n2 = Vector3.Cross(t2, upVector);

            Vector3 center1 = headNode.transform.position + n1 * ((pm - headNode.transform.position).sqrMagnitude / Vector3.Dot(n1, pm - headNode.transform.position) * 0.5f);  // It should be a line if the denominator is zero
            Vector3 center2 = tailNode.transform.position + n2 * ((pm - tailNode.transform.position).sqrMagnitude / Vector3.Dot(n2, pm - tailNode.transform.position) * 0.5f);  // Gotta deal with it later

            // Choosing a direction section
            Vector3 op1 = (headNode.transform.position - center1).normalized;
            Vector3 om1 = (pm - center1).normalized;
            Vector3 op2 = (tailNode.transform.position - center2).normalized;
            Vector3 om2 = (pm - center2).normalized;

            float angle1 = Mathf.Acos(Vector3.Dot(op1, om1)) * (Vector3.Cross(op1, om1).y > 0 ? 1 : -1) * Mathf.Rad2Deg;
            float angle2 = Mathf.Acos(Vector3.Dot(op2, om2)) * (Vector3.Cross(op2, om2).y > 0 ? 1 : -1) * -Mathf.Rad2Deg;

            ////// Assigning arcs //////

            arc1 = new Arc(headNode.transform.position - center1, upVector, angle1);
            arc1Position = center1;
            arc2 = new Arc(pm - center2, upVector, angle2);
            arc2Position = center2;

            ////// MESH GENERATION //////
            // Directly copied from the singletrack code

            int mesh1Length = Mathf.Max(0, Mathf.CeilToInt(arc1.Length));
            int mesh2Length = Mathf.Max(0, Mathf.CeilToInt(arc2.Length));

            Vector3[] finalVertexArray = new Vector3[(mesh1Length + mesh2Length) * 2 + 2];
            int[] finalTriangleArray = new int[(mesh1Length + mesh2Length) * 6];

            if (mesh1Length == 0)
            {
                // Create the mesh with only arc2 if arc1 is 0
                var meshProps = CalculateMeshProperties(ref arc2, ref arc2Position);
                meshProps.Item1.CopyTo(finalVertexArray, 0);
                meshProps.Item2.CopyTo(finalTriangleArray, 0);
            }
            else
            {
                // Create the mesh for the both arcs
                var meshProps1 = CalculateMeshProperties(ref arc1, ref arc1Position);

                meshProps1.Item1.CopyTo(finalVertexArray, 0);
                meshProps1.Item2.CopyTo(finalTriangleArray, 0);

                var meshProps2 = CalculateMeshProperties(ref arc2, ref arc2Position, meshProps1.Item1.Length - 2);

                meshProps2.Item1.CopyTo(finalVertexArray, meshProps1.Item1.Length - 2);
                meshProps2.Item2.CopyTo(finalTriangleArray, meshProps1.Item2.Length);
            }

            mesh.vertices = finalVertexArray;
            mesh.triangles = finalTriangleArray;
        }   // CREATING THE MESH WITH ARCS

        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;

        // for testing purposes
        if (dbgCreateTracks)
        {
            dbgCreateTracks = false;

            SingleTrack newTrack = Instantiate(trackPrefab).GetComponent<SingleTrack>();
            newTrack.transform.position = arc1Position;
            newTrack.arc = arc1;

            SingleTrack newTrack2 = Instantiate(trackPrefab).GetComponent<SingleTrack>();
            newTrack2.transform.position = arc2Position;
            newTrack2.arc = arc2;
        }
    }

    (Vector3[], int[]) CalculateMeshProperties(ref Arc _arc, ref Vector3 _arcPos, int triangleShifter = 0)
    {
        // Creating the mesh vertices
        float vertOrientation = Mathf.Sign(_arc.Angle) * 0.5f;
        int meshLength = Mathf.CeilToInt(_arc.Length);

        Vector3[] vertexArray = new Vector3[meshLength * 2 + 2];
        int[] triangleArray = new int[meshLength * 6];

        for (int i = 0; i < meshLength + 1; i++)
        {
            float _t = i / (float)meshLength;
            Vector3 midPoint = _arc.ReturnPoint(_t);

            vertexArray[i * 2] = midPoint - midPoint.normalized * vertOrientation + _arcPos;
            vertexArray[i * 2 + 1] = midPoint + midPoint.normalized * vertOrientation + _arcPos;
        }

        //Creating the mesh faces
        for (int i = 0; i < meshLength; i++)
        {
            triangleArray[i * 6] = i * 2 + triangleShifter;
            triangleArray[i * 6 + 1] = i * 2 + 1 + triangleShifter;
            triangleArray[i * 6 + 2] = i * 2 + 2 + triangleShifter;
            triangleArray[i * 6 + 3] = i * 2 + 1 + triangleShifter;
            triangleArray[i * 6 + 4] = i * 2 + 3 + triangleShifter;
            triangleArray[i * 6 + 5] = i * 2 + 2 + triangleShifter;
        }

        return (vertexArray, triangleArray);
    }
}
