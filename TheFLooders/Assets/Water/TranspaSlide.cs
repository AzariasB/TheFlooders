using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranspaSlide : MonoBehaviour {

    [Tooltip("Calque transparent")]
    public GameObject transparentLayer;

    [Tooltip("Amplitude du mouvement")]
    public float amplitude = 1;

    [Tooltip("Vitesse d'oscillation")]
    public float timeScale = 0.33f;

    [Tooltip("Transparence des couches superposées au fond")]
    public float layersTransparency = 0.5f;

    [HideInInspector]
    public float yDelta {get; set;}

    public TranspaSlide() {
        yDelta = 0.01f;
    }

    void Start () {
        if (transparentLayer == null) {
            Debug.Log ("Calque transparent non affecté");
        }
    }

    void Update () {
        if (transparentLayer != null) {
            MeshRenderer mr = transparentLayer.GetComponent<MeshRenderer> ();
            Material mat = mr.material;
            if (mat != null) {
                // Position
                float phase = Time.time * timeScale;
                phase = phase - Mathf.PI * ((int) (phase / Mathf.PI)) - Mathf.PI/2;
                transparentLayer.transform.localPosition = new Vector3 (amplitude * Mathf.Sin (phase), yDelta, 0);

                // Couleur
                float alpha = Mathf.Clamp01 (layersTransparency * Mathf.Sin (phase + Mathf.PI / 2));
                mat.color = new Color (1, 1, 1, alpha);
            }
        }
    }
}
