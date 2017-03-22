using System;
using UnityEngine;
using System.Collections.Generic;

public class TerrainHeightMap : MonoBehaviour
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
                RecomputeSamples ();
            }
        }
    }

    [Tooltip ("Pas d'échantillonage de la heightmap (espace UV) pour déterminer le gradient")]
    [SerializeField]
    private float _gradientSampleUVPitch = 0.01f;
    public float GradientSampleUVPitch {
        get {
            return _gradientSampleUVPitch;
        }
        set {
            if (_gradientSampleUVPitch != value) {
                _gradientSampleUVPitch = value;
                RecomputeSamples ();
            }
        }
    }

    [Tooltip("MeshFilter cible où ce composant écrit le mesh qu'il produit automatiquement")]
    public MeshFilter TargetMeshFilter;

    [Tooltip("MeshCollider cible où ce composant écrit le mesh qu'il produit automatiquement")]
    public MeshCollider TargetMeshCollider;

    [Tooltip("Hauteur maximale du terrain (amplitude des déformations)")]
    public float TerrainMaxHeight = 20;

    [Tooltip ("Nombre de subdivisions de la dimension la plus petite du terrain.")]
    [SerializeField]
    private int _minSubdivisions = 1;

    public int MinSubdivisions {
        get {
            return _minSubdivisions;
        }
        set {
            if (_minSubdivisions != value) {
                _minSubdivisions = value;
                RecomputeSamples ();
            }
        }
    }

    [Tooltip ("Largeur du \"monde\".")]
    [SerializeField]
    private float _width = 100;

    public float Width {
        get {
            return _width;
        }
        set {
            if (_width != value) {
                _width = value;
                RecomputeSamples ();
            }
        }
    }

    /// <summary>
    /// Hauteur du "monde"
    /// </summary>
    public float Height {
        get {
            float res = 0;
            if (_heightMapTexture.width != 0)
                res = Width * _heightMapTexture.height / _heightMapTexture.width;
            return res;
        }
    }

    /// <summary>
    /// Maillage du terrain
    /// </summary>
    public Mesh HeightMapMesh{ get; private set; }

    // Les données de hauteur
    private float[][] _heightData;

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

    private void Start ()
    {
        HeightMapMesh = new Mesh ();
        HeightMapMesh.name = "AltitudeMesh";
        RecomputeSamples ();
    }

    private void Update() {
        
    }

    public delegate float TerrainTransform(Vector3 localPosition);

    public void ApplyOnZone(Rect targetZone, TerrainTransform transformOperator) {
        if (IsEmpty())
            return;

        // Détermination des indices concernés
        int nCols = _heightData.Length;
        int nRows = _heightData [0].Length;
        int colIndexMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.xMin + Width / 2) / Width * (nCols -1), 0, nCols-1));
        int colIndexMax = (int) Mathf.Clamp((targetZone.xMax + Width / 2) / Width * (nCols -1), 0, nCols-1);
        int rowIndexMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.yMin + Height / 2) / Height * (nRows -1), 0, nRows-1));
        int rowIndexMax = (int) Mathf.Clamp((targetZone.yMax + Height / 2) / Height * (nRows -1), 0, nRows-1);
        if (colIndexMax < colIndexMin || rowIndexMax < rowIndexMin)
            return;

        // Copie de la zone affectée pour faire les modifs sans changer
        // l'état courant.
        float[][] copy = new float[colIndexMax - colIndexMin + 1][];
        for (int i = 0; i < colIndexMax - colIndexMin + 1; i++)
            copy [i] = new float[rowIndexMax - rowIndexMin + 1];
        for (int colIndex = colIndexMin; colIndex <= colIndexMax; colIndex++) {
            for (int rowIndex = rowIndexMin; rowIndex <= rowIndexMax; rowIndex++) {
                copy [colIndex - colIndexMin] [rowIndex - rowIndexMin] = _heightData [colIndex] [rowIndex];
            }
        }

        // Passage de l'opérateur
        for (int colIndex = colIndexMin; colIndex <= colIndexMax; colIndex++) {
            for (int rowIndex = rowIndexMin; rowIndex <= rowIndexMax; rowIndex++) {
                Vector3 position = new Vector3(
                    ((float) colIndex / nCols - 0.5f) * Width,
                    _heightData [colIndex] [rowIndex],
                    ((float) rowIndex / nRows - 0.5f) * Height);
                copy [colIndex - colIndexMin] [rowIndex - rowIndexMin] = transformOperator(position);
            }
        }

        // Recopie des nouvelles données, modification de l'état courant.
        for (int colIndex = colIndexMin; colIndex <= colIndexMax; colIndex++) {
            for (int rowIndex = rowIndexMin; rowIndex <= rowIndexMax; rowIndex++) {
                _heightData [colIndex] [rowIndex] = copy [colIndex - colIndexMin] [rowIndex - rowIndexMin];
            }
        }

        // Recalcul du mesh.
        ApplyHeightData();
    }

    /// <summary>
    /// Echantillonne la hauteur du terrain sur une grille
    /// de pas relativement égal dans les deux directions.
    /// </summary>
    public void RecomputeSamples ()
    {
        HeightMapMesh.Clear ();
        _heightData = null;

        // test de validité
        if (IsEmpty ()) {
            return;
        }

        // Calcul du nombre de subdivisions sur chaque axe
        int nSubdivX = 0;
        int nSubdivY = 0;
        float heightCopy = Height; // Calcule le getter une seule fois
        if (_width > 0 && heightCopy > 0 && _minSubdivisions >= 1) {
            if (_width > heightCopy) {
                nSubdivY = _minSubdivisions;
                nSubdivX = Mathf.CeilToInt (_width / heightCopy * _minSubdivisions);
            } else {
                nSubdivX = _minSubdivisions;
                nSubdivY = Mathf.CeilToInt (heightCopy / _width * _minSubdivisions);
            }
        }

        // Unity ne gère pas les meshes de plus de 65k sommets
        if ((nSubdivX + 1) * (nSubdivY + 1) > 65000) {
            Debug.LogError ("Ce terrain est trop détaillé, Unity ne peut pas représenter un Mesh de cette taille");
            return;
        }

        // Instanciation du tableau des hauteurs.
        _heightData = new float[nSubdivX + 1][];
        for (int i = 0; i <= nSubdivX; i++)
            _heightData [i] = new float[nSubdivY + 1];

        // Echantillonnage de l'image source
        for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
            for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                Color c = SampleTexture((float)colIdx / nSubdivX, (float)rowIdx / nSubdivY);
                _heightData [colIdx] [rowIdx] = c.grayscale * TerrainMaxHeight;
            }
        }

        ApplyHeightData ();
    }

    private void ApplyHeightData() {
        int nSubdivX = _heightData.Length -1;
        int nSubdivY = _heightData[0].Length -1;

        // Transformation en vertex
        Vector3[] vertices = new Vector3[(nSubdivX + 1) * (nSubdivY + 1)];
        for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
            for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                int vertexIdx = colIdx * (nSubdivY + 1) + rowIdx;
                float x = colIdx * Width / nSubdivX - Width / 2;
                float z = rowIdx * Height / nSubdivY - Height / 2;
                vertices [vertexIdx] = new Vector3 (x, _heightData [colIdx] [rowIdx], z);
            }
        }
        // Remplissage des sommets du mesh
        HeightMapMesh.vertices = vertices;

        // Recalcul des triangles et UV
        RebuildTrianglesAndUVs(nSubdivX, nSubdivY);

        if (TargetMeshFilter != null)
            TargetMeshFilter.mesh = HeightMapMesh;
        if (TargetMeshCollider != null)
            TargetMeshCollider.sharedMesh = HeightMapMesh;
    }

    private bool IsEmpty() {
        return (
            _heightMapTexture == null || _heightMapTexture.width <= 0 ||
            _heightMapTexture.height <= 0 ||
            _width <= 0 || _minSubdivisions < 1);
    }

    private void RebuildTrianglesAndUVs(int nSubdivX, int nSubdivY) {

        List<Vector4> uvs = new List<Vector4>();
        for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
            for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                
                // Le gradient de la heightmap est passé dans le shader
                // via les canaux 3 et 4 des coordonnées UV.
                // Il s'en servira pour déterminer la hauteur de sa bande colorée
                float uvx = (float)colIdx / nSubdivX;
                float uvy = (float)rowIdx / nSubdivY;
                Vector2 grad = SampleGradient(uvx, uvy);
                Vector4 newVect = new Vector4(uvx, uvy, grad.x, grad.y);
//                Debug.Log(newVect);
                uvs.Add(newVect);
            }
        }

        int[] triangles = new int[6 * nSubdivX  * nSubdivY]; // Un quad = 2 triangles = 6 sommets
        for (int colIdx = 0; colIdx < nSubdivX; colIdx++) {
            for (int rowIdx = 0; rowIdx < nSubdivY; rowIdx++) {
                int baseVertexIndex = (nSubdivY + 1) * colIdx + rowIdx;
                int baseIdx = 6 * (nSubdivY * colIdx + rowIdx);
                triangles[baseIdx] = baseVertexIndex;
                triangles[baseIdx+1] = baseVertexIndex + 1;
                triangles[baseIdx+2] = baseVertexIndex + nSubdivY + 1;
                triangles[baseIdx+3] = baseVertexIndex + nSubdivY + 1;
                triangles[baseIdx+4] = baseVertexIndex + 1;
                triangles[baseIdx+5] = baseVertexIndex + nSubdivY + 2;
            }
        }

        HeightMapMesh.triangles = triangles;
        HeightMapMesh.SetUVs(0, uvs);
		HeightMapMesh.RecalculateNormals ();
    }

    private Color SampleTexture(float uvx, float uvy) {
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

    private Vector2 SampleGradient(float uvx, float uvy) {
        Color cxPlus = SampleTexture(uvx + _gradientSampleUVPitch / 2, uvy);
        Color cxMinus = SampleTexture(uvx - _gradientSampleUVPitch / 2, uvy);
        float dx = (cxPlus.grayscale -cxMinus.grayscale) / _gradientSampleUVPitch;
        Color cyPlus = SampleTexture(uvx, uvy + _gradientSampleUVPitch / 2);
        Color cyMinus = SampleTexture(uvx, uvy - _gradientSampleUVPitch / 2);
        float dy = (cyPlus.grayscale -cyMinus.grayscale) / _gradientSampleUVPitch;
        return new Vector2(dx, dy);
    }

}

