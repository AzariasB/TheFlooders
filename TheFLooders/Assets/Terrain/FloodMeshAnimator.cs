using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodMeshAnimator : MonoBehaviour
{
    [Tooltip("Le terrain pour lequel ce script génère un mesh d'inondation")]
    public TerrainHeightMap terrain;

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
                RebuildMesh ();
            }
        }
    }

    // Les données de hauteur
    private float[][] _heightData;
    private int heightDataNCols;
    private int heightDataNRows;

    private void Start() {
        
        // Récupération du terrain pour lequel un mesh d'inondation sera généré
        if (terrain == null)
            terrain = GetComponent<TerrainHeightMap>();
        if (terrain == null)
        {
            Debug.LogError("Impossible de trouver le terrain cible pour créer un mesh d'inondation");
            enabled = false;
            return;
        }

        // Le terrain s'initialise dans son Start().
        // Ce script devrait s'exécuter après, via les réglages de ScriptExecutionOrder du projet.
        // Mais au cas où ça ne serait pas fait, on attend une frame.
        // Typiquement un terrain non initialisé aura Width == Height == 0.
        if (terrain.Height > 0 && terrain.Width > 0) {
            RebuildMesh();
        } else {
            Debug.LogWarning("Le terrain cible ne semble pas initialisé. Il devrait être placé avant ce script dans l'ordre d'exécution du projet Unity");
            StartCoroutine(RebuildMeshAfterOneFrame());
        }
    }

    private IEnumerator RebuildMeshAfterOneFrame() {
        yield return null;
        RebuildMesh();
    }

    private void RebuildMesh() {

    }
}

