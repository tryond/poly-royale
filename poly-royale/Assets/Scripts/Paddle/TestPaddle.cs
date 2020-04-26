using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPaddle : MonoBehaviour
{
    // TODO: delete
    public Vector3 direction;

    [SerializeField] [Range(0f, 20f)] float speed = 10f;
    [SerializeField] [Range(0f, 90f)] float maxNormalAngle = 45f;
    [SerializeField] [Range(0f, 90f)] float maxReflectionAngle = 60f;

    private float paddleWidth;
    private float parentWidth;

    private float leftBound;
    private float rightBound;

    private float movement; // between -1 and 1

    private void Awake()
    {
        var oldRotation = transform.rotation;
        transform.parent.rotation = Quaternion.identity;

        paddleWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        parentWidth = transform.parent.GetComponent<SpriteRenderer>().bounds.extents.x;

        rightBound = (parentWidth - paddleWidth) / transform.parent.localScale.x;
        leftBound = -rightBound;

        transform.parent.rotation = oldRotation;
    }

    void FixedUpdate()
    {
        movement = Input.GetAxis("Horizontal");
        var translation = movement * speed * Time.deltaTime;
        var newX = Mathf.Clamp(transform.localPosition.x + translation, leftBound, rightBound);

        // TODO: clean this up...
        if (newX == transform.localPosition.x)
            movement = 0f;

        transform.localPosition = new Vector3(newX, 0f, 0f);

        // TODO: debug
        DrawNormals();
    }

    public Vector3 GetNormalVector()
    {
        return Quaternion.Euler(0f, 0f, -maxNormalAngle * movement) * transform.up;
    }

    public Vector3 GetReflectionVector(Vector2 direction)
    {
        Vector3 rawReflection = Vector3.Reflect(direction, GetNormalVector());
        var difference = Vector3.SignedAngle(rawReflection, transform.up, transform.forward);
        difference = Mathf.Clamp(difference, -maxReflectionAngle, maxReflectionAngle);
        return Quaternion.Euler(0f, 0f, -difference) * transform.up;
    }

    void DrawNormals()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up * paddleWidth, Color.green);
        Debug.DrawLine(transform.position, transform.position + GetReflectionVector(direction) * paddleWidth, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + GetNormalVector() * paddleWidth, Color.red);

        // draw direction vector
        Debug.DrawLine(transform.position, transform.position - direction * paddleWidth, Color.black);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // get padddle normal
        Ball ball = other.gameObject.GetComponent<Ball>();
        if (ball)
        {
            // TODO: clean this up...
            //ball.velocity = GetReflectionVector(ball.velocity).normalized * ball.velocity.magnitude;
        }
    }
}
