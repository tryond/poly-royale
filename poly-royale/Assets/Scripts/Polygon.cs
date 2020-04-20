using System;
using UnityEngine;

[Serializable] public class Polygon
{
    static public Vector3[] Points(int numSides, float radius)
    {
        if (numSides <= 1)
            return new Vector3[] { Vector3.zero };

        // determine base vertex which to rotate around circle
        float theta = 360f / numSides;
        //Vector3 baseVertex = Quaternion.Euler(0f, 0f, -theta / 2) * new Vector3(0f, -radius, 0f);

        // calculate points around circle
        var points = new Vector3[numSides];
        for (var i = 0; i < numSides; ++i)
            points[i] = Quaternion.Euler(0f, 0f, theta * i) * Vector3.one * radius;

        return points;
    }
}
