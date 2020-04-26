using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]

public class EnemyPaddle : Paddle
{

    private bool goingRight = true;
    
    private void Start()
    {
        movement = 1f;
    }

    private void Update()
    {
        if (movement == 0f)
        {
            movement = goingRight ? -1f : 1f;
            goingRight = !goingRight;
        }
    }

    // void FixedUpdate()
    // {
    //     var translation = movement * speed * Time.deltaTime;
    //     var newX = Mathf.Clamp(transform.localPosition.x + translation, leftBound, rightBound);
    //
    //     transform.localPosition = new Vector3(newX, 0f, 0f);
    //
    //     if (newX == leftBound || newX == rightBound)
    //         movement *= -1;
    //
    //     // TODO: debug
    //     //DrawNormals();
    // }
}
