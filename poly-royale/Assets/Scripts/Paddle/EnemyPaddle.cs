using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class EnemyPaddle : Paddle
{

    private void Start()
    {
        movement = 1f;
    }

    void FixedUpdate()
    {
        var translation = movement * speed * Time.deltaTime;
        var newX = Mathf.Clamp(transform.localPosition.x + translation, leftBound, rightBound);

        transform.localPosition = new Vector3(newX, 0f, 0f);

        if (newX == leftBound || newX == rightBound)
            movement *= -1;

        // TODO: debug
        //DrawNormals();
    }
}
