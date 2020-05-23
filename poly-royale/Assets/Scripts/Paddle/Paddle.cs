using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Paddle : MonoBehaviour
{
    public float movement; // between -1 and 1

    [SerializeField] [Range(0f, 90f)] float maxNormalAngle = 45f;
    [SerializeField] [Range(0f, 90f)] float maxReflectionAngle = 60f;
    [SerializeField] private ParticleSystem dustPrefab;
    
    private ParticleSystem dust;

    public event Action<Ball> OnBallHit;

    private void Awake()
    {
        dust = dustPrefab ? Instantiate(dustPrefab) : null;
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

    protected void OnTriggerEnter2D(Collider2D other)
    {
        // return if collider not a ball
        if (!other.gameObject.CompareTag("Ball"))
            return;

        // update ball velocity
        var ball = other.gameObject.GetComponent<Ball>();
        var ballVelocity = ball.speed + (ball.speedModifier * (ball.maxSpeed - ball.speed));
        var ballReflection = GetReflectionVector(ball.transform.up).normalized;
        ball.SetVelocity(Mathf.Clamp(ballVelocity, ball.minSpeed, ball.maxSpeed), ballReflection);

        // dust particles
        if (dust != null)
        {
            var reflection = Vector3.Reflect(ball.transform.up, transform.up);
            dust.transform.position = other.transform.position;
            dust.transform.right = reflection.normalized;
            dust.Play();
        }

        // notify listeners
        OnBallHit?.Invoke(ball);
    }
}
