using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlanningTrackMesh : MonoBehaviour
{
    [SerializeField] PlanningNode headNode;
    int headDirectionMultiplier = 1;    // Multiplies the direction vector of head node
    [SerializeField] PlanningNode tailNode;
    int tailDirectionMultiplier = 1;    // Multiplies the direction vector of tail node

    Mesh mesh;
    MeshFilter meshFilter;

    Vector3 arc1Position;
    Arc arc1;
    Vector3 arc2Position;
    Arc arc2;

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
        if (_tt != 0)
            d = (Mathf.Sqrt(_vt * _vt + _tt * v.sqrMagnitude) - _vt) / _tt;
        else
            d = v.sqrMagnitude / Vector3.Dot(v, t2) * 0.25f;

        // NOTE TO SELF: Something is wrong with this D value!!!!

        // Edge cases section
        Vector3 pm = headNode.transform.position + d * t1 + (Vector3.Dot(v, t2) - d * Vector3.Dot(t1, t2)) * t2;    // Connection point

        Debug.DrawRay(pm, Vector3.up, Color.yellow, 0.11f);

        // Finding the center section
        Vector3 n1 = Vector3.Cross(t1, upVector);
        Vector3 n2 = Vector3.Cross(t2, upVector);

        Vector3 center1 = headNode.transform.position + n1 * ((pm - headNode.transform.position).sqrMagnitude / Vector3.Dot(n1, pm - headNode.transform.position) * 0.5f);  // It should be a line if the denominator is zero
        Vector3 center2 = tailNode.transform.position + n2 * ((pm - tailNode.transform.position).sqrMagnitude / Vector3.Dot(n2, pm - tailNode.transform.position) * 0.5f);  // Gotta deal with it later

        print("den: " + Vector3.Dot(n1, pm - headNode.transform.position));
        
        // Choosing a direction section
        Vector3 op1 = (headNode.transform.position - center1).normalized;
        Vector3 om1 = (pm - center1).normalized;
        Vector3 op2 = (tailNode.transform.position - center2).normalized;
        Vector3 om2 = (pm - center2).normalized;

        float angle1 = Mathf.Acos(Vector3.Dot(op1, om1)) * (Vector3.Cross(op1, om1).y > 0 ? 1 : -1);
        float angle2 = Mathf.Acos(Vector3.Dot(op2, om2)) * (Vector3.Cross(op2, om2).y > 0 ? 1 : -1);

        ////// Assigning arcs //////
        
        arc1 = new Arc(headNode.transform.position, upVector, angle1);
        arc1Position = center1;
        arc2 = new Arc(pm, upVector, angle2);
        arc2Position = center2;

        print($"{arc1.Length} : {arc2.Length}");

        ////// MESH GENERATION //////
        // Directly copied from the singletrack code

        mesh = new Mesh();

        int activeArcCount = 1;
        for (int j = 0; j < activeArcCount; j++)
        {
            Arc arc = j == 0 ? arc1 : arc2;
            Vector3 cntr = j == 0 ? center1 : center2;

            // Creating the mesh vertices
            float vertOrientation = Mathf.Sign(arc.Angle) * 0.5f;
            int meshLength = Mathf.CeilToInt(arc.Length * 20);
            Vector3[] vertexArray = new Vector3[meshLength * 2 + 2];
            for (int i = 0; i < meshLength + 1; i++)
            {
                float _t = i / (float)meshLength;
                Vector3 midPoint = arc.ReturnPoint(_t) + cntr;

                vertexArray[i * 2] = midPoint - midPoint.normalized * vertOrientation;
                vertexArray[i * 2 + 1] = midPoint + midPoint.normalized * vertOrientation;
            }
            mesh.vertices = vertexArray;

            //Creating the mesh faces
            int[] triangleArray = new int[meshLength * 6];
            for (int i = 0; i < meshLength; i++)
            {
                triangleArray[i * 6] = i * 2;
                triangleArray[i * 6 + 1] = i * 2 + 1;
                triangleArray[i * 6 + 2] = i * 2 + 2;
                triangleArray[i * 6 + 3] = i * 2 + 1;
                triangleArray[i * 6 + 4] = i * 2 + 3;
                triangleArray[i * 6 + 5] = i * 2 + 2;
            }
            mesh.triangles = triangleArray;

            //Defining the uvs
            /*Vector2[] uvArray = new Vector2[vertexArray.Length];
            for (int i = 0; i < meshLength + 1; i++)
            {
                uvArray[i * 2] = new Vector2(1, i % 2);
                uvArray[i * 2 + 1] = new Vector2(0, i % 2);
            }
            mesh.uv = uvArray;*/

            mesh.RecalculateBounds();
            meshFilter.mesh = mesh;
        }
    }
}
