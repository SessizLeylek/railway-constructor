using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrackGenerator : MonoBehaviour
{
    [SerializeField] GameObject trackPrefab;
    [SerializeField] int trackCount;

    void Start()
    {
        Vector3 previousPosition = new Vector3(0, 0, 0);
        Vector3 previousDirection = new Vector3(1, 0, 0);

        SingleTrack[] tracks = new SingleTrack[trackCount];

        for (int i = 0; i < trackCount; i++)
        {
            int randDir = Random.Range(0, 2) * 2 - 1;
            float randAngle = Random.Range(15f, 180f) * randDir;
            float randRadius = Random.Range(5, 26);
            Vector3 differenceVector = Vector3.Cross(Vector3.up, previousDirection).normalized * randRadius * (-randDir);

            SingleTrack newTrack = Instantiate(trackPrefab).GetComponent<SingleTrack>();
            newTrack.transform.position = previousPosition - differenceVector;
            newTrack.arc = new Arc(differenceVector, Vector3.up, randAngle);

            previousPosition = newTrack.arc.ReturnPoint(1) + newTrack.transform.position;
            previousDirection = newTrack.arc.ReturnTangentVector(1);

            tracks[i] = newTrack;
        }

        FindObjectOfType<Train>().route = new TrainRoute(tracks);
    }

    void Update()
    {
        
    }
}
