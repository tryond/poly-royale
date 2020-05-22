using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider2D))]
public class Boundary : Side
{
    protected override void Awake()
    {
        base.Awake();
        lineRenderer.material.color = Color.yellow;
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ball"))
    //     {
    //         var ball = other.GetComponent<Ball>();
    //         ball.SetVelocity(ball.speed, Vector3.Reflect(other.transform.up, transform.up));
    //     }
    // }
}
