using UnityEngine;

public class Sector : MonoBehaviour
{
    [SerializeField] private Goal goal;
    [SerializeField] private Boundary boundary;

    // TODO: clean up the names here
    public void SetSectorPoints(Vector2 left, Vector2 right, Vector2 nextGoalLeft)
    {
        // set the goal coordinates
        goal.SetBounds(left, right);

        // set the boundary coordinates
        boundary.SetBounds(right, nextGoalLeft);
    }
}
