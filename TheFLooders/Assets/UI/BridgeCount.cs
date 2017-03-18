using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BridgeCount : TextBinding {


    public BridgeCount():base(2, "Ajouter pont ({0})")
    {
    }


    public bool CanPlaceBridge()
    {
        return Count > 0;
    }


    public void PlaceBridge()
    {
        base.ReduceCount();
        
    }



}
