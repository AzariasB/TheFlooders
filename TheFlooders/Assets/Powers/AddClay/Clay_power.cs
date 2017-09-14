using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clay_power : Power {

	[Tooltip("La taille de la zone à modifier")]
	public int modifier_size; //La taille de la zone à modifier
	[Tooltip("la hauteur de la modification de l'eau (profondeur si negative)")]
	public int modifier_height; //la hauteur de la montagne (profondeur du trou si negatif)

	public override void Fire()
	{
		base.Fire();
		LevelInfo lInfo = LevelInfo.Instance; //faire appel à la nouvelle variable correspondant au plan d'eau pour modifier celui-ci
		FloodMeshAnimator water = (lInfo != null ? lInfo.Ground_eau : null);
		if (water != null)
			water.AddModifier(new TerrainElevationModifier(transform.position, modifier_size, modifier_height));
		else
			Debug.LogError("Impossible de déclencher le pouvoir, le terrain n'a pas été trouvé");
	}
}
