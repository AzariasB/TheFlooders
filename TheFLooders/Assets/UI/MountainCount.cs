using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainCount : TextBinding {

    public MountainCount() : base(1, "Ajouter Montagne ({0})")
    {
    }


    public bool CanPlaceMountain()
    {
        return Count > 0;
    }

    public void PlaceMountain()
    {
        ReduceCount();
    }
}
