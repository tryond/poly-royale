using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            var normal = (other.transform.position - transform.position).normalized;
            var ball = other.GetComponent<Ball>();
            
            ball.SetVelocity(ball.speed, Vector3.Reflect(ball.Direction, normal));
        }
    }
}
