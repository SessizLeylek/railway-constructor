using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SingleTrack : MonoBehaviour
{
    public Arc arc;

    Mesh mesh;

    void Start()
    {
        mesh = new Mesh();

        // Creating the mesh vertices
        float vertOrientation = Mathf.Sign(arc.Angle) * 0.5f;
        int meshLength = Mathf.CeilToInt(arc.Length);
        Vector3[] vertexArray = new Vector3[meshLength * 2 + 2];
        for (int i = 0; i < meshLength + 1; i++)
        {
            float _t = i / (float)meshLength;
            Vector3 midPoint = arc.ReturnPoint(_t);

            vertexArray[i * 2] = midPoint - midPoint.normalized * vertOrientation;
            vertexArray[i * 2 + 1] = midPoint + midPoint.normalized * vertOrientation;
        }
        mesh.vertices = vertexArray;

        //Creating the mesh faces
        int[] triangleArray = new int[meshLength * 6];
        for(int i = 0; i < meshLength; i++)
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
        Vector2[] uvArray = new Vector2[vertexArray.Length];
        for(int i = 0;i < meshLength + 1;i++)
        {
            uvArray[i * 2] = new Vector2(1, i % 2);
            uvArray[i * 2 + 1] = new Vector2(0, i % 2);
        }
        mesh.uv = uvArray;

        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        Debug.DrawRay(arc.ReturnPoint(1) + transform.position, arc.ReturnTangentVector(1), Color.yellow);
    }
}
