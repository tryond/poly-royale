using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;

    public float minSpeed;
    public float maxSpeed;
    public float speedModifier;

    public void SetVelocity(float magnitude, Vector3 direction)
    {
        speed = magnitude;
        gameObject.transform.up = direction.normalized;
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.up;   
    }

    private void OnBecameInvisible()
    {
        speed = minSpeed;
        gameObject.transform.position = Vector3.zero;
    }
}
