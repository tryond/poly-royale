using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    // TODO: don't expose in prod!
    [SerializeField] private Side side;
    public Side Side { get => side; }
    
    private float location = 0f;
    public float Location { get => location; }

    public void Set(Side side, float location)
    {
        this.side = side;
        this.location = location;
    }

    public Vector3 Point { get => side ? Vector3.Lerp(side.LeftBound, side.RightBound, location) : Vector3.zero; }

    private void FixedUpdate()
    {
        transform.position = Point;
    }
}
