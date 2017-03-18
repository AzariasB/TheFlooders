using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp {
    public class CameraMove : MonoBehaviour {

        private bool _started = false;

        [Tooltip("Temps à partir duquel la caméra commence à bouger")]
        public float TimeStart;

        public int Speed;

        [Tooltip("Le terrain par rapport le long duquel cette caméra doit défiler.")]
        public TerrainHeightMap targetTerrain;

        [Tooltip("La caméra du niveau, dont la taille sera configurée")]
        public Camera targetCamera;

        private float _currentTime;

        // Use this for initialization
        void Start () {
            if (targetTerrain == null) {
                Debug.LogError ("Terrain cible manquant");
                return;
            }
            if (targetCamera == null) {
                Debug.LogError ("Caméra manquante");
                return;
            }

            if (targetTerrain != null) {
                // Config de la taille de la caméra
                float desiredCamHeight = targetTerrain.Width * Screen.height / Screen.width;
                targetCamera.orthographic = true;
                targetCamera.orthographicSize = desiredCamHeight / 2;

                // Positionnement de la caméra
                transform.parent = targetTerrain.transform;
                transform.localPosition = new Vector3 (0, 100, (targetTerrain.Height - desiredCamHeight) / 2);
            }
        }

        // Update is called once per frame
        void Update () {
            if(_started)
            {
                gameObject.transform.position += new Vector3(0, 0, -Speed * Time.deltaTime);
                if(gameObject.transform.position.z < 42)
                {
                    //No need to move anymore
                    Destroy(gameObject.GetComponent<CameraMove>());
                }
            }
            else
            {
                _currentTime += Time.deltaTime;
                if(_currentTime >= TimeStart)
                {
                    _started = true;
                }
            }
        }
    }
}