using UnityEngine;

public class Goal : Side
{
    private Arena arena;

    private void Start()
    {
        arena = GetComponentInParent<Arena>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var ball = collision.GetComponent<Ball>();
        if (ball)
        {
            // TODO: reflect ball first
            arena.GoalScored(this, collision.GetComponent<Ball>());
        }
    }
}
