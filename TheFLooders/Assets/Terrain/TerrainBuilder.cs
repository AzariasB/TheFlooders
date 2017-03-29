using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainBuilder : MonoBehaviour {

    [Tooltip ("Largeur du \"monde\".")]
    [SerializeField]
    private float _width = 100;

    /// <summary>
    /// Largeur du "monde". Sert de base au calcul de la hauteur
    /// </summary>
    public float Width {
        get {
            return _width;
        }
        set {
            if (_width != value) {
                _width = value;
                RebuildTerrain();
            }
        }
    }

    /// <summary>
    /// Hauteur du "monde". Calculée à partir de la largeur
    /// </summary>
    public float Height {
        get {
            return ComputeTerrainHeight(Width);
        }
    }

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
                RebuildTerrain();
            }
        }
    }

    /// <summary>
    /// Retourne vrai ssi le maillage est vide ou ne peut être calculé.
    /// </summary>
    public virtual bool IsEmpty {
        get {
            return (Width <= 0 || Height <= 0 || MinSubdivisions < 1);
        }
    }

    [Tooltip("MeshFilter cible où ce composant écrit le mesh qu'il produit automatiquement")]
    public MeshFilter TargetMeshFilter;

    [Tooltip("MeshCollider cible où ce composant écrit le mesh qu'il produit automatiquement")]
    public MeshCollider TargetMeshCollider;

    [Tooltip("Hauteur maximale du terrain, approximative (c'est un guide pour les modificateurs)")]
    public float TerrainMaxHeight = 20;

    /// <summary>
    /// Méthode qui calcule la nouvelle hauteur du terrain suite à une
    /// modification donnée, en fonction de la position initiale du point transformé.
    /// </summary>
    public delegate float TerrainTransform(Vector3 localPosition, float targetAmplitude);

    // Tableau des hauteurs
    private float[][] _heightData;

    /// <summary>
    /// Taille du tableau des altitudes sur X
    /// </summary>
    protected int HeightDataColCount {
        get { return _heightDataColCount; }
    }
    private int _heightDataColCount;

    /// <summary>
    /// Taille du tableau des altitudes sur Z
    /// </summary>
    protected int HeightDataRowCount {
        get { return _heightDataRowCount; }
    }
    private int _heightDataRowCount;

    /// <summary>
    /// Maillage du terrain
    /// </summary>
    protected Mesh HeightMapMesh
    {
        get { return _heightMapMesh; }
    }
    private Mesh _heightMapMesh;

    /// <summary>
    /// Echantillonne la hauteur du terrain sur une grille
    /// de pas relativement égal dans les deux directions.
    /// </summary>
    public void RebuildTerrain ()
    {
        _heightMapMesh.Clear ();
        _heightData = null;
        _heightDataColCount = 0;
        _heightDataRowCount = 0;

        // test de validité
        if (IsEmpty) {
            return;
        }

        // Calcul du nombre de subdivisions sur chaque axe
        float heightCopy = Height; // Calcule le getter une seule fois
        if (_width > 0 && heightCopy > 0 && MinSubdivisions >= 1) {
            if (_width > heightCopy) {
                _heightDataRowCount = MinSubdivisions +1;
                _heightDataColCount = Mathf.CeilToInt (_width / heightCopy * MinSubdivisions) +1;
            } else {
                _heightDataColCount = MinSubdivisions +1;
                _heightDataRowCount = Mathf.CeilToInt (heightCopy / _width * MinSubdivisions) +1;
            }
        }

        // Unity ne gère pas les meshes de plus de 65k sommets
        // Il faudrait implémenter un découpage en plusieurs meshes si on voulait plus de détail
        if ((_heightDataColCount) * (_heightDataRowCount) > 65000) {
            Debug.LogError ("Ce terrain est trop détaillé, Unity ne peut pas représenter simplement un Mesh de cette taille");
            return;
        }

        // Instanciation du tableau des hauteurs.
        _heightData = new float[_heightDataColCount][];
        for (int i = 0; i < _heightDataColCount; i++)
            _heightData [i] = new float[_heightDataRowCount];

        // Echantillonnage de l'image source
        for (int colIdx = 0; colIdx < _heightDataColCount; colIdx++) {
            for (int rowIdx = 0; rowIdx < _heightDataRowCount; rowIdx++) {
                _heightData[colIdx][rowIdx] = SampleBaseHeight((float)colIdx / (_heightDataColCount -1), (float)rowIdx / (_heightDataRowCount -1));

            }
        }

        ApplyHeightData ();
    }

    /// <summary>
    /// Applique une modification du terrain sur une zone
    /// </summary>
    public void ApplyOnZone(Rect targetZone, TerrainTransform transformOperator) {
        if (IsEmpty)
            return;

        // Détermination des indices concernés
        int colIndexMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.xMin + Width / 2) / Width * (_heightDataColCount -1), 0, _heightDataColCount-1));
        int colIndexMax = (int) Mathf.Clamp((targetZone.xMax + Width / 2) / Width * (_heightDataColCount -1), 0, _heightDataColCount-1);
        int rowIndexMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.yMin + Height / 2) / Height * (_heightDataRowCount -1), 0, _heightDataRowCount-1));
        int rowIndexMax = (int) Mathf.Clamp((targetZone.yMax + Height / 2) / Height * (_heightDataRowCount -1), 0, _heightDataRowCount-1);
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
                    ((float) colIndex / _heightDataColCount - 0.5f) * Width,
                    _heightData [colIndex] [rowIndex],
                    ((float) rowIndex / _heightDataRowCount - 0.5f) * Height);
                copy [colIndex - colIndexMin] [rowIndex - rowIndexMin] = transformOperator(position, TerrainMaxHeight);
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

    private void ApplyHeightData() {
        int nSubdivX = _heightDataColCount -1;
        int nSubdivY = _heightDataRowCount -1;

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
        _heightMapMesh.vertices = vertices;

        // Recalcul des triangles et UV
        RebuildTrianglesAndUVs(nSubdivX, nSubdivY);

        if (TargetMeshFilter != null)
            TargetMeshFilter.mesh = _heightMapMesh;
        if (TargetMeshCollider != null)
            TargetMeshCollider.sharedMesh = _heightMapMesh;
    }

    private void RebuildTrianglesAndUVs(int nSubdivX, int nSubdivY) {

        List<Vector4> uvs = new List<Vector4>();
        for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
            for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                float uvx = (float)colIdx / nSubdivX;
                float uvy = (float)rowIdx / nSubdivY;

                Vector4 newVect = new Vector2(uvx, uvy);
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

        _heightMapMesh.triangles = triangles;
        _heightMapMesh.SetUVs(0, uvs);
        _heightMapMesh.RecalculateNormals ();
        OnGeometryChanged();
    }

    /// <summary>
    /// Applique un filtre de Sobel sur les données de hauteur dynamiques
    /// pour estimer le gradient du terrain à l'endroit désigné.
    /// </summary>
    protected Vector2 SampleGradient(int colIdx, int rowIdx) {

        float h_1_1 = SampleHeightData(colIdx -1, rowIdx -1);
        float h_10 = SampleHeightData(colIdx -1, rowIdx);
        float h_11 = SampleHeightData(colIdx -1, rowIdx +1);
        float h0_1 = SampleHeightData(colIdx, rowIdx -1);
        float h01 = SampleHeightData(colIdx, rowIdx +1);
        float h1_1 = SampleHeightData(colIdx +1, rowIdx -1);
        float h10 = SampleHeightData(colIdx +1, rowIdx);
        float h11 = SampleHeightData(colIdx +1, rowIdx +1);

        float hStep = Width / (_heightDataColCount - 1);
        float dx = (h1_1 + 2 * h10 + h11 - h_1_1 - 2 * h_10 - h_11) / 4 / hStep;
        if (colIdx < 0 || colIdx >= _heightDataColCount)
            dx *= 2;

        float vStep = Height / (_heightDataRowCount -1);
        float dy = (h_11 + 2*h01 + h11 - h_1_1 - 2*h0_1 - h1_1) / 4 / vStep;
        if (rowIdx < 0 || rowIdx >= _heightDataRowCount)
            dy *= 2;

        Vector2 res = new Vector2(dx, dy);
        return res;
    }

    protected float SampleHeightData(int colIdx, int rowIdx) {
        if (IsEmpty)
            return 0;

        int clampedColIdx = colIdx;
        if (clampedColIdx < 0)
            clampedColIdx = 0;
        if (clampedColIdx >= _heightDataColCount)
            clampedColIdx = _heightDataColCount - 1;
        int clampedRowIdx = rowIdx;
        if (clampedRowIdx < 0)
            clampedRowIdx = 0;
        if (clampedRowIdx >= _heightDataRowCount)
            clampedRowIdx = _heightDataRowCount - 1;
        return _heightData[clampedColIdx][clampedRowIdx];
    }

    protected virtual void Start () {
        _heightMapMesh = new Mesh ();
        _heightMapMesh.name = "AltitudeMesh";
	}
	
    protected virtual void Update () {
		
	}

    /// <summary>
    /// Calcule la hauteur du monde à partir de sa largeur.
    /// </summary>
    protected abstract float ComputeTerrainHeight(float width);

    /// <summary>
    /// Calcule la hauteur de base du terrain (sans les modificateurs)
    /// </summary>
    protected abstract float SampleBaseHeight(float relativeX, float relativeY);

    protected virtual void OnGeometryChanged() {

    }
}
