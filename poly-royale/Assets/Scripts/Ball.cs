using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public Vector2 velocity;

    // Update is called once per frame
    void FixedUpdate()
    {
        var translation = velocity * Time.deltaTime;
        transform.Translate(new Vector3(translation.x, translation.y));
    }
}
