using System;
using UnityEngine;

namespace AssemblyCSharp
{
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

        [SerializeField]

        /// <summary>
        /// Hauteur du "monde"
        /// </summary>
        public float Height {
            get {
                return _height;
            }
        }

        private float _height = 0;

        /// <summary>
        /// Maillage du terrain
        /// </summary>
        public Mesh HeightMapMesh{ get; private set; }

        // Les données de hauteur
        private float[][] _heightData;

        /// <summary>
        /// Obtient la hauteur du terrain aux coordonnées spécifiées.
        /// </summary>
        public double GetHeight (float x, float y)
        {
            double res = 0;
            if (_heightData != null && _heightData.Length >= 2) {
                int colIdx = (int)(Mathf.Clamp (x, 0, _width) / _width * (_heightData.Length - 1));
                float[] col = _heightData [colIdx];
                if (col.Length >= 2) {
                    int rowIdx = (int)(Mathf.Clamp (y, 0, _height) / _height * (col.Length - 1));
                    res = col [rowIdx];
                }
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
            // RecomputeSamples ();
        }

        public delegate float TerrainTransform(Vector3 localPosition, float currentValue);

        public void ApplyOnZone(Rect targetZone, TerrainTransform transformOperator) {
            if (IsEmpty())
                return;

            // Détermination des indices concernés
            int nCols = _heightData.Length;
            int nRows = _heightData [0].Length;
            int colIdXMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.xMin + Width / 2) / Width * (nCols -1), 0, nCols-1));
            int colIdXMax = (int) ((Width / 2 - targetZone.xMax) / Width * (nCols -1));
            int colIdYMin = Mathf.CeilToInt(Mathf.Clamp((targetZone.yMin + Height / 2) / Height * (nRows -1), 0, nRows-1));
            int colIdYMax = (int) ((Height / 2 - targetZone.yMax) / Height * (nRows -1));

            // Copie de la zone affectée pour faire les modifs sans changer
            // l'état courant.

            // Passage de l'opérateur

            // Recopie des nouvelles données, modification de l'état courant.

            // Recalcul du mesh.

        }

        /// <summary>
        /// Echantillonne la hauteur du terrain sur une grille
        /// de pas relativement égal dans les deux directions.
        /// </summary>
        private void RecomputeSamples ()
        {
            HeightMapMesh.Clear ();
            float[][] oldData = _heightData;
            _heightData = null;

            // test de validité
            if (IsEmpty()) {
                return;
            }

            _height = Width * _heightMapTexture.height / _heightMapTexture.width;

            // Calcul du nombre de subdivisions sur chaque axe
            int nSubdivX = 0;
            int nSubdivY = 0;
            if (_width > 0 && _height > 0 && _minSubdivisions >= 1) {
                if (_width > _height) {
                    nSubdivY = _minSubdivisions;
                    nSubdivX = Mathf.CeilToInt (_width / _height * _minSubdivisions);
                } else {
                    nSubdivX = _minSubdivisions;
                    nSubdivY = Mathf.CeilToInt (_height / _width * _minSubdivisions);
                }
            }

            // Comparaison avec la taille précédente
            bool sizeChanged;
            if (oldData != null) {
                sizeChanged = nSubdivX + 1 != oldData.Length || nSubdivY + 1 != oldData [0].Length;
                oldData = null; // Pour que le GC puisse libérer ce tableau avant d'instancier le nouveau.
            } else {
                sizeChanged = true;
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
                    int pixelXIdx = (int)((float)colIdx / nSubdivX * _heightMapTexture.width);
                    int pixelYIdx = (int)((float)rowIdx / nSubdivY * _heightMapTexture.height);
                    Color c = _heightMapTexture.GetPixel (pixelXIdx, pixelYIdx);
                    _heightData [colIdx] [rowIdx] = c.grayscale * TerrainMaxHeight;
                }
            }

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

            // Recalcul des triangles et UV si nécessaire
            if (sizeChanged) {
                RebuildTrianglesAndUVs(nSubdivX, nSubdivY);
            }

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

        private void RebuildTrianglesAndUVs() {
            int nSubdivX = 0;
            int nSubdivY = 0;
            if (_width > 0 && _height > 0 && _minSubdivisions >= 1) {
                if (_width > _height) {
                    nSubdivY = _minSubdivisions;
                    nSubdivX = Mathf.CeilToInt (_width / _height * _minSubdivisions);
                } else {
                    nSubdivX = _minSubdivisions;
                    nSubdivY = Mathf.CeilToInt (_height / _width * _minSubdivisions);
                }
            }
            RebuildTrianglesAndUVs(nSubdivX, nSubdivY);
        }

        private void RebuildTrianglesAndUVs(int nSubdivX, int nSubdivY) {
            
            Vector2[] uvs = new Vector2[(nSubdivX + 1) * (nSubdivY + 1)];
            for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
                for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                    int vertexIdx = colIdx * (nSubdivY + 1) + rowIdx;
                    uvs [vertexIdx] = new Vector2 ((float) colIdx / nSubdivX, (float) rowIdx / nSubdivY);
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
            HeightMapMesh.uv = uvs;
        }

    }
}

