using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Goal playerGoal;

    [SerializeField] private float downwardTrauma = 0f;
    [SerializeField] private float ballTrauma = 0.25f;
    [SerializeField] private float traumaFalloff = 0.25f;
    
    [SerializeField] private float bumpForce = 1f;

    [SerializeField] private float maxAngle = 15f;
    [SerializeField] private float maxTranslation = 0.05f;

    private Vector3 originalUp;
    private Vector3 originalCenter;
    
    
    // Start is called before the first frame update
    void Start()
    {
        originalUp = camera.transform.up;
        originalCenter = camera.transform.position;
        
        Paddle playerPaddle = null;
        if (playerGoal != null)
        {
            playerPaddle = playerGoal.GetComponentInChildren<Paddle>();
            if (playerPaddle != null)
                playerPaddle.OnBallHit += BallHit;
        }
        
        print("CM: Player Goal: " + playerGoal);
        print("CM: Player Paddle: " + playerPaddle);
    }

    private void BallHit()
    {
        downwardTrauma = Mathf.Clamp(downwardTrauma + ballTrauma, 0f, 1f);
    }

    private void FixedUpdate()
    {
        var shake = downwardTrauma * downwardTrauma;
        var angle = maxAngle * shake * UnityEngine.Random.Range(-1f, 1f);
        var offsetX = maxTranslation * shake * UnityEngine.Random.Range(-1f, 1f);
        var offsetY = maxTranslation * shake * UnityEngine.Random.Range(-1f, 1f);

        // TODO: rotation
        camera.transform.position = originalCenter + new Vector3(0f, offsetY, 0f);
        
        downwardTrauma = Mathf.Clamp(downwardTrauma - (traumaFalloff * Time.fixedDeltaTime), 0f, 1f);
    }

    private void BumpCamera(float force, Vector3 direction)
    {
        print("BUMP!");
    }
}
