using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class Side : MonoBehaviour
{
    private GameObject leftBound;
    private GameObject rightBound;
    
    private Vector3 originalScale;

    protected LineRenderer lineRenderer;
    
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

    protected GameObject LeftBound { get => leftBound; }
    protected GameObject RightBound { get => rightBound; }

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
        leftBound.transform.position = leftPosition;
        rightBound.transform.position = rightPosition;
        
        // set line renderer positions
        lineRenderer.SetPositions(new[] {leftPosition, rightPosition});
    }
}
