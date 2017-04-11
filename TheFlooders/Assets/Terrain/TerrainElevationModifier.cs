using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainElevationModifier : HeightModifier
{
    private readonly Vector3 position;
    float radius;
    float height;

    public TerrainElevationModifier(Vector3 position, float radius, float height)
    {
        this.position = position;
        this.radius = radius;
        this.height = height;
    }

    public override Rect GetAreaOfEffect()
    {
        return new Rect(new Vector2(position.x - radius, position.z - radius), new Vector2(2*radius, 2*radius));
    }

    public override float Apply(TerrainBuilder onTerrain, Vector3 atPosition)
    {
        // Evitons de diviser bêtement par zéro plus bas.
        if (radius <= 0)
            return 0;
        
        Vector2 dist = new Vector2(atPosition.x - position.x, atPosition.z - position.z);
        float distanceToEpicenter = dist.magnitude;
        if (distanceToEpicenter > radius)
            distanceToEpicenter = radius;
        
        // Facteur multiplicatif en cos^2 : 1 à l'épicentre, décroît jusqu'à
        // 0 sur le rayon de l'effet, dérivée nulle aux deux extrémités.
        float heightFactor = Mathf.Cos(distanceToEpicenter / radius * Mathf.PI / 2);
        heightFactor *= heightFactor;

        return atPosition.y + height * heightFactor;
    }
}
