using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Polygon
{
    // TODO: determine accessibility
    public Vector3[] positions;
    public Quaternion[] rotations;

    public Polygon(int numSides, float sideLength)
    {
        Setup(numSides, sideLength);
    }

    private void Setup(int numSides, float sideLength)
    {
        positions = new Vector3[numSides];
        rotations = new Quaternion[numSides];

        float theta = 360f / numSides;
        // coverts degrees to radians
        float radius = (float)(sideLength / (2f * System.Math.Tan((theta * System.Math.PI) / 360f)));
        Vector3 baseVertex = new Vector3(0f, -radius, 0f);

        for (var i = 0; i < numSides; ++i)
        {
            positions[i] = Quaternion.Euler(0f, 0f, theta * i) * baseVertex;
            rotations[i] = Quaternion.Euler(0f, 0f, theta * i);
        }

    }
}
