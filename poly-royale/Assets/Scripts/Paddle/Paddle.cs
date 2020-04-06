﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] [Range(0f, 20f)] protected float speed = 10f;
    [SerializeField] [Range(0f, 90f)] float maxNormalAngle = 45f;
    [SerializeField] [Range(0f, 90f)] float maxReflectionAngle = 60f;

    private float paddleWidth;

    protected float leftBound;
    protected float rightBound;

    protected float movement; // between -1 and 1

    [SerializeField] Goal goal;

    private void Awake()
    {
        paddleWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        leftBound = goal.leftBound.transform.localPosition.x + (paddleWidth / transform.parent.localScale.x);
        rightBound = goal.rightBound.transform.localPosition.x - (paddleWidth / transform.parent.localScale.x);
    }

    public Vector3 GetNormalVector()
    {
        return Quaternion.Euler(0f, 0f, -maxNormalAngle * movement) * transform.up;
    }

    public Vector3 GetReflectionVector(Vector2 direction)
    {
        Vector3 rawReflection = Vector3.Reflect(direction, GetNormalVector());
        var difference = Vector3.SignedAngle(rawReflection, transform.up, transform.forward);
        difference = Mathf.Clamp(difference, -maxReflectionAngle, maxReflectionAngle);
        return Quaternion.Euler(0f, 0f, -difference) * transform.up;
    }

    protected void DrawNormals()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up * paddleWidth, Color.green);
        Debug.DrawLine(transform.position, transform.position + GetNormalVector() * paddleWidth, Color.red);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // get padddle normal
        Ball ball = other.gameObject.GetComponent<Ball>();
        if (ball)
        {
            // TODO: clean this up...
            ball.velocity = GetReflectionVector(ball.velocity).normalized * ball.velocity.magnitude;
        }
    }
}