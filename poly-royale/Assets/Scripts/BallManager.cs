using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] private int numBalls;
    public int NumBalls { get { return numBalls; } }

    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] [Range(0f, 1f)] private float speedModifier = 0.05f;

    [SerializeField] private float boundsRadius;
    [SerializeField] private Ball ballPrefab;

    private List<Ball> balls;
    private Coroutine currentLaunch = null;

    private void Start()
    {
        // instantiate and store deactivated balls
        balls = new List<Ball>();
        LaunchBalls(numBalls, random: true, overTime: 5f);
    }

    public void Remove(GameObject ball)
    {
        balls.Remove(ball.GetComponent<Ball>());
        Destroy(ball.gameObject);
        
    }

    public void LaunchBalls(int num, float overTime = 0f, bool random = false)
    {
        // stop balls currently launching
        if (currentLaunch != null)
            StopCoroutine(currentLaunch);

        // launch balls
        currentLaunch = StartCoroutine(LaunchBallsCoroutine(num, overTime, random));
    }

    IEnumerator LaunchBallsCoroutine(int num, float overTime = 0f, bool random = false)
    {
        if (num <= 0)
            yield return new WaitForEndOfFrame();

        var ballsLaunched = 0;
        var timeBetweenLaunches = overTime > 0f ? overTime / num : 0f;

        float angle = Random.Range(0f, 360f);
        float uniformAngle = 360f / num;

        for (int i = 0; i < numBalls; ++i)
        {
            var ball = Instantiate(ballPrefab);

            // increment by random or uniform amount
            angle = random ? Random.Range(0f, 360f) : i * uniformAngle;

            // activate and launch balls
            ball.minSpeed = minSpeed;
            ball.maxSpeed = maxSpeed;
            ball.speedModifier = speedModifier;
            
            ball.SetVelocity(minSpeed, Quaternion.Euler(0f, 0f, angle) * transform.right);

            // wait for next launch
            yield return new WaitForSeconds(timeBetweenLaunches);
        }

        // not enough available balls, return
        currentLaunch = null;
    }
}
