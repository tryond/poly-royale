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

    private Goal playerGoal;
    [SerializeField] Goal playerGoalPrefab;
    [SerializeField] Goal enemyGoalPrefab;
    private List<Goal> goals = new List<Goal>();

    [SerializeField] Wall wallPrefab;
    private List<Wall> walls = new List<Wall>();

    [SerializeField] Ball ballPrefab;
    // TODO: add active balls to list
    //private List<Ball> balls;

    void Awake()
    {
        polygon = new Polygon(numPlayers, radius);

        playerGoal = Instantiate(playerGoalPrefab, Vector3.zero, Quaternion.identity);
        playerGoal.transform.parent = gameObject.transform;
        goals.Add(playerGoal);

        Goal enemy;
        for (int i = 1; i < numPlayers; ++i)
        {
            enemy = Instantiate(enemyGoalPrefab, Vector3.zero, Quaternion.identity);
            enemy.transform.parent = gameObject.transform;
            goals.Add(enemy);
        }

        for (int i = 0; i < numPlayers; ++i)
        {
            walls.Add(Instantiate(wallPrefab, Vector3.zero, Quaternion.identity));
        }

        SetGoalTransforms();
    }

    // TODO: needs work...
    void Start()
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

    private void SetGoalTransforms(bool lerp = false)
    {
        if (transitionTime <= 0f || !lerp)
        {
            for (int i = 0; i < goals.Count; ++i)
            {
                goals[i].SetBounds(polygon.positions[i], polygon.positions[(i + 1) % polygon.positions.Length]);
                walls[i].SetBounds(goals[i].rightBound.transform.position, goals[i].rightBound.transform.position);
            }
        }
        else
        {
            var startLeftPositions = new Vector2[goals.Count];
            var startRightPositions = new Vector2[goals.Count];

            for (int i = 0; i < goals.Count; ++i)
            {
                startLeftPositions[i] = goals[i].leftBound.transform.position;
                startRightPositions[i] = goals[i].rightBound.transform.position;

            }
            currentTransition = StartCoroutine(TransitionGoals(Time.time, startLeftPositions, startRightPositions));
        }

        // TODO: need to lerp this...
        //for (int i = 0; i < balls.Count; ++i)
        //{
        //    //balls[i].transform.localScale = new Vector3(polygon.GetSideLength() * ballToSideRatio, polygon.GetSideLength() * ballToSideRatio, 1f);
        //    balls[i].transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        //}
    }

    private IEnumerator TransitionGoals(float startTime, Vector2[] startLeftPositions, Vector2[] startRightPositions)
    {
        float transition = 0f;
        float elapsedTime = 0f;

        Vector3 leftPos;
        Vector3 rightPos;

        while (transition < 1f)
        {

            var t = elapsedTime / transitionTime;
            // smooth stop
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);

            // set goal positions first
            for (int i = 0; i < goals.Count; ++i)
            {
                leftPos = Vector2.Lerp(startLeftPositions[i], polygon.positions[i], transition);
                rightPos = Vector2.Lerp(startRightPositions[i], polygon.positions[(i + 1) % goals.Count], transition);
                goals[i].SetBounds(leftPos, rightPos);
                
            }

            // then set wall positions
            for (int i = 0; i < walls.Count; ++i)
            {
                walls[i].SetBounds(goals[i].rightBound.transform.position, goals[(i + 1) % goals.Count].leftBound.transform.position);
            }

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentTransition = null;
    }

    public void GoalScored(Goal goal, Ball ball)
    {
        if (goal == playerGoal)
        {
            //UnityEngine.Application.Quit();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            if (currentTransition != null)
                StopCoroutine(currentTransition);

            // TODO: this is temporary, should have some effect
            //balls.Remove(ball);
            Destroy(ball.transform.gameObject);

            goals.Remove(goal);
            Destroy(goal.transform.gameObject);

            var destroyWall = walls[0];
            walls.RemoveAt(0);
            Destroy(destroyWall.transform.gameObject);

            polygon = new Polygon(goals.Count, radius);
            SetGoalTransforms(true);
        }
    }


    private void Update()
    {
        for (int i = 0; i < goals.Count; ++i)
        {
            Debug.DrawLine(goals[i].rightBound.transform.position, goals[(i + 1) % goals.Count].leftBound.transform.position, Color.yellow);
        }
    }

}
