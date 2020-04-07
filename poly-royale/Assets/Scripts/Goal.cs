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

    public void SetLeftBound(Vector2 position)
    {
        SetBounds(position, rightBound.transform.position);
    }

    public void SetRightBound(Vector2 position)
    {
        SetBounds(leftBound.transform.position, position);
    }

    public void SetBounds(Vector2 leftPosition, Vector2 rightPosition)
    {
        // set position
        transform.position = new Vector3((leftPosition.x + rightPosition.x) / 2, (leftPosition.y + rightPosition.y) / 2, 0f);

        // set rotation
        Vector2 perpendicular = Vector2.Perpendicular(rightPosition - leftPosition);
        transform.up = perpendicular;

        // set scale
        var oldDistance = Vector3.Distance(rightBound.transform.position, leftBound.transform.position);
        var newDistance = Vector2.Distance(rightPosition, leftPosition);
        transform.localScale *= newDistance / oldDistance;

        // set bound positions
        leftBound.transform.position = leftPosition;
        rightBound.transform.position = rightPosition;
    }
}
