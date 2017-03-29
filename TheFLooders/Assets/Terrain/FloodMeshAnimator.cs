using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodMeshAnimator : TerrainBuilder
{
    [Tooltip("Le terrain pour lequel ce script génère un mesh d'inondation")]
    public TerrainHeightMap terrain;
    private bool spentFrameWithNoTerrain;

    protected override void Awake()
    {
        base.Awake();
        spentFrameWithNoTerrain = false;
        AddModifier(new WaterHeightModifier(this));
    }

    protected override void Update()
    {
        base.Update();
        if (terrain != null)
        {
            // On recopie toujours le terrain parent.
            Width = terrain.Width;
        }
        else
        {
            // Tentative de réparation : s'il y a un terrain solide sur le même GameObject on se greffe dessus.
            terrain = GetComponent<TerrainHeightMap>();
            if (terrain == null)
            {
                if (!spentFrameWithNoTerrain)
                    spentFrameWithNoTerrain = true;
                else
                {
                    Debug.LogError("Impossible de trouver le terrain cible pour créer un mesh d'inondation");
                    enabled = false;
                    return;
                }
            }
        }
    }

    protected override float ComputeTerrainZDimension(float width)
    {
        float res = 0;
        if (terrain != null && terrain.Width != 0)
            res = width * terrain.Height / terrain.Width;
        return res;
    }

    /// <summary>
    /// Modificateur de base pour le plan d'inondation, qui fait monter
    /// le niveau petit à petit.
    /// </summary>
    private class WaterHeightModifier : HeightModifier {
        
        private readonly TerrainBuilder target;

        public WaterHeightModifier(TerrainBuilder target) {
            this.target = target;
        }

        public override bool IsChanging
        {
            get
            {
                return true;
            }
        }

        public override Rect GetAreaOfEffect()
        {
            return new Rect(new Vector2(target.Width/2, target.Height/2), new Vector2(target.Width, target.Height));
        }

        public override float Apply(TerrainBuilder onTerrain, Vector3 atPosition)
        {
            return 5;
        }
    }

}

