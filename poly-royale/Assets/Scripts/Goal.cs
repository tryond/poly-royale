using UnityEngine;
using System;

[RequireComponent(typeof(Paddle))]
public class Goal : Side
{
    public int id;
    public event Action<GameObject, GameObject> GoalScoredEvent;
    public void OnGoalScored(GameObject goal, GameObject ball) => GoalScoredEvent?.Invoke(goal, ball);

    public void Deactivate()
    {
        //var paddle = gameObject.GetComponent<Paddle>();
        //if (paddle != null)
        //    paddle.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            OnGoalScored(this.gameObject, collision.gameObject);
    }
}
