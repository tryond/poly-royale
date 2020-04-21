using UnityEngine;
using System;

public class Goal : Side
{
    public int id;

    public event Action<GameObject, GameObject> GoalScoredEvent;
    public void OnGoalScored(GameObject goal, GameObject ball) => GoalScoredEvent?.Invoke(goal, ball);

    public GameObject Paddle {
        set { Paddle = value; Paddle.transform.parent = this.transform; }
        get { return Paddle; }
    }

    public void Deactivate()
    {
        Paddle.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ball"))
            OnGoalScored(this.gameObject, collision.gameObject);
    }
}
