using System;
using UnityEngine;
using System.Collections.Generic;

public class TerrainHeightMap : TerrainBuilder
{
    [Tooltip ("Image source pour la carte de hauteur")]
    [SerializeField]
    private Texture2D _heightMapTexture = null;
    public Texture2D HeightMapTexture { 
        get {
            return _heightMapTexture;
        }
        set {
            _heightMapTexture = value;
        }
    }

    protected override void Awake ()
    {
        base.Awake();
        AddModifier(new BaseHeightModifier(this));
    }

    protected virtual void Start() {
        LevelInfo lInfo = LevelInfo.Instance;
        if (lInfo != null)
            lInfo.Ground = this;
    }

    protected override float ComputeTerrainZDimension(float width)
    {
        float res = 0;
        if (_heightMapTexture != null && _heightMapTexture.width != 0)
            res = Width * _heightMapTexture.height / _heightMapTexture.width;
        return res;
    }

    protected override void OnGeometryChanged()
    {
        int nSubdivX = HeightDataColCount -1;
        int nSubdivY = HeightDataRowCount -1;

        List<Vector2> gradients = new List<Vector2>();
        for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
            for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                gradients.Add(SampleGradient(colIdx, rowIdx));
            }
        }
        HeightMapMesh.SetUVs(1, gradients);
    }

    /// <summary>
    /// Modificateur qui calcule la hauteur de base du terrain
    /// </summary>
    private class BaseHeightModifier : HeightModifier {

        public TerrainHeightMap target;
        private Texture2D lastSampledTexture;

        public BaseHeightModifier(TerrainHeightMap target) {
            this.target = target;
            if (target == null) {
                throw new ArgumentNullException();
            }
        }

        public override System.Boolean IsChanging
        {
            get
            {
                return target.HeightMapTexture != lastSampledTexture;
            }
        }

        public override Rect GetAreaOfEffect()
        {
            return new Rect(new Vector2(-target.Width/2, -target.Height/2), new Vector2(target.Width, target.Height));
        }

        public override float Apply(TerrainBuilder onTerrain, Vector3 atPosition)
        {
            float relativeX;
            float relativeZ;
            if (target.Width > 0 && target.Height > 0)
            {
                relativeX = (atPosition.x + target.Width / 2) / target.Width;
                relativeZ = (atPosition.z + target.Height / 2) / target.Height;
            }
            else
            {
                relativeX = 0;
                relativeZ = 0;
            }
            Color c = SampleHeightMap(relativeX, relativeZ); 
            float res = (c.grayscale -0.5f) * target.TerrainMaxHeight;
            return res;
        }

        private Color SampleHeightMap(float uvx, float uvy) {
            lastSampledTexture = target.HeightMapTexture;
            if (target.HeightMapTexture == null)
                return Color.black;
            
            int pixelXIdx = (int)(uvx * target.HeightMapTexture.width); 
            if (pixelXIdx < 0) 
                pixelXIdx = 0; 
            if (pixelXIdx > target.HeightMapTexture.width - 1) 
                pixelXIdx = target.HeightMapTexture.width - 1; 
            int pixelYIdx = (int)(uvy * target.HeightMapTexture.height); 
            if (pixelYIdx < 0) 
                pixelYIdx = 0; 
            if (pixelYIdx > target.HeightMapTexture.height - 1) 
                pixelYIdx = target.HeightMapTexture.height - 1;
            return target.HeightMapTexture.GetPixel (pixelXIdx, pixelYIdx); 
        }
    }

}

