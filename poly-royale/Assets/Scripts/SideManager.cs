using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideManager : MonoBehaviour
{
    public static SideManager current;

    [Tooltip("number of sides to start with")] public int numSides = 0;
    [Tooltip("radius of polygon formed by sides")] public float radius;
    [Tooltip("time to transition between side layouts")] public float transitionTime;

    private Vector3[] points;
    private List<Target> polygonPoints;
    private Target[] pointTargets;
    //private (Vector3, Vector3)[] sides;
    private bool[] sideCollapsed;
    private int numActiveSides;
    private Coroutine currentTransition = null;

    public event Action PositionChangeEvent;
    public void PositionChange() => PositionChangeEvent?.Invoke();

    private class Target { public Vector3 position; }

    // TODO: this isn't very performant...
    public (Vector3, Vector3)[] Sides { get {
            var sides = new (Vector3, Vector3)[numSides];
            for (int i = 0; i < numSides; ++i)
                sides[i] = (points[i], points[(i + 1) % numSides]);
            return sides;
        } }

    private void Awake()
    {
        current = this;
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
        polygonPoints = new List<Target>();
        foreach (Vector3 point in Polygon.Points(numActiveSides, radius))
            polygonPoints.Add(new Target { position = point });

        // set point targets to reference polygon points
        pointTargets = new Target[numSides];
        for (int i = 0; i < numSides; ++i)
            pointTargets[i] = polygonPoints[i];

        // transition immediately
        TransitionPoints();
    }

    // TODO: debug
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    var randomSide = UnityEngine.Random.Range(0, numSides);
        //    while (sideCollapsed[randomSide])
        //    {
        //        randomSide = UnityEngine.Random.Range(0, numSides);
        //    }
        //    CollapseSide(randomSide);
        //}
        DrawSides();
    }

    // TODO: debug
    private void DrawSides()
    {
        var sides = Sides;
        for (int i = 0; i < numSides; ++i)
        {
            var sideColor = sideCollapsed[i] ? Color.red : Color.green;
            Debug.DrawLine(sides[i].Item1, sides[i].Item2, sideColor);
            Debug.DrawLine(sides[i].Item1, transform.position, Color.gray);
        }

    }

    public void CollapseSide(int sideID)
    {
        // check if already collapsed
        if (sideCollapsed[sideID]) return;
        sideCollapsed[sideID] = true;
        numActiveSides--;

        // remove target that right point references
        var rightTargetIndex = (sideID + 1) % pointTargets.Length;
        var pointToRemove = pointTargets[rightTargetIndex];

        // update all targets to point to correct point
        pointTargets[rightTargetIndex] = pointTargets[sideID];
        for (int i = 1; i < pointTargets.Length; i++)
        {
            var nextTarget = (rightTargetIndex + i) % pointTargets.Length;
            if (pointTargets[nextTarget] != pointToRemove)
                break;

            pointTargets[nextTarget] = pointTargets[sideID];
        }
        polygonPoints.Remove(pointToRemove);

        // update polygon point values
        var nextPolygonPoints = Polygon.Points(numActiveSides, radius);

        for (int i = 0; i < polygonPoints.Count; ++i)
            polygonPoints[i].position = nextPolygonPoints[i];

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
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // ease out
            //transition = Mathf.Clamp(t * t * t, 0f, 1f);  // ease in

            // find new position
            for (int i = 0; i < points.Length; ++i)
                points[i] = Vector3.Lerp(startPositions[i], pointTargets[i].position, transition).normalized * radius;

            // notify listeners
            PositionChange();

            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        currentTransition = null;
    }
}
