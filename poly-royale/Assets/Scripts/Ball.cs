using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector2 velocity;
    public float speedModifier = 1f;

    public void SetVelocity(Vector2 velocity)
    {
        this.velocity = velocity;

    }
}
