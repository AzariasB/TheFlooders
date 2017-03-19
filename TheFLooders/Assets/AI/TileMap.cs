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

        float xSteps = Width / (NodeColumns);
        float zSteps = Height / (NodeRows);

        float startX = -(Width / 2) + (xSteps / 2);
        float startZ = -(Height / 2) + (zSteps / 2);

        //Create nodes
        for (int z = 0; z < NodeRows;z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                float zPos = (z * zSteps) + startZ;
                float xPos = (x * xSteps) + startX;
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
        Material redMat = Resources.Load("RedMaterial", typeof(Material)) as Material;
        FloodControl Control = GameObject.Find("Flood wave control").GetComponent<FloodControl>();

        float waterHeight = Control.stillWaterPlane.transform.position.y;

        //Shows all the nodes
        for (int z = 0; z < NodeRows; z++)
        {
            for(int x = 0; x < NodeColumns; x++)
            {
                

                GraphNode mNode = _nodes[z][x];
                if (mNode != null)
                {
                    if(mNode.Position.y <= waterHeight)
                    {
                        mNode.Sink();
                        mNode.Sinked = true;
                    }
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

    void Update()
    {
        FloodControl Control = GameObject.Find("Flood wave control").GetComponent<FloodControl>();
        float waterHeight = Control.stillWaterPlane.transform.position.y;
        //Shows all the nodes
        for (int z = 0; z < NodeRows; z++)
        {
            for (int x = 0; x < NodeColumns; x++)
            {


                GraphNode mNode = _nodes[z][x];
                if (mNode != null)
                {
                    foreach (Edge e in mNode.Edges)
                    {
                        if (e.IsNode1(mNode))
                        {
                            Debug.DrawLine(mNode.Position, e.Node2.Position, Color.green);
                        }
                    }

                    if (!mNode.Sinked)
                    {
                        bool rayCast;
                        RaycastHit hitPoint;

                        Vector3 target = mNode.Position;
                        Vector3 flatMapPosition = GameObject.Find("FlatMap").gameObject.transform.position;
                        Vector3 origin = new Vector3(target.x, flatMapPosition.y - 5, target.z);
                        Vector3 direction = target - origin;

                        Debug.DrawRay(origin, direction);


                        //Ray ray = Camera.main.ScreenPointToRay(mNode.Position);
                        rayCast = Physics.Raycast(origin, direction,out hitPoint, Mathf.Infinity);
                        if (rayCast &&  hitPoint.collider.gameObject.name != "HeightMap")//Can't see it => underwater
                        {
                            //Debug.DrawLine(origin, hitPoint.collider.gameObject.transform.position, Color.red);
                            mNode.Sink();
                        }

                    }
                }


            }
        }
    }
}
