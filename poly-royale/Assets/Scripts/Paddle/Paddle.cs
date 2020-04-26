﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Paddle : MonoBehaviour
{
    [SerializeField] [Range(0f, 20f)] protected float speed = 10f;
    [SerializeField] [Range(0f, 90f)] float maxNormalAngle = 45f;
    [SerializeField] [Range(0f, 90f)] float maxReflectionAngle = 60f;
    [SerializeField] [Range(1f, 2f)] float speedModifier = 1f;

    private float paddleWidth;

    protected float leftBound;
    protected float rightBound;

    protected float movement; // between -1 and 1

    [SerializeField] Goal goal;

    public event Action OnBallHit;
    public event Action OnBoundHit;
    
    private void Awake()
    {
        paddleWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        leftBound = goal.leftBound.transform.localPosition.x + (paddleWidth / transform.parent.localScale.x);
        rightBound = goal.rightBound.transform.localPosition.x - (paddleWidth / transform.parent.localScale.x);
    }

    void FixedUpdate()
    {
        var translation = movement * speed * Time.deltaTime;
        var newX = Mathf.Clamp(transform.localPosition.x + translation, leftBound, rightBound);

        if (newX == leftBound || newX == rightBound)
        {
            movement = 0f;
            
            // notify listeners
            OnBoundHit?.Invoke();
        }
        
        transform.localPosition = new Vector3(newX, 0f, 0f);
    }
    
    public Vector3 GetNormalVector()
    {
        return Quaternion.Euler(0f, 0f, -maxNormalAngle * movement) * transform.up;
    }

    public Vector3 GetReflectionVector(Vector3 direction)
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
        if (other.gameObject.CompareTag("Ball"))
        {
            var ball = other.gameObject.GetComponent<Ball>();
            ball.SetVelocity(ball.speed + ball.speedModifier, GetReflectionVector(ball.transform.up).normalized);
            
            // notify listeners
            OnBallHit?.Invoke();
        }
    }
}
