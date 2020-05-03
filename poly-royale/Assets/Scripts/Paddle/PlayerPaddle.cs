using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PlayerPaddle : Paddle
{
    void Update()
    {
        movement = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        movement = Input.GetAxis("Horizontal");
        var translation = movement * speed * Time.deltaTime;
        var newX = Mathf.Clamp(transform.localPosition.x + translation, leftBound, rightBound);

        // TODO: clean this up...
        if (newX == transform.localPosition.x)
            movement = 0f;

        transform.localPosition = new Vector3(newX, 0f, 0f);

        // TODO: debug
        DrawNormals();
    }
    
    protected void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        AudioManager.instance.Play("BallHit");
    }
}
