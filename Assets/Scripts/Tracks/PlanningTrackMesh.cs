using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlanningTrackMesh : MonoBehaviour
{
    public PlanningNode headNode;
    public int headDirectionMultiplier = 1;    // Multiplies the direction vector of head node
    public PlanningNode tailNode;
    public int tailDirectionMultiplier = 1;    // Multiplies the direction vector of tail node

    Mesh mesh;
    MeshFilter meshFilter;

    Vector3 arc1Position;
    Arc arc1;
    Vector3 arc2Position;
    Arc arc2;

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

    void UpdateMesh()
    {
        //First recalculate positions and angles of the new arcs, then generate the meshes for these arcs
        
        Vector3 upVector = Vector3.up;  // I am not sure will this be needed to be changed but...

        ////// CALCULATING ARCS ////// thanks to ryanjuckett for the formulas

        Vector3 v = tailNode.transform.position - headNode.transform.position;
        Vector3 t1 = headNode.nodeDirection * headDirectionMultiplier;
        Vector3 t2 = tailNode.nodeDirection * tailDirectionMultiplier;

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

        mesh = new Mesh();

        int mesh1Length = Mathf.Max(0, Mathf.CeilToInt(arc1.Length));
        int mesh2Length = Mathf.Max(0, Mathf.CeilToInt(arc2.Length));

        Vector3[] finalVertexArray = new Vector3[(mesh1Length + mesh2Length) * 2 + 2];
        int[] finalTriangleArray = new int[(mesh1Length + mesh2Length) * 6];

        int activeArcCount = 2;
        for (int j = 0; j < activeArcCount; j++)
        {
            Arc arc = j == 0 ? arc1 : arc2;
            Vector3 cntr = j == 0 ? center1 : center2;

            int dltVl = j * mesh1Length;

            // Creating the mesh vertices
            float vertOrientation = Mathf.Sign(arc.Angle) * 0.5f;
            int meshLength = Mathf.CeilToInt(arc.Length);
            for (int i = 0; i < meshLength + 1; i++)
            {
                float _t = i / (float)meshLength;
                Vector3 midPoint = arc.ReturnPoint(_t);

                finalVertexArray[(i + dltVl) * 2] = midPoint - midPoint.normalized * vertOrientation + cntr;
                finalVertexArray[(i + dltVl) * 2 + 1] = midPoint + midPoint.normalized * vertOrientation + cntr;
            }

            //Creating the mesh faces
            for (int i = 0; i < meshLength; i++)
            {
                finalTriangleArray[(i + dltVl) * 6] = (i + dltVl) * 2;
                finalTriangleArray[(i + dltVl) * 6 + 1] = (i + dltVl) * 2 + 1;
                finalTriangleArray[(i + dltVl) * 6 + 2] = (i + dltVl) * 2 + 2;
                finalTriangleArray[(i + dltVl) * 6 + 3] = (i + dltVl) * 2 + 1;
                finalTriangleArray[(i + dltVl) * 6 + 4] = (i + dltVl) * 2 + 3;
                finalTriangleArray[(i + dltVl) * 6 + 5] = (i + dltVl) * 2 + 2;
            }
        }

        mesh.vertices = finalVertexArray;
        mesh.triangles = finalTriangleArray;

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
}
