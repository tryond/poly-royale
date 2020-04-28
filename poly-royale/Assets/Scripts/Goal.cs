using System;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using UnityEngine.PlayerLoop;

public class Goal : Side
{
    public event Action<GameObject, GameObject> OnGoalScored;

    private void Start()
    {
        if (CompareTag("Player"))
            lineRenderer.material.color = Color.green;
        else if (CompareTag("Enemy"))
            lineRenderer.material.color = Color.red;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BallGoalCollider"))
        {
            print("Goal!");
            OnGoalScored?.Invoke(this.gameObject, collision.gameObject.transform.parent.gameObject);
        }
    }
}
