using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Arc
{
    public float Angle => arcAngle;
    public float Radius => radius;
    public float Length => radius * Mathf.PI * Mathf.Abs(arcAngle / 180f);

    Vector3 orthogonalVector;
    Vector3 startPoint;
    float arcAngle;
    float radius;

    /// <summary>
    /// Defines a circular arc
    /// </summary>
    /// <param name="_startPoint">The start position of the arc</param>
    /// <param name="_orthogonalVector">Vector that is orthogonal to the plane spanned by arc</param>
    /// <param name="a">The angle of the arc</param>
    public Arc(Vector3 _startPoint, Vector3 _orthogonalVector, float a)
    {
        startPoint = _startPoint;
        orthogonalVector = _orthogonalVector;
        arcAngle = a;
        radius = startPoint.magnitude;
    }

    /// <summary>
    /// Returns a point on the arc according to the value t = [0, 1]
    /// </summary>
    /// <param name="t">t = [0, 1], 0 is the start and 1 is the end point of the arc</param>
    /// <returns>A point on the arc</returns>
    public Vector3 ReturnPoint(float t = 0)
    {
        return Quaternion.AngleAxis(arcAngle * t, orthogonalVector) * startPoint;
    }

    /// <summary>
    /// Returns the tangent vector on a point according to the value t
    /// </summary>
    /// <param name="t">t = [0, 1], 0 is the start and 1 is the end point of the arc</param>
    public Vector3 ReturnTangentVector(float t = 0)
    {
        return Vector3.Cross(ReturnPoint(t), orthogonalVector).normalized;
    }
}