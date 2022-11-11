using SplineEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSpline : MonoBehaviour
{
    public BezierSpline spline;
    public float t;

    // Update is called once per frame
    void Update()
    {
        transform.position = spline.GetPoint(t);
    }
}
