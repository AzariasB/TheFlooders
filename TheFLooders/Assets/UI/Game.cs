using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {


    public static int Bridges { get; set; }
    public static int Mountains { get; set; }

    private Text _text;

    public Game()
    {
        Bridges = 2;
        Mountains = 1;
        _text = gameObject.GetComponent<Text>();
        
    }


    public static bool CanPlaceMountain()
    {
        return Mountains > 0;
    }


    public static void MountainPlaced()
    {
        Mountains--;
    }
}
