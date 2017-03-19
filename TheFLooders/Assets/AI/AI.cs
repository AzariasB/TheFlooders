using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    private Octree _octree;

    //
    public int NodeRows;

    public int NodeColumns;

	// Use this for initialization
	void Start () {
        Generate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // TODO : Pathfind (from pos to pos)

    /// <summary>
    /// Creates the graph
    /// </summary>
    void Generate()
    {
        for(int z = 0 ; z < NodeRows; z++)
        {
            for(int x = 0;  x < NodeColumns; x++)
            {

            }
        }
    }
}
