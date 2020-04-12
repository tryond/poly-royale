using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

//[ExecuteInEditMode]

public class Arena : MonoBehaviour
{
    private Polygon polygon;
    private Coroutine currentTransition = null;
    [SerializeField] float transitionTime = 0.5f;

    [SerializeField] int numPlayers;
    [SerializeField] int numBalls;
    [SerializeField] float radius;
    [SerializeField] float ballToSideRatio;

    private Sector playerSector;
    [SerializeField] Sector playerSectorPrefab;
    [SerializeField] Sector enemySectorPrefab;
    private List<Sector> sectors = new List<Sector>();

    [SerializeField] Ball ballPrefab;
    // TODO: add active balls to list
    //private List<Ball> balls;

    void Awake()
    {
        // create polygon from which to find sector positions
        polygon = new Polygon(numPlayers, radius);

        // instantiate enemy secors  
        playerSector = Instantiate(playerSectorPrefab, Vector3.zero, Quaternion.identity);
        playerSector.transform.parent = gameObject.transform;
        sectors.Add(playerSector);

        // instantiate enemy sectors
        Sector enemySector;
        for (int i = 1; i < numPlayers; ++i)
        {
            enemySector = Instantiate(enemySectorPrefab, Vector3.zero, Quaternion.identity);
            enemySector.transform.parent = gameObject.transform;
            sectors.Add(enemySector);
        }

        SetSectorsTransform();
    }

    void Start()
    {
        LaunchBalls(numBalls, true);
    }

    // TODO: this need much improvement
    private void LaunchBalls(int numBalls, bool uniform = true)
    {
        Ball ball;
        for (int i = 0; i < numBalls; ++i)
        {
            ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            //ball.transform.localScale = new Vector3(polygon.GetSideLength() * ballToSideRatio, polygon.GetSideLength() * ballToSideRatio, 1f);
            ball.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
            ball.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 10f;
            //balls.Add(ball);
        }
    }

    private void SetSectorsTransform(bool lerp = false)
    {
        // move sectors immediately
        if (transitionTime <= 0f || !lerp)
        {
            for (int i = 0; i < sectors.Count; ++i)
            {
                sectors[i].SetSectorPoints(
                    polygon.positions[i],
                    polygon.positions[(i + 1) % polygon.positions.Length],
                    polygon.positions[(i + 1) % polygon.positions.Length]);
            }
        }
        // lerp sector transforms
        else
        {
            var startLeftPoints = new Vector2[sectors.Count];
            var startRightPoints = new Vector2[sectors.Count];
            var startAttachPoints = new Vector2[sectors.Count];

            for (int i = 0; i < sectors.Count; ++i)
            {
                startLeftPoints[i] = sectors[i].LeftPoint;
                startRightPoints[i] = sectors[i].RightPoint;
                startAttachPoints[i] = sectors[i].AttachPoint;

            }
            currentTransition = StartCoroutine(TransitionSectors(Time.time, startLeftPoints, startRightPoints, startAttachPoints));
        }
    }

    private IEnumerator TransitionSectors(float startTime, Vector2[] startLeftPoints, Vector2[] startRightPoints, Vector2[] startAttachPoints)
    {
        float transition = 0f;
        float elapsedTime = 0f;

        while (transition < 1f)
        {
            var t = elapsedTime / transitionTime;
            // smooth stop
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);

            // determine new goal positions first
            var leftPoints = new Vector2[sectors.Count];
            var rightPoints = new Vector2[sectors.Count];
            for (int i = 0; i < sectors.Count; ++i)
            {
                leftPoints[i] = Vector2.Lerp(startLeftPoints[i], polygon.positions[i], transition).normalized * radius;
                rightPoints[i] = Vector2.Lerp(startRightPoints[i], polygon.positions[(i + 1) % sectors.Count], transition).normalized * radius;
            }

            // determine attach points
            var attachPoints = new Vector2[sectors.Count];
            for (int i = 0; i < sectors.Count; ++i)
            {
                attachPoints[i] = leftPoints[(i + 1) % sectors.Count];
            }

            // set the new positions
            for (int i = 0; i < sectors.Count; ++i)
            {
                sectors[i].SetSectorPoints(leftPoints[i], rightPoints[i], attachPoints[i]);
            }

            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentTransition = null;
    }

    public void GoalScored(Sector sector, Ball ball)
    {
        if (sector == playerSector)
        {
            // quit the application
            //UnityEngine.Application.Quit();

            // reset the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            if (currentTransition != null)
                StopCoroutine(currentTransition);
            
            Destroy(ball.transform.gameObject);

            // if the wall were associated with the goal, it wouldn't matter...
            sectors.Remove(sector);
            Destroy(sector.transform.gameObject);

            polygon = new Polygon(sectors.Count, radius);
            SetSectorsTransform(true);
        }
    }
}
