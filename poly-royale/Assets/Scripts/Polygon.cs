using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Polygon
{
    [HideInInspector] public Vector3[] positions;

    private float sideLength;
    private float radius;

    public Polygon(int numSides, float radius)
    {
        Setup(numSides, radius);
    }

    private void Setup(int numSides, float radius)
    {
        positions = new Vector3[numSides];
        float theta = 360f / numSides;
        sideLength = (float)(2f * radius * System.Math.Tan((theta * System.Math.PI) / 360f));
        Vector3 baseVertex = Quaternion.Euler(0f, 0f, -theta / 2) * new Vector3(0f, -radius, 0f); ;

        for (var i = 0; i < numSides; ++i)
            positions[i] = Quaternion.Euler(0f, 0f, theta * i) * baseVertex;
    }

    public float GetSideLength()
    {
        return sideLength;
    }
}
