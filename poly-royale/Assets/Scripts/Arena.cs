﻿using System;
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

    private Coroutine currentTransition = null;

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
            ball.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 10f;
        }
    }

    private void SetGoalTransforms(bool lerp = false)
    {
        if (transitionTime <= 0f || !lerp)
        {
            for (int i = 0; i < goals.Count; ++i)
            {
                goals[i].SetBounds(polygon.positions[i], polygon.positions[(i + 1) % polygon.positions.Length]);
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
    }

    private IEnumerator TransitionGoals(float startTime, Vector2[] startLeftPositions, Vector2[] startRightPositions)
    {
        float transition = 0f;
        float elapsedTime = 0f;

        Vector3 leftPos;
        Vector3 rightPos;

        while (transition < 1f)
        {
            transition = Mathf.Clamp(elapsedTime / transitionTime, 0f, 1f);
            for (int i = 0; i < goals.Count; ++i)
            {
                leftPos = Vector2.Lerp(startLeftPositions[i], polygon.positions[i], transition);
                rightPos = Vector2.Lerp(startRightPositions[i], polygon.positions[(i + 1) % goals.Count], transition);
                goals[i].SetBounds(leftPos, rightPos);
                
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
            UnityEngine.Application.Quit();
        }
        else
        {
            if (currentTransition != null)
                StopCoroutine(currentTransition);

            // TODO: this is temporary, should have some effect
            Destroy(ball.transform.gameObject);

            goals.Remove(goal);
            Destroy(goal.transform.gameObject);

            polygon = new Polygon(goals.Count, sideLength);
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
