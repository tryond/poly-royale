using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SideManager : MonoBehaviour
{
    [Tooltip("number of sides to start with")] public int numSides = 0;
    [Tooltip("radius of polygon formed by sides")] public float radius;
    [Tooltip("time to transition between side layouts")] public float transitionTime;

    private Vector3[] points;
    private List<Vector3C> polygonPoints;
    private Vector3C[] pointTargets;
    //private (Vector3, Vector3)[] sides;
    private bool[] sideCollapsed;
    private int numActiveSides;
    private Coroutine currentTransition = null;

    // TODO: this isn't very performant...
    private (Vector3, Vector3)[] Sides { get {
            var sides = new (Vector3, Vector3)[numSides];
            for (int i = 0; i < numSides; ++i)
                sides[i] = (points[i], points[(i + 1) % numSides]);
            return sides;
        } }

    private class Vector3C
    {
        public Vector3 vector3;
    }

    private void Start()
    {
        // determine points for given sides and radius
        sideCollapsed = new bool[numSides];
        numActiveSides = numSides;

        // set each point to the origin
        points = new Vector3[numSides];
        for (int i = 0; i > numSides; ++i)
            points[i] = new Vector3();

        // find n-sided, regular polygon points
        polygonPoints = new List<Vector3C>();
        foreach (Vector3 point in Polygon.Points(numActiveSides, radius))
            polygonPoints.Add(new Vector3C { vector3 = point });

        // set point targets to reference polygon points
        pointTargets = new Vector3C[numSides];
        for (int i = 0; i < numSides; ++i)
            pointTargets[i] = polygonPoints[i];

        // transition immediately
        TransitionPoints();
    }

    // TODO: debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var randomSide = Random.Range(0, numSides);
            while (sideCollapsed[randomSide])
            {
                randomSide = Random.Range(0, numSides);
            }
            Debug.Log("Collapsing side " + randomSide);
            CollapseSide(randomSide);
        }
        DrawSides();
    }

    // TODO: debug
    private void DrawSides()
    {
        foreach (var side in Sides)
        {
            Debug.DrawLine(side.Item1, side.Item2);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < numSides; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(points[i], 0.5f);
            Handles.Label(points[i], "" + i);
        }
    }

    private void CollapseSide(int sideID)
    {
        // check if already collapsed
        if (sideCollapsed[sideID]) return;
        sideCollapsed[sideID] = true;
        numActiveSides--;

        // remove target that right point references
        var rightTargetIndex = (sideID + 1) % pointTargets.Length;

        // TODO: debug
        Debug.Log("Before removal: " + polygonPoints.Count);

        var pointToRemove = pointTargets[rightTargetIndex];

        pointTargets[rightTargetIndex] = pointTargets[sideID];
        for (int i = 1; i < pointTargets.Length; i++)
        {
            if (pointTargets[(rightTargetIndex + i) % pointTargets.Length] == pointToRemove)
            {
                pointTargets[(rightTargetIndex + i) % pointTargets.Length] = pointTargets[sideID];
            }
            else
            {
                break;
            }
        }
        polygonPoints.Remove(pointToRemove);

        // TODO: debug
        Debug.Log("After removal: " + polygonPoints.Count);

        // set right target to point at left target
        

        // update polygon point values
        var nextPolygonPoints = Polygon.Points(numActiveSides, radius);

        // TODO: debug
        Debug.Log("polygonPoints.Count: " + polygonPoints.Count);
        Debug.Log("nextPolygonPoints.Length: " + nextPolygonPoints.Length);

        for (int i = 0; i < polygonPoints.Count; ++i)
            polygonPoints[i].vector3 = nextPolygonPoints[i];    // TODO: does this work?

        // transition over time
        TransitionPoints(overTime: transitionTime);
    }

    private void TransitionPoints(float overTime = 0f)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentTransition = StartCoroutine(TransitionPointsCoroutine(Time.time, (Vector3[]) points.Clone(), overTime));
    }

    private IEnumerator TransitionPointsCoroutine(float startTime, Vector3[] startPositions, float overTime = 0f)
    {
        float transition = 0f;
        float elapsedTime = 0f;
        float t;

        while (transition < 1f)
        {
            t = overTime > 0f ? elapsedTime / overTime : 1f;
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // smooth stop

            // find new position
            for (int i = 0; i < points.Length; ++i)
                points[i] = Vector3.Lerp(startPositions[i], pointTargets[i].vector3, transition).normalized * radius;

            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        currentTransition = null;
    }
}
