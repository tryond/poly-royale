using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private Arena arena;

    private void Start()
    {
        arena = GetComponentInParent<Arena>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        arena.GoalScored(this);
    }
}
