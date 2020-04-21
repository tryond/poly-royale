using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public static GoalManager current;

    [SerializeField] private int numPlayers;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject playerPaddlePrefab;
    [SerializeField] private GameObject enemyPaddlePrefab;

    private GameObject[] goals;

    public event Action<GameObject> GoalEliminatedEvent;
    public void OnGoalEliminated(GameObject goal) => GoalEliminatedEvent?.Invoke(goal);

    public event Action<GameObject> BallScoredEvent;
    public void OnBallScored(GameObject ball) => BallScoredEvent?.Invoke(ball);

    private void Awake()
    {
        // set singleton object
        current = this;

        // instantiate all goals
        goals = new GameObject[numPlayers];
        for (int i = 0; i < numPlayers; ++i)
        {
            goals[i] = Instantiate(goalPrefab);
            goals[i].GetComponent<Goal>().id = i;
        }

        // set player paddle if one passed in
        goals[0].GetComponent<Goal>().Paddle = Instantiate(playerPaddlePrefab == null ? enemyPaddlePrefab : playerPaddlePrefab);
        foreach (var goal in goals)
            goal.GetComponent<Goal>().Paddle = Instantiate(enemyPaddlePrefab);

        // listen to all goals
        foreach (var goal in goals)
            goal.GetComponent<Goal>().GoalScoredEvent += GoalScored;
    }

    // TODO: debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var randomGoal = UnityEngine.Random.Range(0, numPlayers);
            OnGoalEliminated(goals[randomGoal]);
            
        }
    }

    public void SetGoalPositions((Vector3 left, Vector3 right)[] positions)
    {
        for (int i = 0; i < numPlayers; ++i)
        {
            goals[i].GetComponent<Goal>().SetBounds(positions[i].left, positions[i].right);
        }
    }

    public void GoalScored(GameObject goal, GameObject ball)
    {
        if (goal.GetComponent<Goal>().Paddle.CompareTag("Player"))
        {
            // quit the application
            //UnityEngine.Application.Quit();

            // reset the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            goal.GetComponent<Goal>().Deactivate();

            // notify listeners
            OnGoalEliminated(goal);
        }

        // notify listeners
        OnBallScored(ball);
    }
}
