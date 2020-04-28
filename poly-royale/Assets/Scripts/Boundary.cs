using System;
using UnityEngine;

public class Boundary : Side
{
    private void Start()
    {
        lineRenderer.material.color = Color.yellow;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            var ball = other.GetComponent<Ball>();
            ball.SetVelocity(ball.speed, Vector3.Reflect(other.transform.up, transform.up));
        }
    }

    // private void Update()
    // {
    //     Debug.DrawLine(leftBound.transform.position, rightBound.transform.position, Color.yellow);
    // }
}
