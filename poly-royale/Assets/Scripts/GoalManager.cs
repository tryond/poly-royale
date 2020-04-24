using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public static GoalManager current;

    [SerializeField] private int numPlayers;
    [SerializeField] private GameObject playerGoalPrefab;
    [SerializeField] private GameObject enemyGoalPrefab;

    private GameObject playerGoal;

    private GameObject[] goals;
    private bool[] goalEliminated;

    public event Action<GameObject> GoalEliminatedEvent;
    public void OnGoalEliminated(GameObject goal) => GoalEliminatedEvent?.Invoke(goal);

    public event Action<GameObject> BallScoredEvent;
    public void OnBallScored(GameObject ball) => BallScoredEvent?.Invoke(ball);

    private void Awake()
    {
        // set singleton object
        current = this;

        // create player goal if set in editor
        goals = new GameObject[numPlayers];

        if (playerGoalPrefab != null)
        {
            playerGoal = Instantiate(playerGoalPrefab);
            goals[0] = playerGoal;
        }
        else
        {
            playerGoal = null;
            goals[0] = Instantiate(enemyGoalPrefab);
        }

        goalEliminated = new bool[numPlayers];

        // create enemy goals
        for (int i = 1; i < numPlayers; ++i)
        {
            goals[i] = Instantiate(enemyGoalPrefab);
            goals[i].GetComponent<Goal>().id = i;
        }

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
            while (goalEliminated[randomGoal])
                randomGoal = UnityEngine.Random.Range(0, numPlayers);

            GoalScored(goals[randomGoal], null);
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
        if (goal.GetComponent<Goal>().CompareTag("Player"))
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

        goalEliminated[goal.GetComponent<Goal>().id] = true;

        // notify listeners
        OnBallScored(ball);
    }

    public GameObject GetPlayerGoal()
    {
        return playerGoal;
    }
}
