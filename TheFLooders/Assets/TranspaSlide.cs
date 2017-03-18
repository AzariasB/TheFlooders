using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranspaSlide : MonoBehaviour {

    [Tooltip("Calque d'arrière-plan à dupliquer")]
    public GameObject backgroundLayer;

    [Tooltip("Nombre de calques superposés au calque de fond")]
    public int layerCount = 2;

    [Tooltip("Amplitude du mouvement")]
    public float amplitude = 1;

    [Tooltip("Vitesse d'oscillation")]
    public float timeScale = 0.33f;

    [Tooltip("Transparence des couches superposées au fond")]
    public float layersTransparency = 0.5f;

    private List<GameObject> createdLayers;

    [HideInInspector]
    public float yDelta {get; set;}

    private float elapsedTime = 0;

	void Start () {
        yDelta = 0.01f;
        if (backgroundLayer == null) {
            Debug.LogError("Le calque d'arrière-plan n'est pas affecté");
        }
	}
	
	void Update () {
        elapsedTime += Time.deltaTime;
        layerCount = (layerCount >= 0 ? layerCount : 0);

        if (backgroundLayer != null && layerCount != createdLayers.Count) {
            while (createdLayers.Count > layerCount) {
                Destroy(createdLayers [0]);
                createdLayers.RemoveAt(0);
            }
            while (createdLayers.Count > layerCount) {
                GameObject newLayer = Instantiate(backgroundLayer, backgroundLayer.transform, false);
                createdLayers.Add(newLayer);
            }
        }
	}
}
