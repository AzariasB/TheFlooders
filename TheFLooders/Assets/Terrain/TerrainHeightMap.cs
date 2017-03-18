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

        [Tooltip ("Nombre de subdivisions de la dimension la plus petite du terrain.")]
        [SerializeField]
        private int _minSubdivisions = 10;

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
            RecomputeSamples ();
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
            if (_heightMapTexture == null || _width <= 0 || _height <= 0 && _minSubdivisions < 1)
                return;

            // Calcul du nombre de subdivisions sur chaque axe
            int nSubdivX = 0;
            int nSubdivY = 0;
            if (_width > 0 && _height > 0 && _minSubdivisions >= 1) {
                if (_width > _height) {
                    nSubdivY = _minSubdivisions;
                    nSubdivX = Mathf.CeilToInt (_width / _height * _minSubdivisions);
                } else {
                    nSubdivX = _minSubdivisions;
                    nSubdivY = Mathf.CeilToInt (_width / _height * _minSubdivisions);
                }
            }

            // Instanciation du tableau des hauteurs.
            _heightData = new float[nSubdivX + 1][];
            for (int i = 0; i <= nSubdivX; i++)
                _heightData [i] = new float[nSubdivY];

            // Echantillonnage de l'image source
            for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
                for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                    int pixelXIdx = (int)((float)colIdx / nSubdivX * _heightMapTexture.width);
                    int pixelYIdx = (int)((float)rowIdx / nSubdivY * _heightMapTexture.height);
                    Color c = _heightMapTexture.GetPixel (pixelXIdx, pixelYIdx);
                    _heightData [colIdx] [rowIdx] = c.grayscale;
                }
            }

            // Transformation en vertex, triangles et UVs
            Vector3[] vertices = new Vector3[(nSubdivX + 1) * (nSubdivY + 1)];
            for (int colIdx = 0; colIdx <= nSubdivX; colIdx++) {
                for (int rowIdx = 0; rowIdx <= nSubdivY; rowIdx++) {
                    float x = colIdx * Width / nSubdivX;
                    float y = rowIdx * Height / nSubdivY;
                    vertices [colIdx * (nSubdivX + 1) + rowIdx] = new Vector3 (x, y, _heightData [colIdx] [rowIdx]);
                }
            }

        }

    }
}

