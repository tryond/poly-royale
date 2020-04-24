using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private Camera camera;

    private GameObject playerGoal;
    // if player rotates - rotate with it

    // if player bumps side - shake in direction

    // if player hits ball - shake in direction

    // if side collapsed - add trauma to screen shake

    // if player scores goal - add trauma to screen shake

    // Start is called before the first frame update
    void Start()
    {
        playerGoal = GoalManager.current.GetPlayerGoal();
        camera.transform.up = playerGoal.transform.up;
    }

    private void FixedUpdate()
    {
        //camera.transform.up = Vector3.Lerp(camera.transform.up, playerGoal.transform.up, 0.05f);
        camera.transform.up = playerGoal.transform.up;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (playerGoal != null)
    //    {
    //        camera.transform.up = playerGoal.transform.up;
    //    }
    //}
}
