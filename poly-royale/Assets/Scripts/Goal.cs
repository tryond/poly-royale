using UnityEngine;

public class Goal : Side
{
    private Sector sector;

    private void Start() => sector = GetComponentInParent<Sector>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var ball = collision.GetComponent<Ball>();
        if (ball)
        {
            // TODO: reflect ball first
            sector.GoalScored(collision.GetComponent<Ball>());
        }
    }
}
