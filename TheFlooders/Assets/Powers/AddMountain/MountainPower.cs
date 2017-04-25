using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MountainPower : Power
{

    public override void Fire()
    {
        base.Fire();
        LevelInfo lInfo = LevelInfo.Instance;
        TerrainHeightMap ground = (lInfo != null ? lInfo.Ground : null);
        if (ground != null)
            ground.AddModifier(new TerrainElevationModifier(transform.position, 25, 10));
        else
            Debug.LogError("Impossible de déclencher le pouvoir, le terrain n'a pas été trouvé");
    }
}
