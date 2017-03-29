using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodMeshAnimator : TerrainBuilder
{
    [Tooltip("Le terrain pour lequel ce script génère un mesh d'inondation")]
    public TerrainHeightMap terrain;

   
    protected override void Start() {
        base.Start();
        
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
            RebuildTerrain();
        } else {
            Debug.LogWarning("Le terrain cible ne semble pas initialisé. Il devrait être placé avant ce script dans l'ordre d'exécution du projet Unity");
            StartCoroutine(RebuildTerrainAfterOneFrame());
        }
    }

    /// <summary>
    /// Cette coroutine attend une frame avant de calculer le mesh, au cas où l'ordre d'exécution
    /// des scripts soit mal réglé.
    /// </summary>
    private IEnumerator RebuildTerrainAfterOneFrame() {
        // On attend une frame
        yield return null;

        // Puis on lance l'action
        RebuildTerrain();
    }

    protected override float ComputeTerrainHeight(float width)
    {
        float res = 0;
        if (terrain != null && terrain.Width != 0)
            res = width * terrain.Height / terrain.Width;
        return res;
    }

    protected override float SampleBaseHeight(float relativeX, float relativeY) {
        return 0;
    }

}

