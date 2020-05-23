using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;

    public float minSpeed;
    public float maxSpeed;
    public float speedModifier;
    
    [SerializeField] private Target targetPrefab;
    private Target target;

    public void Awake()
    {
        target = Instantiate(targetPrefab);
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
            
            print("Side = " + side);

            if (!side || side == target.Side)
            {
                print("CONTINUE");
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

    private void OnBecameInvisible()
    {
        speed = minSpeed;
        gameObject.transform.position = Vector3.zero;
    }

    public void OnDestroy()
    {
        if (target)
            Destroy(target.gameObject);
    }
}
