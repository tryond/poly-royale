using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideGoalMediator : MonoBehaviour
{
    public static SideGoalMediator current;

    // Start is called before the first frame update
    void Start()
    {
        // set singleton object
        current = this;

        // listen to sides
        SideManager.current.PositionChangeEvent += OnPositionChange;

        // listen to goals
        GoalManager.current.GoalEliminatedEvent += OnGoalEliminated;

        // set inital goal positions
        GoalManager.current.SetGoalPositions(SideManager.current.GetSides());
    }

    private void OnPositionChange()
    {
        GoalManager.current.SetGoalPositions(SideManager.current.GetSides());
    }

    private void OnGoalEliminated(GameObject goal)
    {
        SideManager.current.CollapseSide(goal.GetComponent<Goal>().id);
    }
}
