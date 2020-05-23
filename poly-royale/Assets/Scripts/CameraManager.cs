using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Side playerSide;

    [SerializeField] private float minBallTrauma = 0.55f;
    [SerializeField] private float maxBallTrauma = 0.55f;
    [SerializeField] private float translationalFrequency = 1f;
    [SerializeField] private float tranlationalTraumaFalloff = 0.15f;
    private float downwardTrauma = 0f;

    
    [SerializeField] private float maxAngle = 0.1f;
    [SerializeField] private float maxVerticalTranslation = 0.05f;
    [SerializeField] private float maxHorizontalTranslation = 0.1f;

    [SerializeField] private float rotationalTraumaFalloff = 0.25f;
    [SerializeField] private float rotationalFrequency = 1f;
    private float rotationalTrauma = 0f;
    
    private Vector3 originalUp;
    private Vector3 originalCenter;

    private bool transitioning = false;

    private float seed;
    
    // Start is called before the first frame update
    void Start()
    {
        originalUp = camera.transform.up;
        originalCenter = camera.transform.position;
        
        Paddle playerPaddle = null;
        if (playerSide != null)
        {
            playerPaddle = playerSide.GetComponent<Paddle>();
            if (playerPaddle != null)
                playerPaddle.OnBallHit += BallHit;
        }

        Arena.current.OnTransitionStart += StartTransition;
        Arena.current.OnTransitionEnd += EndTransition;

        seed = Random.value;
    }

    private void BallHit(Ball ball)
    {
        var ballTrauma = (ball.speed - ball.minSpeed) / (ball.maxSpeed - ball.minSpeed);
        ballTrauma = Mathf.Lerp(minBallTrauma, maxBallTrauma, ballTrauma);
        downwardTrauma = Mathf.Clamp(downwardTrauma + ballTrauma, 0f, 1f);
    }

    private void StartTransition()
    {
        transitioning = true;
    }

    private void EndTransition()
    {
        transitioning = false;
    }
    
    private void FixedUpdate()
    {
        // update with main player
        if (playerSide)
            originalUp = playerSide.transform.up;
        
        // translate
        var shake = downwardTrauma * downwardTrauma;
        var offsetX = maxHorizontalTranslation * shake * (Mathf.PerlinNoise(seed, Time.time  * translationalFrequency) * 2.0f - 1.0f) * 0.5f;
        var offsetY = maxVerticalTranslation * shake * (Mathf.PerlinNoise(seed + 1, Time.time  * translationalFrequency) * 2.0f - 1.0f) * 0.5f;
        camera.transform.position = originalCenter + new Vector3(offsetX, offsetY, 0f);
        downwardTrauma = Mathf.Clamp(downwardTrauma - (tranlationalTraumaFalloff * Time.fixedDeltaTime), 0f, 1f);

        // rotate
        rotationalTrauma = transitioning ? 1f : Mathf.Clamp(rotationalTrauma - (rotationalTraumaFalloff * Time.fixedDeltaTime), 0f, 1f);
        var twist = rotationalTrauma * rotationalTrauma;
        var angle = maxAngle * twist * (Mathf.PerlinNoise(seed + 2, Time.time * rotationalFrequency) * 2.0f - 1.0f) * 0.5f;
        camera.transform.up = Quaternion.Euler(0f, 0f, angle) * originalUp;
    }

    private void BumpCamera(float force, Vector3 direction)
    {
        print("BUMP!");
    }
}
