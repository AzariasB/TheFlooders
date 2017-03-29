using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainBuilder : MonoBehaviour
{

    [Tooltip("Largeur du \"monde\".")]
    [SerializeField]
    private float _width = 100;

    /// <summary>
    /// Largeur du "monde". Sert de base au calcul de la hauteur
    /// </summary>
    public float Width
    {
        get
        {
            return _width;
        }
        set
        {
            if (_width != value)
            {
                _width = value;
                RebuildTerrain();
            }
        }
    }

    /// <summary>
    /// Hauteur du "monde". Calculée à partir de la largeur
    /// </summary>
    public float Height
    {
        get
        {
            return ComputeTerrainZDimension(Width);
        }
    }

    [Tooltip("Nombre de subdivisions de la dimension la plus petite du terrain.")]
    [SerializeField]
    private int _minSubdivisions = 1;

    public int MinSubdivisions
    {
        get
        {
            return _minSubdivisions;
        }
        set
        {
            if (_minSubdivisions != value)
            {
                _minSubdivisions = value;
                RebuildTerrain();
            }
        }
    }

    /// <summary>
    /// Retourne vrai ssi le maillage est vide ou ne peut être calculé.
    /// </summary>
    public virtual bool IsEmpty
    {
        get
        {
            return (_heightData == null || Width <= 0 || Height <= 0 || MinSubdivisions < 1);
        }
    }

    [Tooltip("MeshFilter cible où ce composant écrit le mesh qu'il produit automatiquement")]
    public MeshFilter TargetMeshFilter;

    [Tooltip("MeshCollider cible où ce composant écrit le mesh qu'il produit automatiquement")]
    public MeshCollider TargetMeshCollider;

    [Tooltip("Hauteur maximale du terrain, approximative (c'est un guide pour les modificateurs)")]
    public float TerrainMaxHeight = 20;

    /// <summary>
    /// Taille du tableau des altitudes sur X
    /// </summary>
    protected int HeightDataColCount
    {
        get { return (_heightData != null ? _heightData.Length : 0); }
    }

    /// <summary>
    /// Taille du tableau des altitudes sur Z
    /// </summary>
    protected int HeightDataRowCount
    {
        get { return (HeightDataColCount > 0 ? _heightData[0].Length : 0); }
    }

    /// <summary>
    /// Maillage du terrain
    /// </summary>
    protected Mesh HeightMapMesh
    {
        get { return _heightMapMesh; }
    }

    private Mesh _heightMapMesh;

    /// <summary>
    /// Tableau des hauteurs finales du terrain, incluant la hauteur de
    /// base et tous les modificateurs
    /// </summary>
    private float[][] _heightData;

    /// <summary>
    /// Indice du dernier modificateur appliqué sur les données de hauteur.
    /// </summary>
    private int? _heightDataModLevel;

    /// <summary>
    /// Lorsque le début de la pile des modificateurs (mais pas tous) a rapporté être
    /// constant lors de la dernière frame, ceci contient le calcul partiel de hauteur
    /// suite à l'application de cette partie des modificateurs.
    /// </summary>
    private float[][] _cachedHeightData;

    /// <summary>
    /// Lorsque le début de la pile des modificateurs (mais pas tous) a rapporté être
    /// constant lors de la dernière frame, ceci contient l'index du dernier modificateur
    /// de ce groupe.
    /// </summary>
    private int? _cachedHeightDataModLevel;

    /// <summary>
    /// Pile des modificateurs appliqués à ce terrain
    /// </summary>
    private List<HeightModifier> _modifierStack;

    protected virtual void Awake()
    {
        _heightMapMesh = new Mesh();
        _heightMapMesh.name = "AltitudeMesh";
        _modifierStack = new List<HeightModifier>();
        RebuildTerrain();
    }

    protected virtual void Update()
    {
        ApplyModifiers();
    }

    public float GetHeight(float x, float z) {
        if (IsEmpty)
            return 0;
        int colIdx = (int)Mathf.Clamp((x + Width / 2) / Width * (HeightDataColCount - 1), 0, HeightDataColCount - 1);
        int rowIdx = (int)Mathf.Clamp((z + Height / 2) / Height * (HeightDataRowCount - 1), 0, HeightDataRowCount - 1);
        // TODO: une interpolation entre les sommets proches
        return _heightData[colIdx][rowIdx];
    }

    /// <summary>
    /// Ajoute un modificateur de hauteur sur le terrain, qui modifiera
    /// sa forme en s'ajoutant par-dessus les autres modificateurs déjà présents.
    /// </summary>
    public void AddModifier(HeightModifier modifier)
    {
        if (modifier != null)
        {
            _modifierStack.Add(modifier);
        }
    }

    /// <summary>
    /// Supprime un modificateur de hauteur du terrain.
    /// </summary>
    public void RemoveModifier(HeightModifier modifier)
    {
        // On utilise LastIndexOf pour retirer le modificateur
        // du haut de la pile, si on a ajouté plusieurs fois le même.
        int modIndex = _modifierStack.LastIndexOf(modifier);
        if (modIndex >= 0)
        {
            // Dans tous les cas, le tableau principal devient invalide.
            _heightData = null;
            _heightDataModLevel = null;

            // S'il y avait des infos en cache et qu'elles sont affectées par
            // la suppression de ce mod, il faut les jeter aussi.
            if (_cachedHeightDataModLevel.HasValue && _cachedHeightDataModLevel.Value >= modIndex)
            {
                _cachedHeightData = null;
                _cachedHeightDataModLevel = null;
            }
            _modifierStack.RemoveAt(modIndex);
        }
    }

    /// <summary>
    /// Reconstruit complètement les données d'altitude et le mesh du terrain.
    /// </summary>
    private void RebuildTerrain()
    {
        // Défausse des données actuelles et du cache
        // Assignation quand même d'un tableau de la bonne taille dans
        // heightData pour que le mesh soit construit aux bonnes dimensions.
        _heightData = CreateHeightData();
        _heightDataModLevel = -1;
        _cachedHeightData = null;
        _cachedHeightDataModLevel = null;

        // Reconstruction du mesh.
        RebuildMesh();

        // Recalcul des informations de hauteur
        ApplyModifiers();
    }

    /// <summary>
    /// Instancie un tableau destiné à stocker la hauteur du terrain en différents points.
    /// </summary>
    private float[][] CreateHeightData()
    {

        // Calcul du nombre de subdivisions sur chaque axe
        int heightDataColCount = 0;
        int heightDataRowCount = 0;
        float heightCopy = Height; // Calcule le getter une seule fois
        if (_width > 0 && heightCopy > 0 && MinSubdivisions >= 1)
        {
            if (_width > heightCopy)
            {
                heightDataRowCount = MinSubdivisions + 1;
                heightDataColCount = Mathf.CeilToInt(_width / heightCopy * MinSubdivisions) + 1;
            }
            else
            {
                heightDataColCount = MinSubdivisions + 1;
                heightDataRowCount = Mathf.CeilToInt(heightCopy / _width * MinSubdivisions) + 1;
            }
        }

        // Unity ne gère pas les meshes de plus de 65k sommets
        // Il faudrait implémenter un découpage en plusieurs meshes si on voulait plus de détail
        if ((heightDataColCount) * (heightDataRowCount) > 65000)
        {
            Debug.LogError("Ce terrain est trop détaillé, Unity ne peut pas représenter simplement un Mesh de cette taille");
        }

        float[][] res = new float[heightDataColCount][];
        for (int i = 0; i < heightDataColCount; i++)
            res[i] = new float[heightDataRowCount];
        return res;
    }

    /// <summary>
    /// Met à jour la hauteur calculée des points du terrain en fonction des
    /// modificateurs présents, et met à jour le mesh si nécessaire.
    /// </summary>
    private void ApplyModifiers()
    {        
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();

        // Parcours des modificateurs pour voir jusqu'où ils se déclarent constants
        // Si aucun modificateur ne change, on prend le premier indice situé après
        // la fin de la pile.
        int firstChanging = _modifierStack.Count;
        for (int i = 0; i < _modifierStack.Count; i++)
        {
            if (_modifierStack[i].IsChanging)
            {
                firstChanging = i;
                break;
            }
        }
        
        // indice du premier modificateur restant à appliquer.
        int firstModToApply = 0;

        // Examen de la structure principale, pour voir si
        // elle est à reconstruire ou si elle peut être réutilisée.
        if (_heightData != null && _heightDataModLevel.HasValue)
        {
            if (_heightDataModLevel.Value < firstChanging)
            {
                // Les données présentes sont soit à jour,
                // soit on peut les compléter incrémentalement.
                firstModToApply = _heightDataModLevel.Value +1;
            }
            else
            {
                // Les données ont été invalidées, on les réinitialise.
                _heightData = CreateHeightData();
                _heightDataModLevel = -1;
            }
                
        }
        else
        {
            // La structure a été jetée ou n'a jamais été créée ; il faut la reconstruire.
            _heightData = CreateHeightData();
            _cachedHeightDataModLevel = -1;
        }
        
        // On jette le cache si il a été invalidé
        if (_cachedHeightData != null && _cachedHeightDataModLevel.HasValue)
        {
            if (firstChanging <= _cachedHeightDataModLevel.Value)
            {
                _cachedHeightData = null;
                _cachedHeightDataModLevel = null;
            }
        }

        // On regarde à présent si le cache peut être utile
        if (firstModToApply < _modifierStack.Count && _cachedHeightData != null &&
            _cachedHeightDataModLevel.HasValue && _cachedHeightDataModLevel.Value > firstModToApply)
        {
            // Application du cache sur les données principales
            for (int colIndex = 0; colIndex < HeightDataColCount; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < HeightDataRowCount; rowIndex++)
                {
                    _heightData[colIndex][rowIndex] = _cachedHeightData[colIndex][rowIndex];
                }
            }
            _heightDataModLevel = _cachedHeightDataModLevel;
            firstModToApply = _heightDataModLevel.Value + 1;
        }

        // Application des modificateurs restants
        int nApplied = _modifierStack.Count - firstModToApply;
        for (int modIndex = firstModToApply; modIndex < _modifierStack.Count; modIndex++)
        {
            ApplySingleModifier(_modifierStack[modIndex], _heightData);
            // Mise à jour du cache si on vient d'appliquer le dernier modificateur constant
            if (modIndex == firstChanging -1) {
                _cachedHeightData = CreateHeightData();
                for (int colIndex = 0; colIndex < HeightDataColCount; colIndex++)
                {
                    for (int rowIndex = 0; rowIndex < HeightDataRowCount; rowIndex++)
                    {
                        _cachedHeightData[colIndex][rowIndex] = _heightData[colIndex][rowIndex];
                    }
                }
                _cachedHeightDataModLevel =_heightDataModLevel;
            }
        }
        _heightDataModLevel = _modifierStack.Count - 1;

        // Mise à jour du mesh
        if (nApplied > 0)
        {
            //Debug.Log(GetType().Name + ": Application de " + nApplied.ToString() + " modificateur(s) de hauteur en " + sw.ElapsedMilliseconds + "ms.");
            //sw.Reset();
            RebuildGeometry();
            //Debug.Log(GetType().Name + ": Reconstruction de la géométrie en " + sw.ElapsedMilliseconds + "ms.");
        }
    }

    /// <summary>
    /// Applique l'effet d'un modificateur à un terrain
    /// </summary>
    private void ApplySingleModifier(HeightModifier modifier, float[][] toHeightData)
    {
        if (IsEmpty)
            return;
        
        Rect targetZone = modifier.GetAreaOfEffect();

        // Détermination des indices concernés
        int colIndexMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.xMin + Width / 2) / Width * (HeightDataColCount - 1), 0, HeightDataColCount - 1));
        int colIndexMax = (int)Mathf.Clamp((targetZone.xMax + Width / 2) / Width * (HeightDataColCount - 1), 0, HeightDataColCount - 1);
        int rowIndexMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.yMin + Height / 2) / Height * (HeightDataRowCount - 1), 0, HeightDataRowCount - 1));
        int rowIndexMax = (int)Mathf.Clamp((targetZone.yMax + Height / 2) / Height * (HeightDataRowCount - 1), 0, HeightDataRowCount - 1);
        if (colIndexMax < colIndexMin || rowIndexMax < rowIndexMin)
            return;

        // Copie de la zone affectée pour faire les modifs sans changer
        // l'état courant.
        float[][] copy = new float[colIndexMax - colIndexMin + 1][];
        for (int i = 0; i < colIndexMax - colIndexMin + 1; i++)
            copy[i] = new float[rowIndexMax - rowIndexMin + 1];

        for (int colIndex = colIndexMin; colIndex <= colIndexMax; colIndex++)
        {
            for (int rowIndex = rowIndexMin; rowIndex <= rowIndexMax; rowIndex++)
            {
                copy[colIndex - colIndexMin][rowIndex - rowIndexMin] = toHeightData[colIndex][rowIndex];
            }
        }

        // Passage de l'opérateur
        for (int colIndex = colIndexMin; colIndex <= colIndexMax; colIndex++)
        {
            for (int rowIndex = rowIndexMin; rowIndex <= rowIndexMax; rowIndex++)
            {
                Vector3 position = new Vector3(
                                       ((float)colIndex / HeightDataColCount - 0.5f) * Width,
                                       toHeightData[colIndex][rowIndex],
                                       ((float)rowIndex / HeightDataRowCount - 0.5f) * Height);
                copy[colIndex - colIndexMin][rowIndex - rowIndexMin] = modifier.Apply(this, position);
            }
        }

        // Recopie des nouvelles données, modification de l'état courant.
        for (int colIndex = colIndexMin; colIndex <= colIndexMax; colIndex++)
        {
            for (int rowIndex = rowIndexMin; rowIndex <= rowIndexMax; rowIndex++)
            {
                toHeightData[colIndex][rowIndex] = copy[colIndex - colIndexMin][rowIndex - rowIndexMin];
            }
        }
    }

    /// <summary>
    /// Met à jour les
    /// </summary>
    /// <returns>The height data.</returns>
    private void RebuildMesh()
    {
        _heightMapMesh.Clear();

        // Recalcul des sommets
        RebuildGeometry();

        // Recalcul des triangles et UV
        RebuildTrianglesAndUVs();

        if (TargetMeshFilter != null)
            TargetMeshFilter.mesh = _heightMapMesh;
        if (TargetMeshCollider != null)
            TargetMeshCollider.sharedMesh = _heightMapMesh;
    }

    private void RebuildGeometry()
    {
        int nSubdivX = HeightDataColCount - 1;
        int nSubdivY = HeightDataRowCount - 1;

        // Transformation en vertex
        Vector3[] vertices = new Vector3[(nSubdivX + 1) * (nSubdivY + 1)];
        for (int colIdx = 0; colIdx <= nSubdivX; colIdx++)
        {
            for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++)
            {
                int vertexIdx = colIdx * (nSubdivY + 1) + rowIdx;
                float x = colIdx * Width / nSubdivX - Width / 2;
                float y = (_heightData != null ? _heightData[colIdx][rowIdx] : 0);
                float z = rowIdx * Height / nSubdivY - Height / 2;
                vertices[vertexIdx] = new Vector3(x, y, z);
            }
        }
        // Remplissage des sommets du mesh
        _heightMapMesh.vertices = vertices;

        // Mise à jour des normales et appel des classes filles.
        _heightMapMesh.RecalculateNormals();
        OnGeometryChanged();
    }

    private void RebuildTrianglesAndUVs()
    {
        int nSubdivX = HeightDataColCount - 1;
        int nSubdivY = HeightDataRowCount - 1;

        List<Vector4> uvs = new List<Vector4>();
        for (int colIdx = 0; colIdx <= nSubdivX; colIdx++)
        {
            for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++)
            {
                float uvx = (float)colIdx / nSubdivX;
                float uvy = (float)rowIdx / nSubdivY;

                Vector4 newVect = new Vector2(uvx, uvy);
                uvs.Add(newVect);
            }
        }

        int[] triangles = new int[6 * nSubdivX * nSubdivY]; // Un quad = 2 triangles = 6 sommets
        for (int colIdx = 0; colIdx < nSubdivX; colIdx++)
        {
            for (int rowIdx = 0; rowIdx < nSubdivY; rowIdx++)
            {
                int baseVertexIndex = (nSubdivY + 1) * colIdx + rowIdx;
                int baseIdx = 6 * (nSubdivY * colIdx + rowIdx);
                triangles[baseIdx] = baseVertexIndex;
                triangles[baseIdx + 1] = baseVertexIndex + 1;
                triangles[baseIdx + 2] = baseVertexIndex + nSubdivY + 1;
                triangles[baseIdx + 3] = baseVertexIndex + nSubdivY + 1;
                triangles[baseIdx + 4] = baseVertexIndex + 1;
                triangles[baseIdx + 5] = baseVertexIndex + nSubdivY + 2;
            }
        }

        _heightMapMesh.triangles = triangles;
        _heightMapMesh.SetUVs(0, uvs);
    }

    /// <summary>
    /// Applique un filtre de Sobel sur les données de hauteur dynamiques
    /// pour estimer le gradient du terrain à l'endroit désigné.
    /// </summary>
    protected Vector2 SampleGradient(int colIdx, int rowIdx)
    {
        if (IsEmpty)
            return Vector2.zero;

        float h_1_1 = SampleHeightData(colIdx - 1, rowIdx - 1);
        float h_10 = SampleHeightData(colIdx - 1, rowIdx);
        float h_11 = SampleHeightData(colIdx - 1, rowIdx + 1);
        float h0_1 = SampleHeightData(colIdx, rowIdx - 1);
        float h01 = SampleHeightData(colIdx, rowIdx + 1);
        float h1_1 = SampleHeightData(colIdx + 1, rowIdx - 1);
        float h10 = SampleHeightData(colIdx + 1, rowIdx);
        float h11 = SampleHeightData(colIdx + 1, rowIdx + 1);

        float hStep = Width / (HeightDataColCount - 1);
        float dx = (h1_1 + 2 * h10 + h11 - h_1_1 - 2 * h_10 - h_11) / 4 / hStep;
        if (colIdx < 0 || colIdx >= HeightDataColCount)
            dx *= 2;

        float vStep = Height / (HeightDataRowCount - 1);
        float dy = (h_11 + 2 * h01 + h11 - h_1_1 - 2 * h0_1 - h1_1) / 4 / vStep;
        if (rowIdx < 0 || rowIdx >= HeightDataRowCount)
            dy *= 2;

        Vector2 res = new Vector2(dx, dy);
        return res;
    }

    protected float SampleHeightData(int colIdx, int rowIdx)
    {
        if (IsEmpty)
            return 0;

        int clampedColIdx = colIdx;
        if (clampedColIdx < 0)
            clampedColIdx = 0;
        if (clampedColIdx >= HeightDataColCount)
            clampedColIdx = HeightDataColCount - 1;
        int clampedRowIdx = rowIdx;
        if (clampedRowIdx < 0)
            clampedRowIdx = 0;
        if (clampedRowIdx >= HeightDataRowCount)
            clampedRowIdx = HeightDataRowCount - 1;
        return _heightData[clampedColIdx][clampedRowIdx];
    }

    /// <summary>
    /// Calcule la hauteur du monde à partir de sa largeur.
    /// </summary>
    protected abstract float ComputeTerrainZDimension(float width);

    protected virtual void OnGeometryChanged()
    {

    }
}
