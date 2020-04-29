using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;

public class Arena : MonoBehaviour
{
    [SerializeField] float startTransitionTime = 0.5f;
    [SerializeField] private float endTransitionTime = 3f;
    
    [SerializeField] int numPlayers;
    [SerializeField] float radius;
    [SerializeField] float ballToSideRatio;
    
    [SerializeField] private Goal enemyGoalPrefab;

    [SerializeField] private Boundary boundaryPrefab;
    private List<Boundary> boundaries;
    
    public Goal playerGoal;
    private List<Goal> goals = new List<Goal>();
    
    [SerializeField] private BallManager ballManager;

    private Polygon polygon;
    private Coroutine currentGoalTransition = null;

    public static Arena current;

    public event Action OnTransitionStart;
    public event Action OnTransitionEnd;
    
    
    private void Start()
    {
        current = this;
        
        // create polygon from which to find sector positions
        polygon = new Polygon(numPlayers, radius);
        
        // create goals
        goals = new List<Goal>();
        
        // add player (if defined) and enemy goals
        goals.Add(playerGoal.gameObject.activeSelf ? playerGoal : Instantiate(enemyGoalPrefab));
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

    // TODO: debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // reset the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void SetGoalPositions(float overTime = 0f)
    {
        if (currentGoalTransition != null)
            StopCoroutine(currentGoalTransition);

        currentGoalTransition = StartCoroutine(TransitionGoals(overTime));
    }

    private IEnumerator TransitionGoals(float overTime = 0f)
    {
        // notify listeners
        OnTransitionStart?.Invoke();
        
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
        
        // notify listeners
        OnTransitionEnd?.Invoke();
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

            print("Ratio: " + goals.Count / numPlayers);
            
            var time = Mathf.Lerp(startTransitionTime, endTransitionTime, 1.0f - ((float) goals.Count / numPlayers));
            print("Overt Time: " + time);
            SetGoalPositions(overTime: time);
        }
    }
}
