using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Player player;
    private Paddle paddle;
    
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
        paddle.movement = Input.GetAxis("Horizontal");
        var translation = paddle.movement * speed * Time.deltaTime;
        var newX = Mathf.Clamp(
            paddle.transform.localPosition.x + translation,
            player.Bounds.left,
            player.Bounds.right
        );

        // float equality check ok here
        if (newX == player.Bounds.left || newX == player.Bounds.right)
            paddle.movement = 0f;
    
        paddle.transform.localPosition = new Vector3(newX, 0f, 0f);
    }
}
