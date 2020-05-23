using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Side : MonoBehaviour
{
    private GameObject leftBound;
    private GameObject rightBound;
    
    private Vector3 originalScale;

    protected LineRenderer lineRenderer;
    
    [SerializeField] [Range(0f, 90f)] float maxReflectionAngle = 80f;
    
    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.25f;

        leftBound = new GameObject();
        rightBound = new GameObject();
        leftBound.transform.parent = transform;
        rightBound.transform.parent = transform;

        var extent = transform.right * transform.localScale.x / 2.0f;
        SetBounds(transform.position - extent, rightBound.transform.position = transform.position + extent);
    }

    public Vector3 LeftBound
    {
        get => leftBound.transform.position;
        set => leftBound.transform.position = value;
    }

    public Vector3 RightBound
    {
        get => rightBound.transform.position;
        set => rightBound.transform.position = value;
    }

    public virtual void SetBounds(Vector3 leftPosition, Vector3 rightPosition)
    {
        // set position to center
        transform.position = new Vector3(
            (leftPosition.x + rightPosition.x) / 2, 
            (leftPosition.y + rightPosition.y) / 2, 
            0f
        );

        // set rotation
        transform.up = Vector2.Perpendicular(rightPosition - leftPosition);

        // set scale
        transform.localScale = originalScale * (Vector2.Distance(rightPosition, leftPosition) / originalScale.x);

        // set bound positions
        LeftBound = leftPosition;
        RightBound = rightPosition;
        
        // set line renderer positions
        lineRenderer.SetPositions(new[] {leftPosition, rightPosition});
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ReflectBall(collision.GetComponent<Ball>());
        }
    }

    protected void ReflectBall(Ball ball)
    {
        Vector3 rawReflection = Vector3.Reflect(ball.Direction, GetNormal());
        var difference = Vector3.SignedAngle(rawReflection, transform.up, transform.forward);
        difference = Mathf.Clamp(difference, -maxReflectionAngle, maxReflectionAngle);
        ball.SetVelocity(ball.speed, Quaternion.Euler(0f, 0f, -difference) * transform.up);
    }

    protected virtual Vector3 GetNormal()
    {
        return transform.up;
    }
}
