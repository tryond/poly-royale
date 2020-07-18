using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool isAI = false;
    
    [SerializeField] private float speed;
    private Player player;
    private Paddle paddle;

    [SerializeField] private GameObject ballReflectionPrefab;
    private GameObject ghostBall;
    
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
        ghostBall = Instantiate(ballReflectionPrefab);
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
        if (isAI)
        {
            GetPaddleMovement();
        }
        else
        {
            paddle.movement = Input.GetAxis("Horizontal");

            foreach (Ball ball in BallManager.current.balls)
            {
                // if ball targeting this side
                if (ball.target.Side == player)
                {
                    DrawReflection(ball); 
                }
            }
        }
    }

    private bool CanHit(Paddle paddle, Ball ball)
    {
        // get time till ball hits target
        var ballTime = Vector3.Distance(ball.transform.position, ball.target.Point) / ball.speed;
        var paddleTime = Vector3.Distance(paddle.transform.position, ball.target.Point) / speed;
        return paddleTime <= ballTime;
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
                // DrawReflection(ball);
                
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

    private float GetAngleWeight(Ball ball)
    {
        var angle = Vector3.Angle(-transform.up, ball.transform.up);
        return angle <= 90f ? 1f - (angle / 90f) : 0f;
    }

    private float GetDistanceWeight(Ball ball)
    {
        var distance = transform.InverseTransformPoint(ball.transform.position).y - transform.localPosition.y;
        return distance < MAX_DISTANCE ? 1f - (distance / MAX_DISTANCE) : 0f;
    }

    private void DrawReflection(Ball ball)
    {
        var localPos = transform.InverseTransformPoint(ball.transform.position);
        print("Local Pos: " + localPos);
        
        if (localPos.x < -0.5f)
        {
            // Debug.DrawLine(paddle.transform.position, ball.transform.position, Color.green, Time.fixedDeltaTime);
            
            
            // find intersection with left
            var intersection = LineIntersection(ball.transform.position, ball.target.Point, player.LeftBound,
                player.LeftBound + (100f * player.transform.up));

            var inVec = intersection - ball.transform.position;
            var refVec = Vector3.Reflect(inVec, transform.up);

            ball.reflection.GetComponent<SpriteRenderer>().enabled = true;
            ball.reflection.transform.position = intersection + refVec;
            Debug.DrawLine(intersection + refVec, intersection, Color.green, Time.fixedDeltaTime);
        }
        else if (localPos.x > 0.5f)
        {
            
            var intersection = LineIntersection(ball.transform.position, ball.target.Point, player.RightBound,
                player.RightBound + (100f * player.transform.up));
            
            var inVec = intersection - ball.transform.position;
            var refVec = Vector3.Reflect(inVec, transform.up);
            
            ball.reflection.GetComponent<SpriteRenderer>().enabled = true;
            ball.reflection.transform.position = intersection + refVec;
            Debug.DrawLine(intersection + refVec, intersection, Color.red, Time.fixedDeltaTime);
        }
        else
        {
            ball.reflection.GetComponent<SpriteRenderer>().enabled = false;
            Debug.DrawLine(paddle.transform.position, ball.transform.position, Color.white, Time.fixedDeltaTime);
        }
    }
    
    
    private Vector3 LineIntersection(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var A1 = p1.y - p0.y;
        var B1 = p0.x - p1.x;
        var C1 = A1 * p0.x + B1 * p0.y;
        var A2 = p3.y - p2.y;
        var B2 = p2.x - p3.x;
        var C2 = A2 * p2.x + B2 * p2.y;
        var denominator = A1 * B2 - A2 * B1;

        return new Vector3((B2 * C1 - B1 * C2) / denominator, (A1 * C2 - A2 * C1) / denominator, 0f);
    }
    
    
}
