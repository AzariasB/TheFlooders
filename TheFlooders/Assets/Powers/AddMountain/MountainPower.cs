using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MountainPower : Power
{
	[Tooltip("La taille de la zone à modifier")]
	public int modifier_size; //La taille de la zone à modifier
	[Tooltip("la hauteur de la montagne (profondeur du trou si negatif)")]
	public int modifier_height; //la hauteur de la montagne (profondeur du trou si negatif)

    public override void Fire()
    {
        base.Fire();
        LevelInfo lInfo = LevelInfo.Instance; //faire appel à la nouvelle variable correspondant au plan d'eau pour modifier celui-ci
        TerrainHeightMap ground = (lInfo != null ? lInfo.Ground : null);
        if (ground != null)
            ground.AddModifier(new TerrainElevationModifier(transform.position, modifier_size, modifier_height));
        else
            Debug.LogError("Impossible de déclencher le pouvoir, le terrain n'a pas été trouvé");
    }
}
