using UnityEngine;

public class Goal : Side
{
    private Sector sector;

    private void Start() => sector = GetComponentInParent<Sector>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ball"))
        {
            sector.GoalScored(collision.gameObject);
        }
    }
}
