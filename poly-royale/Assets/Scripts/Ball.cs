using System;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;

    public float minSpeed;
    public float maxSpeed;
    public float speedModifier;

    private static int idCount = 0;
    private int _id;
    private int ID { get => _id; }
    
    [SerializeField] private Target targetPrefab;
    public Target target;

    public void Awake()
    {
        target = Instantiate(targetPrefab);
        _id = idCount;
        idCount++;
    }

    public Vector3 Direction
    {
        get => gameObject.transform.up;
        set => gameObject.transform.up = value.normalized;
    }

    public void SetVelocity(float magnitude, Vector3 direction)
    {
        speed = magnitude;
        Direction = direction;
        
        SetTarget();
    }

    private void SetTarget()
    {
        // cast ray out in direction of motion
        var hits = Physics2D.RaycastAll(transform.position, Direction, Mathf.Infinity, 1 << 8);
        
        // find the first side that is not the current side
        foreach (var hit in hits)
        {
            var side = hit.collider.GetComponent<Side>();
            if (!side || side == target.Side)
            {
                continue;
            }

            // find location along side
            var fromLeft = Vector3.Distance(hit.point, side.LeftBound);
            var fromRight = Vector3.Distance(hit.point, side.RightBound);
            var location = fromLeft / (fromLeft + fromRight);
            
            // set target
            target.Set(side, location);
        }
    }

    private void FixedUpdate()
    {
        var travelDistance = speed * Time.fixedDeltaTime;

        if (!target.Side)
        {
            SetTarget();
            // transform.position += travelDistance * Direction;
        }
        else
        {
            var trajectory = target.Point - transform.position;
            transform.up = trajectory.normalized;
            
            // move to the target point if closer
            transform.position += Mathf.Min(travelDistance, trajectory.magnitude) * Direction;
        }
    }

    // private void OnBecameInvisible()
    // {
    //     speed = minSpeed;
    //     gameObject.transform.position = Vector3.zero;
    // }

    public void OnDestroy()
    {
        if (target)
            Destroy(target.gameObject);
    }
}
