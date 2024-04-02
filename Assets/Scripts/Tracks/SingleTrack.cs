using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SingleTrack : MonoBehaviour
{
    public Arc arc;
    public TrackConnectionPoint headConnection;
    public TrackConnectionPoint tailConnection;

    Mesh mesh;

    void Awake()
    {
        TrackManager.instance.AddTrack(this);
    }

    /// <summary>
    /// Initializes the track instance, must use after instatiation
    /// </summary>
    /// <param name="_arc">Arc that forms the track</param>
    /// <param name="_position">World position of the origin of the track</param>
    /// <param name="_headConnection">Connection Point at the head of the track, do not assign if it is not connected</param>
    /// <param name="_tailConnection">Connection Point at the end of the track, do not assign if it is not connected</param>
    public void Initialize(Arc _arc, Vector3 _position, TrackConnectionPoint _headConnection = null, TrackConnectionPoint _tailConnection = null)
    {
        arc = _arc;
        transform.position = _position;

        //Assigning connections if not assigned
        if (_headConnection == null) headConnection = new TrackConnectionPoint(this, arc.ReturnPoint(0) + transform.position);
        else headConnection = _headConnection;

        if (_tailConnection == null) tailConnection = new TrackConnectionPoint(this, arc.ReturnPoint(1) + transform.position);
        else tailConnection = _tailConnection;
    }

    void Start()
    {
        #region MESH_GENERATION
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
        GetComponent<MeshCollider>().sharedMesh = mesh;
        #endregion
    }

    void Update()
    {
        Debug.DrawRay(arc.ReturnPoint(1) + transform.position, arc.ReturnTangentVector(1), Color.yellow);
    }

    private void OnDestroy()
    {
        TrackManager.instance.RemoveTrack(this);
    }

    /// <summary>
    /// Returns the world position of a point on the arc according to the value t = [0, 1]
    /// </summary>
    /// <param name="t">t = [0, 1], 0 is the start and 1 is the end point of the arc</param>
    /// <returns>A point on the arc</returns>
    public Vector3 ReturnPointWorldPosition(float t = 0)
    {
        return arc.ReturnPoint(t) + transform.position;
    }
}
