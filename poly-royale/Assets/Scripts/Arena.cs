using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

//[ExecuteInEditMode]

public class Arena : MonoBehaviour
{
    [SerializeField] float transitionTime = 0.5f;

    [SerializeField] int numPlayers;
    [SerializeField] int numBalls;

    [SerializeField] float sideLength;

    [SerializeField] Goal playerGoalPrefab;
    [SerializeField] Goal enemyGoalPrefab;
    [SerializeField] Ball ballPrefab;

    private Goal playerGoal;
    private List<Goal> goals = new List<Goal>();

    private Polygon polygon;

    void Awake()
    {
        polygon = new Polygon(numPlayers, sideLength);

        playerGoal = Instantiate(playerGoalPrefab, Vector3.zero, Quaternion.identity);
        playerGoal.transform.localScale = new Vector3(sideLength, sideLength / 3f, 1f); // TODO: where should this be set?
        playerGoal.transform.parent = gameObject.transform;
        goals.Add(playerGoal);

        Goal enemy;
        for (int i = 1; i < numPlayers; ++i)
        {
            enemy = Instantiate(enemyGoalPrefab, Vector3.zero, Quaternion.identity);
            enemy.transform.localScale = new Vector3(sideLength, sideLength / 3f, 1f);  // TODO: where should this be set?
            enemy.transform.parent = gameObject.transform;
            goals.Add(enemy);
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
            ball.transform.localScale = new Vector3(0.5f, 0.5f, 1f);    // TODO: where should this be set?
            ball.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 5f;
        }
    }

    private void SetGoalTransforms(bool lerp = false)
    {
        if (transitionTime <= 0f || !lerp)
        {
            for (int i = 0; i < goals.Count; ++i)
            {
                goals[i].transform.position = polygon.positions[i];
                goals[i].transform.rotation = polygon.rotations[i];
            }
        }
        else
        {
            var startPositions = new Vector3[goals.Count];
            var startRotations = new Quaternion[goals.Count];

            for (int i = 0; i < goals.Count; ++i)
            {
                startPositions[i] = goals[i].transform.position;
                startRotations[i] = goals[i].transform.rotation;
            }

            StartCoroutine(TransitionGoals(Time.time, startPositions, startRotations));
        }
    }

    private IEnumerator TransitionGoals(float startTime, Vector3[] startPositions, Quaternion[] startRotations)
    {
        float transition = 0f;
        float elapsedTime = 0f;
        while (transition < 1f)
        {
            transition = Mathf.Clamp(elapsedTime / transitionTime, 0f, 1f);
            for (int i = 0; i < goals.Count; ++i)
            {
                goals[i].transform.position = Vector3.Lerp(startPositions[i], polygon.positions[i], transition);
                goals[i].transform.rotation = Quaternion.Lerp(startRotations[i], polygon.rotations[i], transition);
            }
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void GoalScored(Goal goal)
    {
        if (goal == playerGoal)
        {
            UnityEngine.Application.Quit();
        }
        else
        {
            goals.Remove(goal);
            Destroy(goal.transform.gameObject);

            numPlayers -= 1;
            polygon = new Polygon(numPlayers, sideLength);
            SetGoalTransforms(true);
        }
    }
}
