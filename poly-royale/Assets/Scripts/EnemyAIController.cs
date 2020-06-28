using System;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Player player;
    private Paddle paddle;

    private float TARGET_THRESHOLD = 0.15f;
    private float MOVEMENT_STEP_SIZE = 0.05f;
    private float MAX_DISTANCE = 10f;
    private float MAX_ANGLE = 90f;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        if (!player)
            throw new Exception("required to be child of Player object");
    }

    private void Start()
    {
        paddle = player.Paddle;
    }

    void FixedUpdate()
    {
        // determine new x coordinate
        SetPaddleMovement();
        var translation = paddle.movement * speed * Time.deltaTime;
        
        // print("Paddle local position: " + paddle.transform.localPosition);
        // print("Paddle position: " + paddle.transform.position);
        
        var newX = Mathf.Clamp(
            paddle.transform.localPosition.x + translation,
            player.PaddleBounds.left,
            player.PaddleBounds.right
        );
    
        // float equality check ok here
        if (newX == player.PaddleBounds.left || newX == player.PaddleBounds.right)
            paddle.movement = 0f;
    
        paddle.transform.localPosition = new Vector3(newX, 0f, 0f);
    }

    private void SetPaddleMovement()
    {
        GetPaddleMovement();
    }

    private void GetPaddleMovement()
    {
        // find closest incoming ball
        var numIncoming = 0;
        bool targetSet = false;
        Vector3 target = Vector3.zero;
        float targetTime = 0f;
        foreach (Ball ball in BallManager.current.balls)
        {
            // if ball targeting this side
            if (ball.target.Side == player)
            {
                // Debug.DrawLine(paddle.transform.position, ball.transform.position, Color.yellow, Time.fixedDeltaTime);
                
                // determine if player can get there in time
                var ballTime = Vector3.Distance(ball.transform.position, ball.target.Point) / ball.speed;
                var paddleTime = Vector3.Distance(paddle.transform.position, ball.target.Point) / speed;

                // if (paddleTime <= ballTime && (!targetSet || ballTime < targetTime))
                if (!targetSet || ballTime < targetTime)
                {
                    targetSet = true;
                    // target = ball.transform.position;
                    target = ball.target.Point;
                    targetTime = ballTime;
                }
            }
        }
        
        if (targetSet && Vector3.Distance(paddle.transform.position, target) > TARGET_THRESHOLD)
        {
            // Debug.DrawLine(paddle.transform.position, target, Color.white, Time.fixedDeltaTime);

            // move toward target
            var relativeAngle = Vector3.Angle(paddle.transform.right, target - paddle.transform.position);
            var movement = relativeAngle > 90 ? -MOVEMENT_STEP_SIZE : MOVEMENT_STEP_SIZE;
            paddle.movement = Mathf.Clamp(paddle.movement + movement, -1f, 1f);
        }
        else if (paddle.movement != 0f)
        {
            if (paddle.movement > 0f)
            {
                paddle.movement = Mathf.Max(paddle.movement - MOVEMENT_STEP_SIZE, 0f);
            }
            else
            {
                paddle.movement = Mathf.Min(paddle.movement + MOVEMENT_STEP_SIZE, 0f);
            }
        }
    }
}
