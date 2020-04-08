using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : Section
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
            arena.GoalScored(this, collision.GetComponent<Ball>());
    }
}
