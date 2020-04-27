﻿using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Boundary : Side
{
    private void Start()
    {
        if (lineRenderer != null)
            lineRenderer.startColor = Color.yellow;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            var ball = other.GetComponent<Ball>();
            ball.SetVelocity(ball.speed, Vector3.Reflect(other.transform.up, transform.up));
        }
    }

    private void Update()
    {
        Debug.DrawLine(leftBound.transform.position, rightBound.transform.position, Color.yellow);
    }
}
