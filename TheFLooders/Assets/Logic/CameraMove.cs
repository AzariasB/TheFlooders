using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp {
    public class CameraMove : MonoBehaviour {

        [Tooltip("Le terrain par rapport le long duquel cette caméra doit défiler.")]
        public TerrainHeightMap targetTerrain;

        [Tooltip("La caméra du niveau, dont la taille sera configurée")]
        public Camera targetCamera;

        private Vector3 _targetPosition;
        private float _accumulatedTime;

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

                // Calcul de la vitesse de translation future
                _targetPosition = new Vector3 (0, 100, (- targetTerrain.Height + desiredCamHeight) / 2);
            }
        }

        // Update is called once per frame
        void Update () {
            _accumulatedTime += Time.deltaTime;
            if (_accumulatedTime > LevelInfo.Instance.StartDelay) {
                if (_accumulatedTime - LevelInfo.Instance.StartDelay < LevelInfo.Instance.LevelDuration) {
                    float remainingTime = LevelInfo.Instance.LevelDuration - (_accumulatedTime - LevelInfo.Instance.StartDelay);
                    Vector3 direction = (_targetPosition - transform.position).normalized;
                    float speed = 0;
                    if (remainingTime != 0)
                        speed =(_targetPosition - transform.position).magnitude / remainingTime;
                    transform.Translate(direction * speed * Time.deltaTime);
                } else {
                    transform.position = _targetPosition;
                    enabled = false;
                }
            }
        }
    }
}