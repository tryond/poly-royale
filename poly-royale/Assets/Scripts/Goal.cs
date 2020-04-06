using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private Arena arena;

    public GameObject leftBound;
    public GameObject rightBound;

    private void Start()
    {
        arena = GetComponentInParent<Arena>();
    }

    private void Update()
    {
        Debug.DrawLine(leftBound.transform.position, rightBound.transform.position, Color.cyan);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        var ball = collision.GetComponent<Ball>();
        if (ball)
            arena.GoalScored(this, collision.GetComponent<Ball>());
    }
}
