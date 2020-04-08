using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public GameObject leftBound;
    public GameObject rightBound;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;   
    }

    // TODO: remove
    public bool debug = false;

    // TODO: remove
    private void Update()
    {
        if (debug)
        {
            Debug.DrawLine(leftBound.transform.position, rightBound.transform.position, Color.cyan);
        }
    }

    public void SetLeftBound(Vector2 position)
    {
        SetBounds(position, rightBound.transform.position);
    }

    public void SetRightBound(Vector2 position)
    {
        SetBounds(leftBound.transform.position, position);
    }

    public void SetBounds(Vector2 leftPosition, Vector2 rightPosition)
    {
        // set position
        transform.position = new Vector3((leftPosition.x + rightPosition.x) / 2, (leftPosition.y + rightPosition.y) / 2, 0f);

        // set rotation
        Vector2 perpendicular = Vector2.Perpendicular(rightPosition - leftPosition);
        transform.up = perpendicular;

        // set scale
        var newDistance = Vector2.Distance(rightPosition, leftPosition);
        transform.localScale = originalScale * (newDistance / originalScale.x);

        // set bound positions
        leftBound.transform.position = leftPosition;
        rightBound.transform.position = rightPosition;
    }
}
