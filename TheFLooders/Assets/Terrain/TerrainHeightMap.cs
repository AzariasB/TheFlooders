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
            if (_heightMapTexture != value) {
                _heightMapTexture = value;
                RebuildTerrain ();
            }
        }
    }

    /// <summary>
    /// Obtient la hauteur du terrain aux coordonnées spécifiées.
    /// </summary>
    public double GetHeight (float x, float z)
    {
        double res = 0;
        if (_heightMapTexture != null) {
            int pixelXIdx = (int)((x + Width/2) / Width * _heightMapTexture.width);
            int pixelZIdx = (int)((z + Height/2) / Height * _heightMapTexture.height);
            Color c = _heightMapTexture.GetPixel (pixelXIdx, pixelZIdx);
            res = c.grayscale * TerrainMaxHeight;
        }
        return res;
    }

    protected override void Start ()
    {
        base.Start();
        RebuildTerrain ();
    }

    private Color SampleHeightMap(float uvx, float uvy) { 
        int pixelXIdx = (int)(uvx * _heightMapTexture.width); 
        if (pixelXIdx < 0) 
            pixelXIdx = 0; 
        if (pixelXIdx > _heightMapTexture.width - 1) 
            pixelXIdx = _heightMapTexture.width - 1; 
        int pixelYIdx = (int)(uvy * _heightMapTexture.height); 
        if (pixelYIdx < 0) 
            pixelYIdx = 0; 
        if (pixelYIdx > _heightMapTexture.height - 1) 
            pixelYIdx = _heightMapTexture.height - 1; 
        return _heightMapTexture.GetPixel (pixelXIdx, pixelYIdx); 
    }

    protected override float ComputeTerrainHeight(float width)
    {
        float res = 0;
        if (_heightMapTexture != null && _heightMapTexture.width != 0)
            res = Width * _heightMapTexture.height / _heightMapTexture.width;
        return res;
    }

    protected override float SampleBaseHeight(float relativeX, float relativeY) {
        Color c = SampleHeightMap(relativeX, relativeY); 
        return (c.grayscale -0.5f) * TerrainMaxHeight;
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

}

