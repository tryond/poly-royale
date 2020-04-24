using System.Collections;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] private int numBalls;
    public int NumBalls { get { return numBalls; } }

    [SerializeField] private float speed;
    [SerializeField] [Range(0f, 1f)] private float speedModifier = 0f;
    [SerializeField] private float boundsRadius;
    [SerializeField] private GameObject ballPrefab;

    private GameObject[] balls;
    private Coroutine currentLaunch = null;
    private Coroutine currentScale = null;

    private void Start()
    {
        // instantiate and store deactivated balls
        balls = new GameObject[numBalls];
        for (int i = 0; i < numBalls; i++)
        {
            balls[i] = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            balls[i].GetComponent<Ball>().speedModifier = speedModifier;
            balls[i].SetActive(false);
        }

        LaunchBalls(numBalls);
    }

    void Update()
    {
        // move each ball
        foreach(GameObject ball in balls)
        {
            if (ball.activeSelf)
            {
                // determine new position
                ball.transform.Translate(ball.GetComponent<Ball>().velocity * Time.deltaTime);

                // if outside of bounds, relaunch new ball
                if (Vector2.Distance(ball.transform.position, transform.position) >= boundsRadius)
                {
                    ball.SetActive(false);
                    ball.GetComponent<Ball>().SetVelocity(Vector2.zero);
                    LaunchBalls(1);
                }
            }
        }
    }

    public void Remove(GameObject ball)
    {
        ball.GetComponent<Ball>().SetVelocity(Vector2.zero);
        ball.SetActive(false);
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
        var timeBetweenLaunches = overTime > 0f ?overTime / num : 0f;

        float angle = Random.Range(0f, 360f);
        float uniformAngle = 360f / num;

        for (int i = 0; i < numBalls; ++i)
        {
            // if not currently active, activate and launch
            if (!balls[i].activeSelf)
            {
                // increment by random or uniform amount
                angle += random ? Random.Range(0f, 360f) : ballsLaunched * uniformAngle;

                // activate and launch balls
                balls[i].transform.position = Vector3.zero;
                balls[i].GetComponent<Ball>().SetVelocity(Quaternion.Euler(0f, 0f, angle) * transform.right * speed);
                balls[i].SetActive(true);

                // return if all balls launched else wait for next launch
                if (++ballsLaunched >= num)
                    yield return new WaitForEndOfFrame();
                else
                    yield return new WaitForSeconds(timeBetweenLaunches);
            }
            
        }
        // not enough available balls, return
        currentLaunch = null;
    }

    // TODO: this is broken
    public void ScaleBalls(float scale, float overTime = 0f)
    {
        // stop balls currently scaling
        if (currentScale != null)
            StopCoroutine(currentScale);

        // scale balls
        currentScale = StartCoroutine(ScaleBallsCoroutine(scale, overTime));
    }

    // TODO: this is broken
    private IEnumerator ScaleBallsCoroutine(float scale, float overTime = 0f)
    {
        float transition = 0f;
        float elapsedTime = 0f;

        float t;
        Vector3 lerpScale;

        var startScale = balls[0].transform.localScale;
        var endScale = startScale * scale;

        while (transition < 1f)
        {
            t = overTime > 0 ? elapsedTime / overTime : 1f;
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // smooth stop

            // set scale for each ball
            lerpScale = Vector3.Lerp(startScale, endScale, transition);
            foreach (GameObject ball in balls)
                ball.transform.localScale = lerpScale;

            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentScale = null;
    }
}
