using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour
{
    [SerializeField] float transitionTime = 0.5f;
    [SerializeField] int numPlayers;
    [SerializeField] float radius;
    [SerializeField] float ballToSideRatio;
    [SerializeField] Sector playerSectorPrefab;
    [SerializeField] Sector enemySectorPrefab;

    [SerializeField] private BallManager ballManager;

    private Polygon polygon;
    private Coroutine currentSectorTransition = null;
    private Sector playerSector;
    private List<Sector> sectors = new List<Sector>();

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
        // Launch all balls
        ballManager.LaunchBalls(ballManager.NumBalls, overTime: 0f, random: true);
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

    public void GoalScored(Sector sector, GameObject ball)
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
            ballManager.Remove(ball);

            // destroy the sector
            sectors.Remove(sector);
            Destroy(sector.transform.gameObject);

            // transition sectors
            polygon = new Polygon(sectors.Count, radius);
            SetSectorsTransform(true);

            // TODO: broken
            // scale remaining balls
            //ballManager.ScaleBalls(ballToSideRatio / polygon.SideLength);
        }
    }
}
