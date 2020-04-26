using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;

public class Arena : MonoBehaviour
{
    [SerializeField] float transitionTime = 0.5f;
    [SerializeField] int numPlayers;
    [SerializeField] float radius;
    [SerializeField] float ballToSideRatio;
    
    [SerializeField] private Goal playerGoalPrefab;
    [SerializeField] private Goal enemyGoalPrefab;

    [SerializeField] private Boundary boundaryPrefab;
    private List<Boundary> boundaries;
    
    private Goal playerGoal;
    private List<Goal> goals = new List<Goal>();
    
    [SerializeField] private BallManager ballManager;

    private Polygon polygon;
    private Coroutine currentGoalTransition = null;

    private void Start()
    {
        // create polygon from which to find sector positions
        polygon = new Polygon(numPlayers, radius);
        
        // create goals
        goals = new List<Goal>();
        
        // add player (if defined) and enemy goals
        goals.Add(Instantiate(playerGoalPrefab != null ? playerGoalPrefab : enemyGoalPrefab));
        for (int i = 1; i < numPlayers; i++)
            goals.Add(Instantiate(enemyGoalPrefab));

        // listen to all goals
        foreach (Goal goal in goals)
            goal.OnGoalScored += GoalScored;
        
        // create boundaries
        boundaries = new List<Boundary>();
        for (int i = 0; i < numPlayers; i++)
            boundaries.Add(Instantiate(boundaryPrefab));
        
        // set goal positions
        SetGoalPositions(overTime: 0f);
    }

    private void SetGoalPositions(float overTime = 0f)
    {
        if (currentGoalTransition != null)
            StopCoroutine(currentGoalTransition);

        currentGoalTransition = StartCoroutine(TransitionGoals(overTime));
    }

    private IEnumerator TransitionGoals(float overTime = 0f)
    {
        (Vector3 left, Vector3 right)[] startPositions = new (Vector3, Vector3)[goals.Count];
        for (int i = 0; i < goals.Count; i++)
            startPositions[i] = (goals[i].leftBound.transform.position, goals[i].rightBound.transform.position);

        float transition = 0f;
        float elapsedTime = 0f;

        while (transition < 1f)
        {
            var t = overTime > 0f ? elapsedTime / overTime : 1f;
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // smooth stop
            
            // determine new goal positions first
            for (int i = 0; i < goals.Count; ++i)
            {
                var leftPoint = Vector3.Lerp(startPositions[i].left, polygon.Positions[i].left, transition).normalized * radius;
                var rightPoint = Vector3.Lerp(startPositions[i].right, polygon.Positions[i].right, transition).normalized * radius;
                goals[i].SetBounds(leftPoint, rightPoint);
            }
            
            // set boundaries
            for (int i = 0; i < boundaries.Count; i++)
            {
                boundaries[i].SetBounds(
                    goals[i % goals.Count].rightBound.transform.position,
                    goals[(i + 1) % goals.Count].leftBound.transform.position);
            }

            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentGoalTransition = null;
    }

    public void GoalScored(GameObject goal, GameObject ball)
    {
        if (goal.CompareTag("Player"))
        {
            // quit the application
            //UnityEngine.Application.Quit();

            // reset the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // stop current sector transition
            if (currentGoalTransition != null)
                StopCoroutine(currentGoalTransition);

            // destroy the ball
            ballManager.Remove(ball);

            // destroy the sector
            if (goals.Remove(goal.GetComponent<Goal>()))
            {
                Destroy(goal.gameObject);
                
                var boundary = boundaries[0];
                boundaries.RemoveAt(0);
                Destroy(boundary.gameObject);
            }
            
            // reset if only one player remaining
            if (goals.Count <= 1)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
            // transition sectors
            polygon = new Polygon(goals.Count, radius);
            SetGoalPositions(overTime: transitionTime);
        }
    }
}
