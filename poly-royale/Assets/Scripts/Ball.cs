using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;
    public float speedModifier = 1f;
  
    public void SetVelocity(float magnitude, Vector3 direction)
    {
        speed = magnitude;
        gameObject.transform.up = direction.normalized;
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.up;   
    }
}
