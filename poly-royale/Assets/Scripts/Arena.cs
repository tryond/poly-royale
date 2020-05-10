using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour
{
    [SerializeField] float startTransitionTime = 0.5f;
    [SerializeField] private float endTransitionTime = 3f;
    
    [SerializeField] int numPlayers;
    [SerializeField] float radius;
    
    [SerializeField] private Player enemyPlayerPrefab;

    [SerializeField] private Boundary boundaryPrefab;
    private List<Boundary> boundaries;
    
    public Player mainPlayer;
    private List<Player> players = new List<Player>();
    
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
        players = new List<Player>();
        
        // add player (if defined) and enemy goals
        players.Add(mainPlayer.gameObject.activeSelf ? mainPlayer : Instantiate(enemyPlayerPrefab));
        for (int i = 1; i < numPlayers; i++)
            players.Add(Instantiate(enemyPlayerPrefab));

        // listen to all goals
        foreach (Player p in players)
            p.OnPlayerEliminated += GoalScored;
        
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

        currentGoalTransition = StartCoroutine(TransitionPlayers(overTime));
    }

    private IEnumerator TransitionPlayers(float overTime = 0f)
    {
        // notify listeners
        OnTransitionStart?.Invoke();
        
        (Vector3 left, Vector3 right)[] startPositions = new (Vector3, Vector3)[players.Count];
        for (int i = 0; i < players.Count; i++)
            startPositions[i] = (players[i].LeftBound, players[i].RightBound);
        
        float transition = 0f;
        float elapsedTime = 0f;
        
        while (transition < 1f)
        {
            var t = overTime > 0f ? elapsedTime / overTime : 1f;
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // smooth stop
            
            // determine new goal positions first
            for (int i = 0; i < players.Count; ++i)
            {
                var leftPoint = Vector3.Lerp(startPositions[i].left, polygon.Positions[i].left, transition).normalized * radius;
                var rightPoint = Vector3.Lerp(startPositions[i].right, polygon.Positions[i].right, transition).normalized * radius;
                players[i].SetBounds(leftPoint, rightPoint);
            }
            
            // set boundaries
            for (int i = 0; i < boundaries.Count; i++)
            {
                boundaries[i].SetBounds(
                    players[i % players.Count].RightBound,
                    players[(i + 1) % players.Count].LeftBound);
            }
        
            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentGoalTransition = null;
        
        // notify listeners
        OnTransitionEnd?.Invoke();
    }

    public void GoalScored(Player player, Ball ball)
    {
        // stop current transition
        if (currentGoalTransition != null)
            StopCoroutine(currentGoalTransition);

        // destroy the ball
        ballManager.Remove(ball.gameObject);

        // destroy the goal
        if (players.Remove(player))
        {
            // display score if player out
            if (player.CompareTag("Player") || players.Count <= 1)
                Canvas.instance.DisplayScore(playersRemaining: players.Count, playersTotal: numPlayers);
            
            Destroy(player.gameObject);
            
            var boundary = boundaries[0];
            boundaries.RemoveAt(0);
            Destroy(boundary.gameObject);
        }

        // transition sectors
        polygon = new Polygon(players.Count, radius);

        print("Ratio: " + players.Count / numPlayers);
        
        var time = Mathf.Lerp(startTransitionTime, endTransitionTime, 1.0f - ((float) players.Count / numPlayers));
        print("Overt Time: " + time);
        SetGoalPositions(overTime: time);
    }
}
