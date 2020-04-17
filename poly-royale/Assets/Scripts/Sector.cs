using UnityEngine;

public class Sector : MonoBehaviour
{
    private Arena arena;

    [SerializeField] private Goal goal;
    [SerializeField] private Boundary boundary;

    public Vector2 LeftPoint { get { return goal.leftBound.transform.position; } }
    public Vector2 RightPoint { get { return goal.rightBound.transform.position; } }
    public Vector2 AttachPoint { get { return boundary.rightBound.transform.position; } }

    private void Start()
    {
        arena = GetComponentInParent<Arena>();
    }

    // TODO: clean up the names here
    public void SetSectorPoints(Vector2 left, Vector2 right, Vector2 nextGoalLeft)
    {
        // set the goal coordinates
        goal.SetBounds(left, right);

        // set the boundary coordinates
        boundary.SetBounds(right, nextGoalLeft);
    }

    public void GoalScored(GameObject ball) => arena.GoalScored(this, ball);
}
