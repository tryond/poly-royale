using System;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using UnityEngine.PlayerLoop;

public class Goal : Side
{
    public event Action<GameObject, GameObject> OnGoalScored;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BallGoalCollider"))
        {
            print("Goal!");
            OnGoalScored?.Invoke(this.gameObject, collision.gameObject.transform.parent.gameObject);
        }
    }
    
    private void Update()
    {
        if (CompareTag("Player"))
            Debug.DrawLine(leftBound.transform.position, rightBound.transform.position, Color.green);
        else if (CompareTag("Enemy"))
            Debug.DrawLine(leftBound.transform.position, rightBound.transform.position, Color.red);
    }
}
