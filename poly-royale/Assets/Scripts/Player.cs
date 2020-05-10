using System;
using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : Side
{
    [SerializeField] private float hp;
    [SerializeField] private Color color;
    
    [SerializeField] private Goal goalPrefab;
    [SerializeField] private Paddle paddlePrefab;

    [SerializeField] private float goalDamage;
    [SerializeField] private float bumpRegen;
    [SerializeField] private float healthRegen;

    private float basePaddleScaleX;
    private float minPaddleScaleX;
    
    [SerializeField] private float paddleScaleTime;
    private Coroutine currentPaddleScaleCoroutine = null;

    private float maxHP;
    private float minX;
    private float maxX;

    private Goal goal;
    private Paddle paddle;
    public Paddle Paddle => paddle;

    private Ball lastBallScored = null;

    public (float left, float right) PaddleBounds
    {
        get { return (left: minX, right: maxX); }
        private set { 
            minX = value.left;
            maxX = value.right;
        }
    }
    
    public event Action<Player, Ball> OnPlayerEliminated;
    
    protected override void Awake()
    {
        base.Awake();

        // set max HP to starting HP
        maxHP = hp;
        
        // set renderer color
        lineRenderer.material.color = color;

        // instantiate entities
        goal = Instantiate(goalPrefab, parent: transform);
        paddle = Instantiate(paddlePrefab, parent: transform);

        // subscribe to goal and paddle
        goal.OnGoalScored += GoalScored;
        paddle.OnBallHit += BallHit;
        
        // set initial scales
        basePaddleScaleX = paddle.transform.localScale.x;
        minPaddleScaleX = 0.001f;
        
        // set paddle bounds
        UpdatePaddle();
    }

    public override void SetBounds(Vector3 leftPosition, Vector3 rightPosition)
    {
        base.SetBounds(leftPosition, rightPosition);
        UpdatePaddle();
    }

    private void UpdatePaddle()
    {
        if (!paddle)
            return;
        
        if (currentPaddleScaleCoroutine != null)
            StopCoroutine(currentPaddleScaleCoroutine);
        currentPaddleScaleCoroutine = StartCoroutine(ScalePaddle());
    }

    private void Update()
    {
        UpdateHP(healthRegen * Time.deltaTime);
        
        // TODO: test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetBounds(
                LeftBound - new Vector3(1f, 1f, 0f), 
                RightBound + new Vector3(1f, 1f, 0f)
            );
        }
    }

    private void UpdateHP(float amount)
    {
        if (amount == 0f)
            return;
        
        hp = Mathf.Clamp(hp + amount, 0f, maxHP);
        if (hp <= 0f)
        {
            // notify listeners
            OnPlayerEliminated?.Invoke(this, lastBallScored);
        }
        UpdatePaddle();
    }

    private void GoalScored(Ball ball)
    {
        lastBallScored = ball;
        UpdateHP(-goalDamage);
        
        // reflect ball
        ball.SetVelocity(ball.speed, Vector3.Reflect(ball.transform.up, transform.up));
    }

    private void BallHit(Ball ball)
    {
        UpdateHP(bumpRegen);
    }

    private void UpdatePaddleBounds()
    {
        var paddleExtent = paddle.transform.localScale.x / 2.0f;
        PaddleBounds = (-0.5f + paddleExtent, 0.5f - paddleExtent);
    }
    
    private IEnumerator ScalePaddle()
    {
        var transition = 0f;
        var elapsedTime = 0f;
        
        var start = paddle.transform.localScale;
        var endX = Mathf.Max((hp / maxHP) * basePaddleScaleX, minPaddleScaleX);
        
        while (transition < 1f)
        {
            var t = paddleScaleTime > 0f ? elapsedTime / paddleScaleTime : 1f;
            transition = Mathf.Clamp(1 - (1 - t) * (1 - t) * (1 - t), 0f, 1f);  // smooth stop
            
            // determine new width
            var currentX = Mathf.Lerp(start.x, endX, transition);
            paddle.transform.localScale = new Vector3(currentX, start.y, start.z);

            // update paddle bounds
            UpdatePaddleBounds();
            
            // wait for the end of frame and yield
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentPaddleScaleCoroutine = null;
    }
}