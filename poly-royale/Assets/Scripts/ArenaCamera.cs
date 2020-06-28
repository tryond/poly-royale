using UnityEngine;

public class ArenaCamera : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Side playerSide;

    private void FixedUpdate()
    {
        if (camera && playerSide)
            camera.transform.up = playerSide.transform.up;
    }
}
