using System;
using UnityEngine;
using System.Runtime.CompilerServices;

public static class TerrainOperations
{
    public static void CreateMountaintOrHole(this TerrainHeightMap terrain, Vector2 center, float radius, float heightVariation)
    {
        terrain.ApplyOnZone(
            new Rect(center, new Vector2(2 * radius, 2*radius)),
            point => point.y + heightVariation
        );
    }

}

