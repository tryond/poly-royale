using System;
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
    
    
    private float maxHP;
    private float minX;
    private float maxX;

    private Goal goal;
    private Paddle paddle;
    public Paddle Paddle => paddle;

    public (float left, float right) Bounds
    {
        get { return (left: minX, right: maxX); }
        private set { 
            minX = value.left;
            maxX = value.right;
        }
    }
    
    public event Action<Player> OnPlayerEliminated;
    
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
        
        // set paddle bounds
        UpdatePaddleBounds();
    }

    public override void SetBounds(Vector3 leftPosition, Vector3 rightPosition)
    {
        base.SetBounds(leftPosition, rightPosition);
        UpdatePaddleBounds();
    }

    private void UpdatePaddleBounds()
    {
        if (!paddle)
            return;
        
        var relativeWidth = paddle.Width / transform.localScale.x;
        Bounds = (
            LeftBound.transform.localPosition.x + relativeWidth,
            RightBound.transform.localPosition.x - relativeWidth
        );
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpdateHP(bumpRegen);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpdateHP(-goalDamage);
        }
        
        UpdateHP(healthRegen * Time.deltaTime);
    }


    private void UpdateHP(float amount)
    {
        if (amount == 0f)
            return;
        
        hp = Mathf.Clamp(hp + amount, 0f, maxHP);
        if (hp <= 0f)
        {
            // notify listeners
            OnPlayerEliminated?.Invoke(this);
        }
        paddle.Width = hp / maxHP;
        UpdatePaddleBounds();
    }
    
    
    private void GoalScored(Ball ball)
    {
        // update HP
        hp = Mathf.Max(0f, hp - goalDamage);
        
        // process new HP
        if (hp <= 0f)
        {
            // notify listeners
            OnPlayerEliminated?.Invoke(this);
        }
        else
        {
            paddle.Width = hp / maxHP;
            UpdatePaddleBounds();
        }
        
        // reflect ball
        ball.SetVelocity(ball.speed, Vector3.Reflect(ball.transform.up, transform.up));
    }

    private void BallHit(Ball ball)
    {
        // update HP
        hp = Mathf.Min(maxHP, hp + bumpRegen);
        paddle.Width = hp / maxHP;
        UpdatePaddleBounds();
    }
}