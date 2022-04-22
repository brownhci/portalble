using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will bind to Ink Object to help generate Bezier Smooth Line
/// It uses Cubic Bezier Curves to smooth line
/// 
/// </summary>

public class BezierLine : MonoBehaviour {
    private LineRenderer lineRenderer;
    private Vector3 previous1, previous2;
    private Vector3 current;

	// Use this for initialization
	void Start () {
        // Try to get LineRenderer Component
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Called to add a new point
    public void AddNewPoint (Vector3 pos) {
        if (lineRenderer == null) {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null) {
                return;
            }
        }

        // Add to point
        previous2 = previous1;
        previous1 = current;
        current = pos;
        if (previous2 == null)
            previous2 = pos;
        if (previous1 == null)
            previous1 = pos;

        // If we have enough number of points to smooth

    }

    private Vector3 calculateCubicBezierCurve (Vector3 point1, Vector3 point2, Vector3 ctrl1, Vector3 ctrl2, float t) {
        t = Mathf.Clamp01(t);
        float u = 1 - t;
        float u2 = u * u;
        float t2 = t * t;
        Vector3 result = u2 * u * point1 + 3 * u2 * t * ctrl1 + 3 * u * t2 * ctrl2 + t2 * t * point2;
        return result;
    }
}
