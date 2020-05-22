using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;

    public float minSpeed;
    public float maxSpeed;
    public float speedModifier;

    public Vector3 Direction
    {
        get => transform.up;
        set => transform.up = value.normalized;
    }

    public void SetVelocity(float magnitude, Vector3 direction)
    {
        speed = magnitude;
        Direction = direction;
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * Direction;   
    }

    private void OnBecameInvisible()
    {
        speed = minSpeed;
        gameObject.transform.position = Vector3.zero;
    }
}
