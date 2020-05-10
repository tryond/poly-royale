using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Player player;
    private Paddle paddle;
    
    private bool goingRight = true;
    
    private void Awake()
    {
        player = GetComponentInParent<Player>();
        if (!player)
            throw new Exception("required to be child of Player object");
    }

    private void Start()
    {
        paddle = player.Paddle;
        paddle.movement = 1f;
    }

    void FixedUpdate()
    {
        var translation = paddle.movement * speed * Time.deltaTime;
        var newX = Mathf.Clamp(
            paddle.transform.localPosition.x + translation,
            player.PaddleBounds.left,
            player.PaddleBounds.right
        );

        if (newX == player.PaddleBounds.left || newX == player.PaddleBounds.right)
            paddle.movement *= -1;
        
        paddle.transform.localPosition = new Vector3(newX, 0f, 0f);
    }
}
