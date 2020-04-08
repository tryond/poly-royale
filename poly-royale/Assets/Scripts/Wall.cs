﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Section
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        // get padddle normal
        Ball ball = other.gameObject.GetComponent<Ball>();
        if (ball)
        {
            ball.velocity = Vector3.Reflect(ball.velocity, transform.up);
        }
    }
}
