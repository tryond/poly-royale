using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour
{
    private Polygon polygon;
    private Coroutine currentSectorTransition = null;
    private Coroutine currentBallTransition = null;

    [SerializeField] float transitionTime = 0.5f;

    [SerializeField] int numPlayers;
    [SerializeField] int numBalls;
    [SerializeField] float radius;
    [SerializeField] float ballToSideRatio;
    [SerializeField] float ballSpeed;

    private Sector playerSector;
    [SerializeField] Sector playerSectorPrefab;
    [SerializeField] Sector enemySectorPrefab;
    private List<Sector> sectors = new List<Sector>();

    [SerializeField] Ball ballPrefab;
    private List<Ball> balls = new List<Ball>();

    private void Awake()
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

    private void Start()
    {
        LaunchBalls(numBalls, true);
    }

    private void LaunchBalls(int numBalls, bool uniform = true)
    {
        Ball ball;
        Vector2 baseVector = new Vector2(0f, -1f) * ballSpeed;

        if (uniform)
        {
            // pick a random starting angle
            var theta = 360f / numBalls;
            var variance = theta / 2f;

            // launch balls uniformally around arena
            for (int i = 0; i < numBalls; ++i)
            {
                ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
                balls.Add(ball);

                //ball.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                ball.velocity = Quaternion.Euler(0f, 0f, (theta * i) + Random.Range(-variance, variance)) * baseVector;
            }
        }
        else
        {
            // launch balls in random directions
            for (int i = 0; i < numBalls; ++i)
            {
                ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
                balls.Add(ball);

                ball.velocity = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) * baseVector;
            }
        }

        ScaleBalls(false);
    }

    private void SetSectorsTransform(bool lerp = false)
    {
        // move sectors immediately
        if (transitionTime <= 0f || !lerp)
        {
            for (int i = 0; i < sectors.Count; ++i)
            {
                sectors[i].SetSectorPoints(
                    polygon.Positions[i].left,
                    polygon.Positions[i].right,
                    polygon.Positions[i].right);
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
            currentSectorTransition = StartCoroutine(TransitionSectors(Time.time, startLeftPoints, startRightPoints, startAttachPoints));
        }
    }

    private IEnumerator TransitionSectors(float startTime, Vector2[] startLeftPoints, Vector2[] startRightPoints, Vector2[] startAttachPoints)
    {
        float transition = 0f;
        float elapsedTime = 0f;

        while (transition < 1f)
        {
            var t = elapsedTime / transitionTime;
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // smooth stop

            // determine new goal positions first
            var leftPoints = new Vector2[sectors.Count];
            var rightPoints = new Vector2[sectors.Count];
            for (int i = 0; i < sectors.Count; ++i)
            {
                leftPoints[i] = Vector2.Lerp(startLeftPoints[i], polygon.Positions[i].left, transition).normalized * radius;
                rightPoints[i] = Vector2.Lerp(startRightPoints[i], polygon.Positions[i].right, transition).normalized * radius;
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
        currentSectorTransition = null;
    }

    private void ScaleBalls(bool lerp = false)
    {
        var newScale = polygon.SideLength * ballToSideRatio;

        if (balls.Count <= 0)
        {
            return;
        }
        // scale balls immediately
        else if (transitionTime <= 0f || !lerp)
        {
            foreach (Ball ball in balls)
            {
                ball.transform.localScale = Vector3.one * newScale;
            }
        }
        // lerp ball scale
        else
        {
            currentBallTransition = StartCoroutine(TransitionBalls(
                Time.time,
                balls[0].transform.localScale,
                Vector3.one * newScale));
        }
    }

    private IEnumerator TransitionBalls(float startTime, Vector3 startScale, Vector3 endScale)
    {
        float transition = 0f;
        float elapsedTime = 0f;

        float t;
        Vector3 lerpScale;

        while (transition < 1f)
        {
            t = elapsedTime / transitionTime;
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // smooth stop

            // set scale for each ball
            lerpScale = Vector3.Lerp(startScale, endScale, transition);
            foreach (Ball ball in balls)
                ball.transform.localScale = lerpScale;

            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentBallTransition = null;
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
            // stop current sector transition
            if (currentSectorTransition != null)
                StopCoroutine(currentSectorTransition);

            // destroy the ball
            balls.Remove(ball);
            Destroy(ball.transform.gameObject);

            // destroy the sector
            sectors.Remove(sector);
            Destroy(sector.transform.gameObject);

            // transition sectors
            polygon = new Polygon(sectors.Count, radius);
            SetSectorsTransform(true);

            // stop current ball transition
            if (currentBallTransition != null)
                StopCoroutine(currentBallTransition);

            // scale remaining balls
            ScaleBalls(true);
        }
    }
}
