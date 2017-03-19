using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour {

    private GraphNode[][] _nodes;

    public int NodeRows;
    public int NodeColumns;

	// Use this for initialization
	void Start () {
        _nodes = new GraphNode[NodeRows][];
        for(int i = 0; i < NodeRows; i++)
        {
            _nodes[i] = new GraphNode[NodeColumns];
        }

        TerrainHeightMap heightMap = GameObject.Find("HeightMap").GetComponent<TerrainHeightMap>();

        float Width = heightMap.Width;
        float Height = heightMap.Height;

        float xSteps = Width / NodeColumns;
        float zSteps = Height / NodeRows;

        //Create nodes
        for (int z = 0; z < NodeRows;z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                float zPos = z * zSteps;
                float xPos = x * xSteps;
                float yPos = (float)heightMap.GetHeight(xPos, zPos);
                Vector3 nodePos = new Vector3(xPos, yPos, zPos);
                //Check water height
                _nodes[z][x] = new GraphNode(nodePos);
            }
        }

        //Create edges
        for(int z =0; z < NodeRows; z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                GenerateEdge(x, z);
            }
        }

        //Debug print
        DebugPrint();
	}

    private void DebugPrint()
    {
        //Shows all the nodes
        for(int z = 0; z < NodeRows; z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                

                GraphNode mNode = _nodes[z][x];
                if (mNode != null)
                {
                    GameObject nwBrige = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    nwBrige.transform.position = mNode.Position;
                }


            }
        }
    }

    private void GenerateEdge(int x, int z)
    {
        GraphNode mNode = _nodes[z][x];
        if(CanConnect(x-1, z, mNode ))
        {
            new Edge(mNode, _nodes[z][x-1]);
        }
        if(CanConnect(x+1, z, mNode) )
        {
            new Edge(mNode, _nodes[z][x + 1]);
        }
        if(CanConnect(x, z-1, mNode))
        {
            new Edge(mNode, _nodes[z - 1][x]);
        }
        if(CanConnect(x, z-1, mNode))
        {
            new Edge(mNode, _nodes[z + 1][x]);
        }
    }

    /// <summary>
    /// Checks wether a connection can be done with the node at the given coordinates
    /// If the given node already has a connection, whill return false
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="connector"></param>
    /// <returns></returns>
    private bool CanConnect(int x, int z, GraphNode connector)
    {
        return IsValidCoord(x, z) && _nodes[z][x] != null && !connector.AlreadyConnectedTo(_nodes[z][x]);
    }

    private bool IsValidCoord(int x, int z)
    {
        return x >= 0 && x < NodeColumns && z >= 0 && z < NodeRows;
    }

}
